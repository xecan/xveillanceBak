using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Neurotec.Samples.Code
{
    public class CellPhoneRecord
    {
        public CellPhoneRecord(string value, string owner)
        {
            Value = value;
            Owner = owner;
        }

        public string Value { get; set; }
        public string Owner { get; set; }
    }

    public class CellPhoneCollection : IEnumerable<CellPhoneRecord>
    {
        #region Private constants

        private const string TableName = "CellPhoneWatchList";

        #endregion

        #region Private fields

        //private readonly SQLiteConnection _mysqlConnection;
        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, CellPhoneRecord> _dictionary = new Dictionary<string, CellPhoneRecord>();

        #endregion

        #region Public constructor

        public CellPhoneCollection(string conn)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand($"SELECT * FROM {TableName} where storeAcctNum = '{SessionManager.StoreAccNum}';", connection);
                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetString(0);
                    var owner = reader.GetString(1);
                    _dictionary[id] = new CellPhoneRecord(id, owner);
                }
                reader.Close();
            }
        }

        #endregion

        #region Public static methods

        public static void CheckCreateDB(string fileName)
        {
            if (!File.Exists(fileName))
            {
                SQLiteConnection.CreateFile(fileName);
                using (var conn = new SQLiteConnection("Version=3;New=True;Compress=False;Data Source=" + fileName))
                {
                    try
                    {
                        conn.Open();
                        CreateTable(conn);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not create database tables, reason: {ex.Message}");
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public void Close()
        {
            _mysqlConnection.Close();
        }

        public bool Add(string value, string owner)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            var upper = value.Trim().ToUpperInvariant();
            if (!_dictionary.ContainsKey(upper))
            {
                WriteToDb(upper, owner);
                _dictionary[upper] = new CellPhoneRecord(upper, owner);
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
                using (var deleteCommand = new MySqlCommand($"DELETE FROM {TableName} WHERE id=@id", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@id", value);
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

        public CellPhoneRecord this[string value]
        {
            get
            {
                return _dictionary.TryGetValue(value, out var result) ? result : null;
            }
        }

        #endregion

        #region Private methods

        private void WriteToDb(string value, string owner)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"INSERT INTO {TableName} (id, owner,storeAcctNum) VALUES (@id, @owner,@storeAcctNum);";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.Parameters.AddWithValue("@id", value);
                        insert.Parameters.AddWithValue("@owner", owner);
                        insert.Parameters.AddWithValue("@storeAcctNum", SessionManager.StoreAccNum);

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

        #region Private static methods

        private static void CreateTable(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand($"CREATE TABLE {TableName} ([id] TEXT PRIMARY KEY, [owner] TEXT)", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region IEnumerable

        public IEnumerator<CellPhoneRecord> GetEnumerator()
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
