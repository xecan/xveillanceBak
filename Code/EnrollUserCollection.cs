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
    public class EnrollUserRecord
    {
        public EnrollUserRecord(string username, string pwd, string phone)
        {
            Username = username;
            Password = pwd;
            Cell = phone;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Cell { get; set; }

    }
    public class EnrollUserCollection : IEnumerable<EnrollUserRecord>
    {
        #region Private constants

        private const string TableName = "xv_user";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, EnrollUserRecord> _dictionary = new Dictionary<string, EnrollUserRecord>();

        #endregion
        public EnrollUserCollection()
        {
            var select = new MySqlCommand();
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                select = new MySqlCommand($"SELECT * FROM {TableName};", connection);

                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var username = reader.GetString(1);
                    var pasword = reader.GetString(2);
                    var cell = reader.GetString(6);

                    _dictionary[username] = new EnrollUserRecord(username, pasword, cell);
                }
                reader.Close();
            }
        }
    


    #region Public methods

    public void Close()
    {
        _mysqlConnection.Close();
    }

    public bool Add(string username, string password, string cell, string address)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        var upper = username.Trim().ToUpperInvariant();
        if (!_dictionary.ContainsKey(upper))
        {
            WriteToDb(username, password, cell, address);

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

    public EnrollUserRecord this[string value]
    {
        get
        {
            return _dictionary.TryGetValue(value, out var result) ? result : null;
        }
    }

    #endregion

    #region Private methods

    private void WriteToDb(string username, string password, string cell, string address)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string cmdstr = $"INSERT INTO {TableName} (user_name, user_password,cell,role,Address) VALUES (@username, @password,@cell,@role,@address);";
                using (var insert = new MySqlCommand(cmdstr, connection))
                {
                    insert.Parameters.AddWithValue("@username", username);
                    insert.Parameters.AddWithValue("@password", password);
                    insert.Parameters.AddWithValue("@cell", cell);
                    insert.Parameters.AddWithValue("@role", "siteAdmin");
                    insert.Parameters.AddWithValue("@address", address);


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

    public IEnumerator<EnrollUserRecord> GetEnumerator()
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
