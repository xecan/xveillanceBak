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
    public class Records
    {
        public Records(int id, byte[] features, DateTime enrollTime,string source,string faceQuality,string faceId,string matchScore)
        {
            Id = id;
            Features = features;
            EnrollTime = enrollTime;
            Source = source;
            FaceQuality = FaceQuality;
            FaceId = faceId;
            MatchScore = matchScore;
        }

        public int Id { get; set; }
        public byte[] Features { get; set; }
        public DateTime EnrollTime { get; set; }
        public string Source { get; set; }
        public string FaceQuality { get; set; }
        public string FaceId { get; set; }
        public string MatchScore { get; set; }

    }
    public class AllVisitorsCollection : IEnumerable<Records>
    {
        #region Private constants

        private const string TableName = "recordstable";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;
        private readonly Dictionary<string, Records> _dictionary = new Dictionary<string, Records>();

        #endregion

        #region Public constructor

        public AllVisitorsCollection()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand(
                    string.Format("SELECT `id`, `faceThumbnail`,`timestamp`,`source`,`faceQuality`,`faceAttributes`,`matchId`, `matchScore` from {0} {1} ", TableName,  " where matchId != '' "), connection);
                MySqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    long featuresSize = reader.GetBytes(1, 0, null, 0, 0);
                    byte[] features = new byte[featuresSize];
                    reader.GetBytes(1, 0, features, 0, (int)featuresSize);
                    var enrollTime = reader.GetDateTime(2);
                    string source = reader.GetString(3);
                    string faceQuality = reader.GetString(4);
                    string faceId = reader.GetString(6);
                    string matchScore = reader.GetString(7);


                    _dictionary[id.ToString()] = new Records(id, features, enrollTime, source,faceQuality, faceId, matchScore);
                }
                reader.Close();
            }
        }


        #endregion
        #region IEnumerable

        public IEnumerator<Records> GetEnumerator()
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
