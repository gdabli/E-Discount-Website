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
    public class FeedbackProvider
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public static bool InsertFeedback(string pid, int isliked, int rating, string comment)
        {
            //bool result = false;
            if (string.IsNullOrEmpty(pid))
            {
                return false;
            }
            STORE store = mydb.STOREs.SingleOrDefault(s => s.PLACE_ID == pid);
            if (store == null)
            {
                return false;
            }
            if (HttpContext.Current.Session == null)
            {
                return false;
            }
            USER user = HttpContext.Current.Session["user"] as USER;
            FEEDBACK feedback = new FEEDBACK() { IS_LIKED=isliked,RATING=rating, COMMENT=comment, RATING_DATE= DateTime.Now,STORE_ID=store.STORE_ID , USER_ID=user.USER_ID};
            try
            {
                mydb.FEEDBACKs.InsertOnSubmit(feedback);
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
