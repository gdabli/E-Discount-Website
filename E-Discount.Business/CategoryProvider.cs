using E_Discount.DB_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace E_Discount.Business
{
    public class CategoryProvider
    {
         SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
         JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public  string GetCategories()
        {
            try
            {
                var result = mydb.GetCategories();
                return myConvert.Serialize(result);
                //StringBuilder sb = new StringBuilder();
                //XmlWriter writer = XmlWriter.Create(sb);
                //myConvert.WriteObject(writer, myConvert);
                //writer.Close();
                //string xml = sb.ToString();
                //return xml;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
