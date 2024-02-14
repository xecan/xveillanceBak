using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurotec.Samples.Code
{
    public static class SessionManager
    {
        private static string otp;
        private static string role;
        private static string storeAccNum;
        private static string storeName;
        private static string companyAcctNum;
        private static string companyName;
        private static string faceId;
        private static bool isAllVisitors;


        public static string Otp
        {
            get { return otp; }
            set { otp = value; }
        }
        public static string Role
        {
            get { return role; }
            set { role = value; }
        }
        public static string StoreAccNum
        {
            get { return storeAccNum; }
            set { storeAccNum = value; }
        }
        public static string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        public static string CompanyAccNum
        {
            get { return companyAcctNum; }
            set { companyAcctNum = value; }
        }
        public static string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
        public static string FaceId
        {
            get { return faceId; }
            set { faceId = value; }
        }
        public static bool IsAllVisitors
        {
            get { return isAllVisitors; }
            set { isAllVisitors = value; }
        }
        public static void ClearSession()
        {
            otp = null;
            // Add more session variables to clear if needed
        }
    }

}
