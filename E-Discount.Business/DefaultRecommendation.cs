using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Discount.DB_Model;
using System.Collections;
using System.Web.Script.Serialization;

namespace E_Discount.Business
{
    public class DefaultRecommendation
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public static string Recommendation(double latitude, double longitude, double radius, int maxReturns,string userid)
        {
            var categoryID = (from uc in mydb.USER_CATEGORies
                              where uc.USER_ID == userid
                              orderby uc.COUNT
                              select uc.CATEGORY_ID).FirstOrDefault();
            if (categoryID != 0)
            {
                var stores = mydb.GetNearbyStoresByCategory(latitude, longitude, radius, categoryID, maxReturns); ;
                return myConvert.Serialize(stores);
            }
            return "";
        }
    }
}
