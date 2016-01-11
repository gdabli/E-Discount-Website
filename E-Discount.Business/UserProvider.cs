using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using E_Discount.DB_Model;
using System.Web;

namespace E_Discount.Business
{
    public class UserProvider
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();

        public static string InsertUser(string firstName, string lastName,string loginID, string password,string email,string role)
        {
            try
            {
                if (mydb.USERs.SingleOrDefault(u => u.USER_ID == loginID) != null)
                {
                    return "";
                }
                USER user = new USER() { FIRST_NAME = firstName, LAST_NAME = lastName, PASSWORD = password, EMAIL = email, USER_ID = loginID, REGISTER_DATE=DateTime.Now, ROLE=role };
                mydb.USERs.InsertOnSubmit(user);
                mydb.SubmitChanges();
                return SaveLoginSession(user);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private static string SaveLoginSession(USER user)
        {
            LoginSession ls = new LoginSession(user.FIRST_NAME + "." + user.LAST_NAME, user.ROLE);
           string result=myConvert.Serialize(ls);
           HttpContext.Current.Session["user"] = user;
           return result;
        }
        public static string ValidateUser(string loginID, string password)
        {
            try
            {
                var user = mydb.USERs.SingleOrDefault(u => u.USER_ID == loginID);
                if (user != null)
                {
                    if (user.PASSWORD == password)
                    {
                        return SaveLoginSession(user);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string GetUser(string loginID)
        {
            try
            {
                var user = mydb.GetUser(loginID);
                if (user != null)
                {
                    return myConvert.Serialize(user);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string UpdatetUserInfo(string firstName, string lastName, string loginID, string password, string email)
        {
            try
            {
                var user = mydb.USERs.SingleOrDefault(u => u.USER_ID == loginID);
                if (user == null)
                {
                    return "";
                }
                else
                {
                    user.FIRST_NAME = firstName;
                    user.LAST_NAME = lastName;
                    user.PASSWORD = password;
                    user.EMAIL = email;
                    mydb.SubmitChanges();
                    return SaveLoginSession(user);;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string ValidateUserByFaceBookName(string faceBookName)
        {
            try
            {
                var user = mydb.USERs.SingleOrDefault(u => u.FIRST_NAME == faceBookName);
                if (user != null)
                {
                    return SaveLoginSession(user);
                }
                return "";
            }
            catch 
            {
                return "";
            }
        }

    }
    public class LoginSession
    {
        public string Name;
        public string Role;
        public LoginSession(string name, string role)
        {
            this.Name = name;
            this.Role = role;
        }
    }
}
