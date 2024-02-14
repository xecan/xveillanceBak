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
    public class CompanyRecord
    {
        public CompanyRecord(string store, string acctNum, string phone, bool status,DateTime enrolltime,string address)
        {
            name = store;
            accNum = acctNum;
            cell = phone;
            Status = status;
            EnrollTime = enrolltime;
            Address = address;
        }

        public string name { get; set; }
        public string accNum { get; set; }
        public string cell { get; set; }
        public bool Status { get; set; }
        public DateTime EnrollTime { get; set; }
        public string Address { get; set; }


    }
    public class CompanyCollection : IEnumerable<CompanyRecord>
    {
        #region Private constants

        private const string TableName = "xv_company";

        #endregion

        #region Private fields

        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, CompanyRecord> _dictionary = new Dictionary<string, CompanyRecord>();

        #endregion

        #region Public constructor

        public CompanyCollection()
        {
            bool isActive = false;
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand($"SELECT * FROM {TableName};", connection);
                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var enrollDate = DateTime.Now;

                    var storeName = reader.GetString(1);
                    var accNum = reader.GetString(0);
                    var cell = reader.GetString(3);
                    if (!reader.IsDBNull(5))
                    {
                        isActive = (reader.GetBoolean(5));
                    }
                    else
                    {
                        isActive = true;
                    }
                    if (!reader.IsDBNull(6))
                    {
                        enrollDate = reader.GetDateTime(6);
                    }
                    else
                    {
                        enrollDate = DateTime.Now;
                    }
                    var address = (!reader.IsDBNull(4) ? reader.GetString(4) : "");

                    _dictionary[accNum] = new CompanyRecord(storeName, accNum, cell, isActive,enrollDate,address);
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

        public bool Add(string storeName, string acctNum, string cell, string address)
        {
            if (string.IsNullOrEmpty(storeName))
                throw new ArgumentNullException(nameof(storeName));

            var upper = storeName.Trim().ToUpperInvariant();
            if (!_dictionary.ContainsKey(upper))
            {
                WriteToDb(storeName, acctNum, cell, address);
                _dictionary[upper] = new CompanyRecord(storeName, acctNum, cell, true,DateTime.Now,"");
                return true;
            }
            return false;
        }

        public bool Update(string acctNum, bool status)
        {
            UpdateToDb(acctNum,status);
            return true;
        }
        public bool UpdateCompany(string companyName, string acctNum, string cell, string address)
        {
            UpdateCompanyToDb(companyName, acctNum, cell, address);
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
                using (var deleteCommand = new MySqlCommand($"DELETE FROM {TableName} WHERE companyAcctNum=@companyAcctNum", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@companyAcctNum", value);
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

        public CompanyRecord this[string value]
        {
            get
            {
                return _dictionary.TryGetValue(value, out var result) ? result : null;
            }
        }

        #endregion

        #region Private methods

        private void WriteToDb(string companyName, string accNum, string cell, string address)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"INSERT INTO {TableName} (companyAcctNum, companyName,cell,Address,IsActive,created_date) VALUES (@companyAcctNum, @companyName,@cell,@address,@isactive, @createdDate);";
                    using (var insert = new MySqlCommand(cmdstr, connection))
                    {
                        insert.Parameters.AddWithValue("@companyAcctNum", accNum);
                        insert.Parameters.AddWithValue("@companyName", companyName);
                        insert.Parameters.AddWithValue("@cell", cell);
                        insert.Parameters.AddWithValue("@address", address);
                        insert.Parameters.AddWithValue("@isactive", true);
                        insert.Parameters.AddWithValue("@createdDate", DateTime.Now);

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
        private void UpdateToDb(string acctNum , bool status)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"UPDATE {TableName} set IsActive={status} where companyAcctNum ='{acctNum}';";
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
        private void UpdateCompanyToDb(string companyName, string accNum, string cell, string address)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string cmdstr = $"UPDATE {TableName} set companyName='{companyName}',  cell='{cell}',Address='{address}'   where companyAcctNum ='{accNum}';";
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


        #region IEnumerable

        public IEnumerator<CompanyRecord> GetEnumerator()
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
