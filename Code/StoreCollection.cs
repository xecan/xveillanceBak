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
    public class StoreRecord
    {
        public StoreRecord(string store, string acctNum, string phone, string cameraUrl, bool status, DateTime enrollTime, string compAccNum, string address,bool isxveillance)
        {
            name = store;
            accNum = acctNum;
            cell = phone;
            CameraUrl = cameraUrl;
            Status = status;
            EnrollDate = enrollTime;
            CompanyAccNum = compAccNum;
            Address = address; IsXveillance = isxveillance;
        }

        public string name { get; set; }
        public string accNum { get; set; }
        public string cell { get; set; }
        public string CameraUrl { get; set; }
        public bool Status { get; set; }
        public DateTime EnrollDate { get; set; }
        public string CompanyAccNum { get; set; }
        public string Address { get; set; } public bool IsXveillance { get; set; }


    }
    public class StoreCollection : IEnumerable<StoreRecord>
    {
        #region Private constants

        private const string TableName = "xv_store";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, StoreRecord> _dictionary = new Dictionary<string, StoreRecord>();

        #endregion

        #region Public constructor

        public StoreCollection()
        {
            bool isActive = false;
            var select = new MySqlCommand();
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                select = new MySqlCommand($"SELECT * FROM {TableName};", connection);
                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var enrollDate = DateTime.Now; bool isXveilance = false;
                    var storeName = reader.GetString(1);
                    var accNum = reader.GetString(0);
                    var cell = reader.GetString(3);
                    var cameraUrl = reader.GetString(2);
                    if (!reader.IsDBNull(6))
                    {
                        isActive = (reader.GetBoolean(6));
                    }
                    else
                    {
                        isActive = true;
                    }
                    if (!reader.IsDBNull(7))
                    {
                        enrollDate = reader.GetDateTime(7);
                    }
                    else
                    {
                        enrollDate = DateTime.Now;
                    }
                    if (!reader.IsDBNull(8))
                    {
                        isXveilance = reader.GetBoolean(8);
                    }
                    else
                    {
                        isXveilance = false;
                    }
                    var companyAccNum = (!reader.IsDBNull(4) ? reader.GetString(4) : "");
                    var address = (!reader.IsDBNull(5) ? reader.GetString(5) : "");

                    var status =
                    _dictionary[accNum] = new StoreRecord(storeName, accNum, cell, cameraUrl, isActive, enrollDate, companyAccNum, address, isXveilance);
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

        public bool Add(string storeName, string acctNum, string cell, string cameraUrl, string address, string compAccNum)
        {
            if (string.IsNullOrEmpty(storeName))
                throw new ArgumentNullException(nameof(storeName));

            var upper = storeName.Trim().ToUpperInvariant();
            if (!_dictionary.ContainsKey(upper))
            {
                WriteToDb(storeName, acctNum, cell, cameraUrl, address, compAccNum);
                _dictionary[upper] = new StoreRecord(storeName, acctNum, cell, "", true, DateTime.Now, "", "",false);
                return true;
            }
            return false;
        }
        public bool Update(string acctNum, bool status)
        {
            UpdateToDb(acctNum, status);
            return true;
        }
        public bool UpdateStore(string storeName, string acctNum, string cell, string cameraUrl, string address, string compAccNum)
        {
            UpdateStoreToDb(storeName, acctNum, cell, cameraUrl, address, compAccNum);
            return true;
        }
        public bool UpdateCameraUrl(string acctNum, string cameraUrl)
        {
            UpdateCameraUrlToDb(acctNum, cameraUrl);
            return true;
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
                using (var deleteCommand = new MySqlCommand($"DELETE FROM {TableName} WHERE storeAcctNum=@storeAcctNum", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@storeAcctNum", value);
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

        public StoreRecord this[string value]
        {
            get
            {
                return _dictionary.TryGetValue(value, out var result) ? result : null;
            }
        }

        #endregion

        #region Private methods

        private void WriteToDb(string storeName, string accNum, string cell, string cameraUrl, string address, string compAcctNum)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"INSERT INTO {TableName} (storeAcctNum, storeName,cell,companyAcctNum,cameraURLIDs,Address,created_date,IsActive) VALUES (@storeAcctNum, @storeName,@cell,@companyAcctNum,@cameraURLIDs,@address,@created_date,@isActive);";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.Parameters.AddWithValue("@storeAcctNum", accNum);
                        insert.Parameters.AddWithValue("@storeName", storeName);
                        insert.Parameters.AddWithValue("@cell", cell);
                        if (!string.IsNullOrEmpty(compAcctNum))
                        {
                            insert.Parameters.AddWithValue("@companyAcctNum", compAcctNum);
                        }
                        else
                        {
                            insert.Parameters.AddWithValue("@companyAcctNum", SessionManager.CompanyAccNum);
                        }
                        insert.Parameters.AddWithValue("@cameraURLIDs", cameraUrl);
                        insert.Parameters.AddWithValue("@address", address);
                        insert.Parameters.AddWithValue("@isActive", true);
                        insert.Parameters.AddWithValue("@created_date", DateTime.Now);



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
        private void UpdateToDb(string acctNum, bool status)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"UPDATE {TableName} set IsActive={status} where storeAcctNum ='{acctNum}';";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.ExecuteNonQuery();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to update record into DB:\n{0}", ex.Message));
            }
        }

        private void UpdateStoreToDb(string storeName, string accNum, string cell, string cameraUrl, string address, string compAcctNum)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"UPDATE {TableName} set storeName='{storeName}',  cameraURlIDs='{cameraUrl}', cell='{cell}',companyAcctNum='{compAcctNum}',Address='{address}'   where storeAcctNum ='{accNum}';";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.ExecuteNonQuery();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to update record into DB:\n{0}", ex.Message));
            }
        }

        private void UpdateCameraUrlToDb(string acctNum, string cameraUrl)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"UPDATE {TableName} set cameraURLIDs='{cameraUrl}' where storeAcctNum ='{acctNum}';";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.ExecuteNonQuery();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Failed to insert record into DB:\n{0}", ex.Message));
            }
        }


        #region IEnumerable

        public IEnumerator<StoreRecord> GetEnumerator()
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
