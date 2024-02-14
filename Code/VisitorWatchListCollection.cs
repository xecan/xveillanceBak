using MySql.Data.MySqlClient;
using Neurotec.Biometrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurotec.Samples.Code
{
   public class VisitorWatchListCollection : IEnumerable<FaceRecord>
    {
        #region Private constants

        private const string TableName = "WatchList";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;
        private readonly Dictionary<string, FaceRecord> _dictionary = new Dictionary<string, FaceRecord>();

        #endregion

        #region Public constructor

        public VisitorWatchListCollection()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand(
                    string.Format("SELECT id, features, faceId, enrollTime, " +
                        "faceRectangleX, faceRectangleY, faceRectangleWidth, faceRectangleHeight, rectRoll, " +
                        "leftEyeConfidence, leftEyeX, leftEyeY, rightEyeConfidence, rightEyeX, rightEyeY FROM {0} ", TableName), connection);
                MySqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    long featuresSize = reader.GetBytes(1, 0, null, 0, 0);
                    byte[] features = new byte[featuresSize];
                    reader.GetBytes(1, 0, features, 0, (int)featuresSize);
                    string faceId = reader.GetString(2);
                    var enrollTime = reader.GetDateTime(3);
                    var attributes = new NLAttributes();
                    attributes.BoundingRect = new Neurotec.Drawing.Rectangle(reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7));
                    attributes.Roll = reader.GetFloat(8);
                    var leftEye = new NLFeaturePoint();
                    leftEye.Confidence = (byte)reader.GetInt32(9);
                    leftEye.X = (ushort)reader.GetInt32(10);
                    leftEye.Y = (ushort)reader.GetInt32(11);
                    attributes.LeftEyeCenter = leftEye;
                    var rightEye = new NLFeaturePoint();
                    rightEye.Confidence = (byte)reader.GetInt32(12);
                    rightEye.X = (ushort)reader.GetInt32(13);
                    rightEye.Y = (ushort)reader.GetInt32(14);
                    attributes.RightEyeCenter = rightEye;

                    _dictionary[faceId] = new FaceRecord(id, features, faceId, enrollTime, attributes);
                }
                reader.Close();
            }
        }

      
        #endregion
        #region IEnumerable

        public IEnumerator<FaceRecord> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        #endregion
    }
    
}
