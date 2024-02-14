using MySql.Data.MySqlClient;
using Neurotec.Biometrics;
using Neurotec.Images;
using Neurotec.IO;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Code
{
    public enum Order
    {
        Descending,
        Ascending
    }

    public class RecordCollection
    {
        #region Private constants

        private const string RecordsTable = "RecordsTable";
        private const string FacesFields =
            @"faceThumbnail, faceRectangleX, faceRectangleY, faceRectangleWidth, faceRectangleHeight, rectRoll,
			  leftEyeConfidence, leftEyeX, leftEyeY, rightEyeConfidence, rightEyeX, rightEyeY,
			  faceQuality, faceAttributes, matchId, matchScore";
        private static readonly int FaceFieldCount = GetFieldCount(FacesFields);
        private static readonly string FaceParameters = GetParametersString(FacesFields);

        private const string ObjectFields =
            @"objThumbnail,
			  car, person, bus, truck, bike, 
			  red, orange, yellow, green, blue, silver, white, black, brown, gray,
			  north, northEast, east, southEast, south, southWest, west, northWest, 
			  objRectangleX, objRectangleY, objRectangleWidth, objRectangleHeight, objDetectionConfidence,
			  vehicleMake, vehicleMakeConfidence, carModel, clothes, clothesConfidence, ageGroup, ageGroupConfidence";
        private static readonly int ObjectFieldCount = GetFieldCount(ObjectFields);
        private static readonly string ObjectParameters = GetParametersString(ObjectFields);

        private const string LicensePlateFields =
            @"lpThumbnail,
			  lpValue, lpOrigin, lpDetectionConfidence, lpOcrConfidence,
				lpRectangleX, lpRectangleY, lpRectangleWidth, lpRectangleHeight";
        private static readonly int LicensePlateFieldCount = GetFieldCount(LicensePlateFields);
        private static readonly string LicensePlateParameters = GetParametersString(LicensePlateFields);

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        #endregion

        #region Public constructor

        public RecordCollection(string conn)
        {
            _mysqlConnection = new MySqlConnection();
            _mysqlConnection.ConnectionString = conn;
            try
            {
                _mysqlConnection.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(string.Format("{0}. StackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }

        #endregion

        #region Public methods

        public void Close()
        {
            _mysqlConnection.Close();
        }

        public async Task AddAsync(Record record)
        {
            using (record)
            {
                await _semaphore.WaitAsync();
                try
                {
                    if (record.Object != null && record.License != null)
                    {
                        await WriteObjAndLpRecordToDbAsync(record);
                    }
                    else if (record.Object != null && record.Face != null)
                    {
                        await WriteObjAndFaceRecordToDbAsync(record);
                    }
                    else if (record.Object != null)
                    {
                        await WriteObjectToDbAsync(record);
                    }
                    else if (record.License != null)
                    {
                        await WriteLicenseToDbAsync(record);
                    }
                    else if (record.Face != null)
                    {
                        await WriteFaceToDbAsync(record);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public void ClearWeekOld()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"DELETE FROM {RecordsTable} WHERE timestamp <= '{DateTime.Now.Subtract(TimeSpan.FromDays(7)).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}'";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Could not clear database, reason: {ex.Message}");
                    }
                }
            }
        }

        public async Task<List<Record>> GetObjectsWithLicenseAsync(int limit, DateTime fromTime, Order order, int? fromId = null, bool withThumbnails = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DbDataReader reader = null;
            var records = new List<Record>();
            string query = $@"SELECT id, source, timestamp, image,
								{ObjectFields},
								{LicensePlateFields}
							FROM {RecordsTable}
							WHERE objThumbnail IS NOT NULL AND lpThumbnail IS NOT NULL
							AND timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' {GetFromId(fromId, order)}
							ORDER BY id {GetOrder(order)}
							LIMIT {limit}";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        int ordinal = 0;
                        int id = reader.GetInt32(ordinal++);
                        string source = reader.GetString(ordinal++);
                        DateTime timestamp = reader.GetDateTime(ordinal++);
                        NImage image = null;
                        // Uncomment to read full image, if it was saved
                        //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                        ordinal++;

                        var objectRecord = ReadObjectRecord(reader, ref ordinal, withThumbnails);
                        var licenseRecord = ReadLicenseRecord(reader, ref ordinal, withThumbnails);
                        var record = new Record(id, timestamp, source, objectRecord, licenseRecord, null) { Image = image };
                        records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not select events, reason: {ex.Message}");
                }
                finally
                {
                    reader?.Close();
                }
            }
            return records;
        } }

        public async Task<List<Record>> GetObjectRecordsAsync(int limit, DateTime fromTime, Order order, int? fromId = null, string objectClass = "any", bool withThumbnail = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DbDataReader reader = null;
                var records = new List<Record>();
                string query = $@"SELECT id, source, timestamp, image, {ObjectFields}
							FROM {RecordsTable}
							WHERE objThumbnail IS NOT NULL AND timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' 
							{GetFromId(fromId, order)}
							{GetObjectClass(objectClass)}
							ORDER BY id {GetOrder(order)}
							LIMIT {limit}";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            int ordinal = 0;
                            int id = reader.GetInt32(ordinal++);
                            string source = reader.GetString(ordinal++);
                            DateTime timestamp = reader.GetDateTime(ordinal++);
                            NImage image = null;
                            // Uncomment to read full image, if it was saved
                            //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                            ordinal++;
                            var objectRecord = ReadObjectRecord(reader, ref ordinal, withThumbnail);
                            var record = new Record(id, timestamp, source, objectRecord, null, null) { Image = image };
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not select events, reason: {ex.Message}");
                    }
                    finally
                    {
                        reader?.Close();
                    }
                }
                return records;
            }
        }

        public async Task<List<Record>> GetLicenseRecordsAsync(int limit, DateTime fromTime, Order order, int? fromId = null, bool withThumbnail = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DbDataReader reader = null;
            var records = new List<Record>();
            string query = $@"SELECT id, source, timestamp, image, {LicensePlateFields}
							FROM {RecordsTable}
							WHERE lpThumbnail IS NOT NULL AND timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}'
							{GetFromId(fromId, order)}
							ORDER BY id {GetOrder(order)}
							LIMIT {limit}";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        int ordinal = 0;
                        int id = reader.GetInt32(ordinal++);
                        string source = reader.GetString(ordinal++);
                        DateTime timestamp = reader.GetDateTime(ordinal++);
                        NImage image = null;
                        // Uncomment to read full image, if it was saved
                        //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                        ordinal++;
                        var licenseRecord = ReadLicenseRecord(reader, ref ordinal, withThumbnail);
                        var record = new Record(id, timestamp, source, null, licenseRecord, null) { Image = image };
                        records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not select events, reason: {ex.Message}");
                }
                finally
                {
                    reader?.Close();
                }
            }
            return records;
        } }

        public async Task<List<Record>> GetFaceRecordsAsync(int limit, DateTime fromTime, Order order, int? fromId = null, bool withThumbnail = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                DbDataReader reader = null;
                var records = new List<Record>();
                string query = $@"SELECT id, source, timestamp, image, {FacesFields}
							FROM {RecordsTable}
							WHERE faceThumbnail IS NOT NULL AND timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' 
							{GetFromId(fromId, order)}
							ORDER BY id {GetOrder(order)} 
							LIMIT {limit}";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            int ordinal = 0;
                            int id = reader.GetInt32(ordinal++);
                            string source = reader.GetString(ordinal++);
                            DateTime timestamp = reader.GetDateTime(ordinal++);
                            NImage image = null;
                            // Uncomment to read full image, if it was saved
                            //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                            ordinal++;
                            var faceRecord = ReadFaceRecord(reader, ref ordinal, withThumbnail);
                            var record = new Record(id, timestamp, source, null, null, faceRecord) { Image = image };
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not select events, reason: {ex.Message}");
                    }
                    finally
                    {
                        reader?.Close();
                    }
                }
                return records;
            }
        }

        public async Task<List<Record>> GetRecordsAsync(int limit, DateTime fromTime, Order order, int? fromId = null, bool withThumbnails = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var records = new List<Record>();
                DbDataReader reader = null;
                string query = $@"SELECT id, source, timestamp, image, {ObjectFields}, {LicensePlateFields}, {FacesFields}
							FROM {RecordsTable}
							WHERE timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}'
							{GetFromId(fromId, order)}
							ORDER BY id {GetOrder(order)} 
							LIMIT {limit}";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            int ordinal = 0;
                            int id = reader.GetInt32(ordinal++);
                            string source = reader.GetString(ordinal++);
                            DateTime timestamp = reader.GetDateTime(ordinal++);
                            NImage image = null;
                            // Uncomment to read full image, if it was saved
                            //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                            ordinal++;
                            ObjectRecord objectRecord = ReadObjectRecord(reader, ref ordinal, withThumbnails);
                            LicenseRecord licenseRecord = ReadLicenseRecord(reader, ref ordinal, withThumbnails);
                            FullFaceRecord faceRecord = ReadFaceRecord(reader, ref ordinal, withThumbnails);
                            var record = new Record(id, timestamp, source, objectRecord, licenseRecord, faceRecord) { Image = image };
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not select events, reason: {ex.Message}");
                    }
                    finally
                    {
                        reader?.Close();
                    }
                }
                return records;
            }
        }

        public async Task<List<Record>> GetObjectsWithFacesAsync(int limit, DateTime fromTime, Order order, int? fromId = null, bool withThumbnails = true)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var records = new List<Record>();
                DbDataReader reader = null;
                string query = $@"SELECT id, source, timestamp, image, {ObjectFields}, {FacesFields}
							FROM {RecordsTable}
							WHERE objThumbnail IS NOT NULL AND faceThumbnail IS NOT NULL AND timestamp >= '{fromTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' 
							{GetFromId(fromId, order)}
							ORDER BY id {GetOrder(order)} 
							LIMIT {limit}";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            int ordinal = 0;
                            int id = reader.GetInt32(ordinal++);
                            string source = reader.GetString(ordinal++);
                            DateTime timestamp = reader.GetDateTime(ordinal++);
                            NImage image = null;
                            // Uncomment to read full image, if it was saved
                            //image = !reader.IsDBNull(3) ? ReadNImage(reader, 3) : null;
                            ordinal++;
                            ObjectRecord objectRecord = ReadObjectRecord(reader, ref ordinal, withThumbnails);
                            FullFaceRecord faceRecord = ReadFaceRecord(reader, ref ordinal, withThumbnails);
                            var record = new Record(id, timestamp, source, objectRecord, null, faceRecord) { Image = image };
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not select events, reason: {ex.Message}");
                    }
                    finally
                    {
                        reader?.Close();
                    }
                }
                return records;
            }
        }

        #endregion

        #region Public static methods

        public static void CheckCreateDb(string fileName)
        {
            if (!File.Exists(fileName))
            {
                //MySqlConnection.CreateFile(fileName);
            }

            using (var conn = new MySqlConnection("Version=3;New=True;Compress=False;foreign keys=true;Data Source=" + fileName))
            {
                try
                {
                    conn.Open();
                    CreateRecordsTable(conn);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not create database tables, reason: {ex.Message}");
                }
            }
        }

        #endregion

        #region Private static methods

        private static void CreateRecordsTable(MySqlConnection conn)
        {
            string query =
                $@"CREATE TABLE IF NOT EXISTS {RecordsTable} (
					[id] INTEGER PRIMARY KEY,
					[image] IMAGE,
					[timestamp] TIMESTAMP,
					[source] TEXT,
					[car] REAL, [person] REAL, [bus] REAL, [truck] REAL, [bike] REAL,
					[red] REAL, [orange] REAL, [yellow] REAL, [green] REAL, [blue] REAL,
					[silver] REAL, [white] REAL, [black] REAL, [brown] REAL,[gray] REAL,
					[north] REAL, [northEast] REAL, [east] REAL,
					[southEast] REAL, [south] REAL, [southWest] REAL,
					[west] REAL, [northWest] REAL, [objThumbnail] IMAGE,
					[objRectangleX] INTEGER, [objRectangleY] INTEGER,
					[objRectangleWidth] INTEGER, [objRectangleHeight] INTEGER,
					[objDetectionConfidence] REAL,
					[vehicleMake] TEXT, [vehicleMakeConfidence] INTEGER,
					[carModel] TEXT,
					[clothes] TEXT, [clothesConfidence] TEXT,
					[ageGroup] INTEGER, [ageGroupConfidence] REAL,

					[lpThumbnail] IMAGE,
					[lpValue] TEXT, [lpOrigin] TEXT,
					[lpDetectionConfidence] REAL, [lpOcrConfidence] REAL,
					[lpRectangleX] INTEGER, [lpRectangleY] INTEGER,
					[lpRectangleWidth] INTEGER, [lpRectangleHeight] INTEGER,

					[picture] IMAGE,
					[faceThumbnail] IMAGE,
					[faceAvailable] BOOL,
					[faceRectangleX] INTEGER, [faceRectangleY] INTEGER,
					[faceRectangleWidth] INTEGER, [faceRectangleHeight] INTEGER,
					[rectRoll] FLOAT,
					[leftEyeConfidence] INTEGER,
					[leftEyeX] INTEGER, [leftEyeY] INTEGER,
					[rightEyeConfidence] INTEGER,  
					[rightEyeX] INTEGER, [rightEyeY] INTEGER,

					[faceQuality] INTEGER,
					[faceAttributes] TEXT,
					[match] TEXT,
					[matchScore] INTEGER)";

            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private methods

        private NImage ReadNImage(DbDataReader reader, int ordinal)
        {
            long fieldSize = reader.GetBytes(ordinal, 0, null, 0, 0);
            byte[] bytes = new byte[fieldSize];
            reader.GetBytes(ordinal, 0, bytes, 0, (int)fieldSize);
            return NImage.FromMemory(bytes);
        }

        private Bitmap ReadImage(DbDataReader reader, int ordinal)
        {
            using (var img = ReadNImage(reader, ordinal))
            {
                return img.ToBitmap();
            }
        }

        private static int GetFieldCount(string fieldsStrings)
        {
            return fieldsStrings.Split(new[] { "," }, StringSplitOptions.None).Length;
        }

        private static string GetParametersString(string fieldsString)
        {
            var fields = fieldsString.Split(new[] { "," }, StringSplitOptions.None);
            return fields.Select(x => "@" + x.Trim()).Aggregate((a, b) => a + ", " + b);
        }

        private static Neurotec.Drawing.Rectangle ReadRectangle(DbDataReader reader, ref int ordinal)
        {
            return new Neurotec.Drawing.Rectangle
                (
                    reader.GetInt32(ordinal++),
                    reader.GetInt32(ordinal++),
                    reader.GetInt32(ordinal++),
                    reader.GetInt32(ordinal++)
                );
        }

        private static NLFeaturePoint ReadFeaturePoint(DbDataReader reader, ref int ordinal)
        {
            return new NLFeaturePoint
            {
                Confidence = (byte)reader.GetInt32(ordinal++),
                X = (ushort)reader.GetInt32(ordinal++),
                Y = (ushort)reader.GetInt32(ordinal++)
            };
        }

        private ObjectRecord ReadObjectRecord(DbDataReader reader, ref int ordinal, bool withThumbnail)
        {
            if (reader.IsDBNull(ordinal))
            {
                ordinal += ObjectFieldCount;
                return null;
            }

            Bitmap thumb = null;
            if (withThumbnail)
                thumb = ReadImage(reader, ordinal++);
            else
                ordinal++;

            float car = reader.GetFloat(ordinal++);
            float person = reader.GetFloat(ordinal++);
            float bus = reader.GetFloat(ordinal++);
            float truck = reader.GetFloat(ordinal++);
            float bike = reader.GetFloat(ordinal++);

            float red = reader.GetFloat(ordinal++);
            float orange = reader.GetFloat(ordinal++);
            float yellow = reader.GetFloat(ordinal++);
            float green = reader.GetFloat(ordinal++);
            float blue = reader.GetFloat(ordinal++);
            float silver = reader.GetFloat(ordinal++);
            float white = reader.GetFloat(ordinal++);
            float black = reader.GetFloat(ordinal++);
            float brown = reader.GetFloat(ordinal++);
            float gray = reader.GetFloat(ordinal++);

            float north = reader.GetFloat(ordinal++);
            float northEast = reader.GetFloat(ordinal++);
            float east = reader.GetFloat(ordinal++);
            float southEast = reader.GetFloat(ordinal++);
            float south = reader.GetFloat(ordinal++);
            float southWest = reader.GetFloat(ordinal++);
            float west = reader.GetFloat(ordinal++);
            float northWest = reader.GetFloat(ordinal++);

            int objRectX = reader.GetInt32(ordinal++);
            int objRectY = reader.GetInt32(ordinal++);
            int objRectWidth = reader.GetInt32(ordinal++);
            int objRectHeight = reader.GetInt32(ordinal++);
            float objDetectionConfidence = reader.GetFloat(ordinal++);

            string vehicleMake = null;
            float vehicleMakeConfidence = 0;
            string carModel = null;

            if (!reader.IsDBNull(ordinal))
            {
                vehicleMake = reader.GetString(ordinal++);
                vehicleMakeConfidence = (float)reader.GetFloat(ordinal++);
                carModel = reader.GetString(ordinal++);
            }
            else
                ordinal = ordinal + 3;

            List<KeyValuePair<string, float>> clothesList = new List<KeyValuePair<string, float>>();

            if (!reader.IsDBNull(ordinal))
            {
                string clothes = reader.GetString(ordinal++);
                string clothesConfidence = reader.GetString(ordinal++);

                string[] clothesSplit = clothes.Split(';');
                string[] clothesConfidenceSplit = clothesConfidence.Split(';');

                for (int i = 0; i < clothesSplit.Length - 1; i++)
                {
                    if (float.TryParse(clothesConfidenceSplit[i], out var result))
                        clothesList.Add(new KeyValuePair<string, float>(clothesSplit[i], result));
                }
            }
            else
            {
                ordinal += 2;
            }

            var ageGroup = (NAgeGroup)reader.GetInt32(ordinal++);
            var ageGroupConf = reader.GetFloat(ordinal++);

            return new ObjectRecord()
            {
                CarConfidence = car,
                PersonConfidence = person,
                BusConfidence = bus,
                TruckConfidence = truck,
                BikeConfidence = bike,
                RedColorConfidence = red,
                OrangeColorConfidence = orange,
                YellowColorConfidence = yellow,
                GreenColorConfidence = green,
                BlueColorConfidence = blue,
                SilverColorConfidence = silver,
                WhiteColorConfidence = white,
                BlackColorConfidence = black,
                BrownColorConfidence = brown,
                GrayColorConfidence = gray,
                NorthConfidence = north,
                NorthEastConfidence = northEast,
                EastConfidence = east,
                SouthEastConfidence = southEast,
                SouthConfidence = south,
                SouthWestConfidence = southWest,
                WestConfidence = west,
                NorthWestConfidence = northWest,
                Thumbnail = thumb,
                DetectionConfidence = objDetectionConfidence,
                BoundingRect = new Rectangle(objRectX, objRectY, objRectWidth, objRectHeight),
                VehicleMake = vehicleMake,
                VehicleMakeConfidence = vehicleMakeConfidence,
                CarModel = carModel,
                Clothes = clothesList,
                AgeGroup = ageGroup,
                AgeGroupConfidence = ageGroupConf
            };
        }

        private LicenseRecord ReadLicenseRecord(DbDataReader reader, ref int ordinal, bool withThumbnail)
        {
            if (reader.IsDBNull(ordinal))
            {
                ordinal += LicensePlateFieldCount;
                return null;
            }

            Bitmap thumb = null;
            if (withThumbnail)
                thumb = ReadImage(reader, ordinal++);
            else
                ordinal++;

            string value = reader.GetString(ordinal++);
            string origin = reader.GetString(ordinal++);
            float licDetectionConfidence = reader.GetFloat(ordinal++);
            float ocrConfidence = reader.GetFloat(ordinal++);
            int licRectX = reader.GetInt32(ordinal++);
            int licRectY = reader.GetInt32(ordinal++);
            int licRectWidth = reader.GetInt32(ordinal++);
            int licRectHeight = reader.GetInt32(ordinal++);

            return new LicenseRecord()
            {
                Value = value,
                Origin = origin,
                Thumbnail = thumb,
                DetectionConfidence = licDetectionConfidence,
                OcrConfidence = ocrConfidence,
                BoundingRect = new Rectangle(licRectX, licRectY, licRectWidth, licRectHeight)
            };
        }

        private FullFaceRecord ReadFaceRecord(DbDataReader reader, ref int ordinal, bool withThumbnail)
        {
            if (reader.IsDBNull(ordinal))
            {
                ordinal += FaceFieldCount;
                return null;
            }

            Bitmap thumb = null;
            if (withThumbnail)
                thumb = ReadImage(reader, ordinal++);
            else
                ordinal++;

            var attributes = new NLAttributes();
            attributes.BoundingRect = ReadRectangle(reader, ref ordinal);
            attributes.Roll = reader.GetFloat(ordinal++);
            attributes.LeftEyeCenter = ReadFeaturePoint(reader, ref ordinal);
            attributes.RightEyeCenter = ReadFeaturePoint(reader, ref ordinal);
            attributes.Quality = (byte)reader.GetInt32(ordinal++);
            var attributesString = reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            if (attributesString != null)
            {
                foreach (var pair in FullFaceRecord.ParseAttributesString(attributesString))
                {
                    attributes.SetAttributeValue(pair.Key, pair.Value);
                }
            }
            ordinal++;

            NsedMatchResult nsedMatchResult = null;
            if (!reader.IsDBNull(ordinal))
                nsedMatchResult = new NsedMatchResult(reader.GetString(ordinal++), (byte)reader.GetInt32(ordinal++));
            else
                ordinal = ordinal + 2;

            var faceRecord = new FullFaceRecord(attributes, nsedMatchResult)
            {
                Thumbnail = thumb
            };
            return faceRecord;
        }

        private string GetObjectClass(string objectClass)
        {
            if (string.IsNullOrEmpty(objectClass) || objectClass == "any") return "";
            return $" AND {objectClass} >= 0.50 ";
        }

        private string GetFromId(int? fromId, Order order)
        {
            if (fromId == null) return "";

            if (order == Order.Descending)
                return $"AND id < {fromId} ";
            else
                return $"AND id > {fromId} ";
        }

        private string GetOrder(Order order)
        {
            return order == Order.Descending ? "DESC" : "ASC";
        }

        private async Task WriteObjectToDbAsync(Record record)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var image = record.Image?.Save(NImageFormat.Jpeg))
                using (var thumb = NImage.FromBitmap(record.Object.Thumbnail).Save(NImageFormat.Jpeg))
                {
                    string query =
                        $@"INSERT INTO {RecordsTable} (image, timestamp, source, {ObjectFields})
						VALUES (@image, @timestamp, @source, {ObjectParameters})";

                    using (var insert = new MySqlCommand(query, connection))
                    {
                        AddCommonFields(insert, record, image);
                        AddObjectFields(insert, record.Object, thumb);
                        await insert.ExecuteNonQueryAsync();
                    }
                }
            }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        private async Task WriteLicenseToDbAsync(Record record)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var image = record.Image?.Save(NImageFormat.Jpeg))
                using (var thumb = NImage.FromBitmap(record.License.Thumbnail).Save(NImageFormat.Jpeg))
                {
                    string query =
                        $@"INSERT INTO {RecordsTable} (image, timestamp, source, {LicensePlateFields})
								VALUES (@image, @timestamp, @source, {LicensePlateParameters})";

                    using (var insert = new MySqlCommand(query, connection))
                    {
                        AddCommonFields(insert, record, image);
                        AddLicenseFields(insert, record.License, thumb);
                        await insert.ExecuteNonQueryAsync();
                    }
                }
            }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        private async Task WriteFaceToDbAsync(Record record)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var image = record.Image?.Save(NImageFormat.Jpeg))
                    using (var thumb = NImage.FromBitmap(record.Face.Thumbnail).Save(NImageFormat.Jpeg))
                    {
                        string query =
                            $@"INSERT INTO {RecordsTable} (image, timestamp, source, {FacesFields})
								VALUES (@image, @timestamp, @source, {FaceParameters})";

                        using (var insert = new MySqlCommand(query, connection))
                        {
                            AddCommonFields(insert, record, image);
                            AddFaceFields(insert, record.Face, thumb);
                            try
                            {
                                await insert.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        private async Task WriteObjAndLpRecordToDbAsync(Record record)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var image = record.Image?.Save(NImageFormat.Jpeg))
                    using (var lpThumb = NImage.FromBitmap(record.License.Thumbnail).Save(NImageFormat.Jpeg))
                    using (var objThumb = NImage.FromBitmap(record.Object.Thumbnail).Save(NImageFormat.Jpeg))
                    {
                        string query =
                            $@"INSERT INTO {RecordsTable} (image, timestamp, source, {ObjectFields}, {LicensePlateFields})

						VALUES (@image, @timestamp, @source, {ObjectParameters}, {LicensePlateParameters})";

                        using (var insert = new MySqlCommand(query, connection))
                        {
                            AddCommonFields(insert, record, image);
                            AddObjectFields(insert, record.Object, objThumb);
                            AddLicenseFields(insert, record.License, lpThumb);
                            await insert.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        private async Task WriteObjAndFaceRecordToDbAsync(Record record)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var image = record.Image?.Save(NImageFormat.Jpeg))
                    using (var objThumb = NImage.FromBitmap(record.Object.Thumbnail).Save(NImageFormat.Jpeg))
                    using (var faceThumb = NImage.FromBitmap(record.Face.Thumbnail).Save(NImageFormat.Jpeg))
                    {
                        string query =
                            $@"INSERT INTO {RecordsTable} (image, timestamp, source, {ObjectFields}, {FacesFields})
						VALUES (@image, @timestamp, @source, {ObjectParameters}, {FaceParameters})";

                        using (var insert = new MySqlCommand(query, connection))
                        {
                            AddCommonFields(insert, record, image);
                            AddObjectFields(insert, record.Object, objThumb);
                            AddFaceFields(insert, record.Face, faceThumb);
                            try
                            {
          
                                await insert.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {
                            }

                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        private void AddObjectFields(MySqlCommand insert, ObjectRecord record, NBuffer thumb)
        {
            insert.Parameters.AddWithValue("@objThumbnail", thumb.ToArray());
            insert.Parameters.AddWithValue("@car", record.CarConfidence);
            insert.Parameters.AddWithValue("@person", record.PersonConfidence);
            insert.Parameters.AddWithValue("@bus", record.BusConfidence);
            insert.Parameters.AddWithValue("@truck", record.TruckConfidence);
            insert.Parameters.AddWithValue("@bike", record.BikeConfidence);

            insert.Parameters.AddWithValue("@red", record.RedColorConfidence);
            insert.Parameters.AddWithValue("@orange", record.OrangeColorConfidence);
            insert.Parameters.AddWithValue("@yellow", record.YellowColorConfidence);
            insert.Parameters.AddWithValue("@green", record.GreenColorConfidence);
            insert.Parameters.AddWithValue("@blue", record.BlueColorConfidence);
            insert.Parameters.AddWithValue("@silver", record.SilverColorConfidence);
            insert.Parameters.AddWithValue("@white", record.WhiteColorConfidence);
            insert.Parameters.AddWithValue("@black", record.BlackColorConfidence);
            insert.Parameters.AddWithValue("@brown", record.BrownColorConfidence);
            insert.Parameters.AddWithValue("@gray", record.GrayColorConfidence);

            insert.Parameters.AddWithValue("@north", record.NorthConfidence);
            insert.Parameters.AddWithValue("@northEast", record.NorthEastConfidence);
            insert.Parameters.AddWithValue("@east", record.EastConfidence);
            insert.Parameters.AddWithValue("@southEast", record.SouthEastConfidence);
            insert.Parameters.AddWithValue("@south", record.SouthConfidence);
            insert.Parameters.AddWithValue("@southWest", record.SouthWestConfidence);
            insert.Parameters.AddWithValue("@west", record.WestConfidence);
            insert.Parameters.AddWithValue("@northWest", record.NorthWestConfidence);

            insert.Parameters.AddWithValue("@objRectangleX", record.BoundingRect.X);
            insert.Parameters.AddWithValue("@objRectangleY", record.BoundingRect.Y);
            insert.Parameters.AddWithValue("@objRectangleWidth", record.BoundingRect.Width);
            insert.Parameters.AddWithValue("@objRectangleHeight", record.BoundingRect.Height);
            insert.Parameters.AddWithValue("@objDetectionConfidence", record.DetectionConfidence);

            // Check if the variable contains NaN and replace it with 0
            if (float.IsNaN(record.VehicleMakeConfidence))
            {
                record.VehicleMakeConfidence = 0;
            }

            insert.Parameters.AddWithValue("@vehicleMake", record.VehicleMake);
            insert.Parameters.AddWithValue("@vehicleMakeConfidence", record.VehicleMakeConfidence);
            insert.Parameters.AddWithValue("@carModel", record.CarModel);

            string clothesString = null;
            string confidencesString = null;
            if (record.Clothes != null)
            {
                foreach (var v in record.Clothes)
                {
                    clothesString += v.Key + ";";
                    confidencesString += v.Value.ToString() + ";";
                }
            }
            insert.Parameters.AddWithValue("@clothes", clothesString);
            insert.Parameters.AddWithValue("@clothesConfidence", confidencesString);

            insert.Parameters.AddWithValue("@ageGroup", record.AgeGroup);
            insert.Parameters.AddWithValue("@ageGroupConfidence", record.AgeGroupConfidence);
        }

        private void AddLicenseFields(MySqlCommand insert, LicenseRecord record, NBuffer thumb)
        {
            insert.Parameters.AddWithValue("@lpThumbnail", thumb.ToArray());
            insert.Parameters.AddWithValue("@lpValue", record.Value);
            insert.Parameters.AddWithValue("@lpOrigin", record.Origin);

            insert.Parameters.AddWithValue("@lpRectangleX", record.BoundingRect.X);
            insert.Parameters.AddWithValue("@lpRectangleY", record.BoundingRect.Y);
            insert.Parameters.AddWithValue("@lpRectangleWidth", record.BoundingRect.Width);
            insert.Parameters.AddWithValue("@lpRectangleHeight", record.BoundingRect.Height);

            insert.Parameters.AddWithValue("@lpDetectionConfidence", record.DetectionConfidence);
            insert.Parameters.AddWithValue("@lpOcrConfidence", record.OcrConfidence);
        }

        private void AddFaceFields(MySqlCommand insert, FullFaceRecord record, NBuffer thumb)
        {
            insert.Parameters.AddWithValue("@faceThumbnail", thumb.ToArray());
            insert.Parameters.AddWithValue("@faceRectangleX", record.BoundingRect.X);
            insert.Parameters.AddWithValue("@faceRectangleY", record.BoundingRect.Y);
            insert.Parameters.AddWithValue("@faceRectangleWidth", record.BoundingRect.Width);
            insert.Parameters.AddWithValue("@faceRectangleHeight", record.BoundingRect.Height);
            insert.Parameters.AddWithValue("@rectRoll", record.Roll);
            insert.Parameters.AddWithValue("@leftEyeConfidence", record.LeftEye.Confidence);
            insert.Parameters.AddWithValue("@leftEyeX", record.LeftEye.X);
            insert.Parameters.AddWithValue("@leftEyeY", record.LeftEye.Y);
            insert.Parameters.AddWithValue("@rightEyeConfidence", record.RightEye.Confidence);
            insert.Parameters.AddWithValue("@rightEyeX", record.RightEye.X);
            insert.Parameters.AddWithValue("@rightEyeY", record.RightEye.Y);

            insert.Parameters.AddWithValue("@faceQuality", record.Quality);

            insert.Parameters.AddWithValue("@faceAttributes", record.GetAttributesString());

            insert.Parameters.AddWithValue("@matchId", record.Match?.Id);

            insert.Parameters.AddWithValue("@matchScore", record.Match?.Score);

        }

        private void AddCommonFields(MySqlCommand insert, Record record, NBuffer image)
        {
            insert.Parameters.AddWithValue("@image", image?.ToArray());
            insert.Parameters.AddWithValue("@timestamp", record.Timestamp);
            insert.Parameters.AddWithValue("@source", record.Source);
        }

        #endregion
    }
}
