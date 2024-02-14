using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neurotec.Samples.Code
{
    public class UserRecord
    {
        public UserRecord(string Username, string pwd,string userid)
        {
            username = Username;
            password = pwd;
          
            UserId = userid;
        }

        public string username { get; set; }
        public string password { get; set; }
        public string cell { get; set; }
        public string UserId { get; set; }

    }
    public class UserCollection : IEnumerable<UserRecord>
    {
        #region Private constants

        private const string TableName = "xv_user";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, UserRecord> _dictionary = new Dictionary<string, UserRecord>();

        #endregion

        #region Public constructor

        public UserCollection()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand($"SELECT * FROM {TableName};", connection);
                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var username = reader.GetString(1);
                    var userid = reader.GetString(0);
                    var password = reader.GetString(2);
                   
                    _dictionary[username] = new UserRecord(username, password,userid);
                }
                reader.Close();
            }
        }

        #endregion

        #region Public methods

        public void Close()
        {
            _mysqlConnection.Close();
        }

        public bool Add(string username, string password, string cell,string acctNum,string role,string address)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            var upper = username.Trim().ToUpperInvariant();
            if (!_dictionary.ContainsKey(upper))
            {
                WriteToDb(username, password, cell,acctNum,role,address);
                _dictionary[upper] = new UserRecord(username, password, "");
                return true;
            }
            return false;
        }

        public bool Contains(string value)
        {
            return _dictionary.ContainsKey(value?.Trim().ToUpperInvariant() ?? string.Empty);
        }

        public void Delete(string value)
        {
            value = value?.Trim().ToUpperInvariant() ?? string.Empty;
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var deleteCommand = new MySqlCommand($"DELETE FROM {TableName} WHERE user_name=@username", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@username", value);
                    deleteCommand.ExecuteNonQuery();
                    _dictionary.Remove(value);
                }
            }
        }

        public int GetCount()
        {
            return _dictionary.Count;
        }

        public void Clear()
        {
            _dictionary.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var deleteAll = new MySqlCommand($"DELETE FROM {TableName}", connection))
                {
                    deleteAll.ExecuteNonQuery();
                }
            }
        }

        public UserRecord this[string value]
        {
            get
            {
                return _dictionary.TryGetValue(value, out var result) ? result : null;
            }
        }

        #endregion

        #region Private methods

        private void WriteToDb(string username, string password, string cell,string acctNum,string role,string address)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"INSERT INTO {TableName} (user_name,user_password,role,companyAcctNum,storeAcctNum,cell,Address) VALUES (@user_name, @user_password,@role,@companyAcctNum,@storeAcctNum,@cell,@address);";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.Parameters.AddWithValue("@user_name", username);
                        insert.Parameters.AddWithValue("@user_password", password);
                        insert.Parameters.AddWithValue("@role", role);
                        insert.Parameters.AddWithValue("@cell", cell);
                        insert.Parameters.AddWithValue("@address", address);

                        if (role.ToLower() == "companyadminuser")
                        {
                            insert.Parameters.AddWithValue("@companyAcctNum", acctNum);
                            insert.Parameters.AddWithValue("@storeAcctNum", "");

                        }
                        else if(role.ToLower() == "storeuser")
                        {
                            insert.Parameters.AddWithValue("@storeAcctNum", acctNum);
                            insert.Parameters.AddWithValue("@companyAcctNum", "");
                        }
                        insert.ExecuteNonQuery();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }

        #endregion



        #region IEnumerable

        public IEnumerator<UserRecord> GetEnumerator()
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
