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
    public class LoginRecord
    {
        public LoginRecord(string name, string pwd,string phone,string Role,string Storename,string companyAccNum,string compName,string storeAccountNum)
        {
            username = name;
            password = pwd;
            cell = phone;
            role = Role;
            storeName = Storename;
            companyAcctNum = companyAccNum;
            companyName = compName;
            storeAcctNum = storeAccountNum;
        }

        public string username { get; set; }
        public string password { get; set; }
        public string cell { get; set; }
        public string role { get; set; }
        public string storeName { get; set; }
        public string companyName { get; set; }
        public string companyAcctNum { get; set; }
        public string storeAcctNum { get; set; }

    }
    public class LoginCollection : IEnumerable<LoginRecord>
    {
        #region Private constants

        private const string TableName = "xv_user";

        #endregion

        #region Private fields

        //private readonly SQLiteConnection _mysqlConnection;
        private readonly MySqlConnection _mysqlConnection;

        private readonly Dictionary<string, LoginRecord> _dictionary = new Dictionary<string, LoginRecord>();

        #endregion
        #region Public constructor

        public LoginCollection()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var select = new MySqlCommand($"SELECT  u.user_name,u.user_password,u.cell,u.role,u.companyAcctNum,u.storeAcctNum,s.storeName,c.companyName FROM xv_user as u left join xv_store as s on u.storeAcctNum = s.storeAcctNum left join xv_company as c on u.companyAcctNum = c.companyAcctNum where s.IsActive=1 or c.IsActive =1 or role ='siteAdmin';", connection);
                var reader = select.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var pwd = reader.GetString(1);
                    var cell = reader.GetString(2);
                    var role = reader.GetString(3);
                    var storeName =  (!reader.IsDBNull(6)? reader.GetString(6):"");
                    var companyAccnum= (!reader.IsDBNull(4) ? reader.GetString(4) : "");
                    var companyName = (!reader.IsDBNull(7) ? reader.GetString(7) : "");
                    var storeAcctNum = (!reader.IsDBNull(5) ? reader.GetString(5) : "");

                    _dictionary[name] = new LoginRecord(name, pwd,cell,role, storeName,companyAccnum,companyName,storeAcctNum);
                }
                reader.Close();
            }
        }
        public LoginRecord  this[string value]
        {
            get
            {
                return _dictionary.TryGetValue(value, out var result) ? result : null;
            }
        }

        #endregion
        #region IEnumerable

        public IEnumerator<LoginRecord> GetEnumerator()
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
