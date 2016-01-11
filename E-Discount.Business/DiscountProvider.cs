using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using E_Discount.DB_Model;

namespace E_Discount.Business
{
    public class DiscountProvider
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public static bool InsertDiscount(int storeID, double discountValue, TimeSpan start, TimeSpan end, DateTime from, DateTime to, string description, int times)
        {
            //bool result = false;
            DISCOUNT discount = new DISCOUNT() { VALUE = discountValue, START_TIME = start, END_TIME = end, AVAILABLE_FROM = from, AVAILABLE_TO = to, DESCRIPTION = description, TIMES = times, STORE_ID = storeID , AVAILABLE="YES", POPULAR=0};
            try
            {
                mydb.DISCOUNTs.InsertOnSubmit(discount);
                mydb.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
