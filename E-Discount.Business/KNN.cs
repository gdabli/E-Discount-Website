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
    public class KNN
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public static string RecommendationByKNN(double latitude, double longitude, double radius, int categoryID, int maxReturns,string userid)
        {
            try
            {
                var nearbyStores = mydb.GetNearbyStoresByCategory(latitude, longitude, radius, categoryID, maxReturns).ToList();
                List<List<double>> trainingDS =//new List<List<double>> { new List<double> { 8.7, 1, 3, 4.725, 0 }, new List<double> { 8.8, 1, 4, 4.137, 0 }, new List<double> { 8.9, 1, 1, 3.721, 0 }, new List<double> { 9.0, 0, 3, 4.20201, 0 }, new List<double> { 9.1, 0, 3, 4.51531, 0 }, new List<double> { 9.2, 1, 3.1, 3.5341, 0 } };
                                (from s in nearbyStores
                                 join f in mydb.FEEDBACKs on s.STORE_ID equals f.STORE_ID
                                 where f.USER_ID == userid
                                 select new List<double> { s.STORE_ID, f.IS_LIKED, s.PRICE_LEVEL, s.RATING }).ToList();
                var testdata = //new List<List<double>>{new List<double> {8.1,0,3,4.70341,0} ,new List<double> {8.2,0,4,3.7178,0},new List<double> {8.4,0,1,4.2567,0},new List<double> {8.5,0,3,4.2213,0},new List<double> {8.6,0,3,3.534,0}};
                                (from s in nearbyStores
                                join f in mydb.FEEDBACKs on s.STORE_ID equals f.STORE_ID
                                 select new List<double> { s.STORE_ID,0, s.PRICE_LEVEL, s.RATING }).ToList();  
                if (trainingDS.Count() == 0 || testdata.Count() == 0)
                {
                    return "";
                }
                List<List<double>> testingDS = new List<List<double>>();
                foreach(List<double> t in testdata)
                {
                    double sid = t.ElementAt(0);
                    var s = testingDS.SingleOrDefault(td => td.ElementAt(0) == sid);
                    if (s==null)
                    {
                        testingDS.Add(t);
                    }
                }
                double[] classOfTesting = new double[2];

                foreach (var testing in testingDS)
                {
                    classOfTesting[0] = 0.0; // unliked
                    classOfTesting[1] = 0.0; // liked
                    foreach (var training in trainingDS)
                    {
                        double wi=KNNByEuclideanDistance(testing, training);
                        if (training[1] == 1)//1 means liked
                        {
                            classOfTesting[1] += wi;
                        }
                        else
                        {
                            classOfTesting[0] += wi;
                        }
                    }
                    testing[1] = classOfTesting[1] > classOfTesting[0] ? 1 : 0;
                }
                var sidList = from test in testingDS
                              where test[1] == 1
                              select test[0];
                var stores = from s in nearbyStores
                             join test in testingDS on s.STORE_ID equals test[0]
                             where test[1] == 1
                             select s;
                return myConvert.Serialize(stores);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private static double KNNByEuclideanDistance(List<double> testObj, List<double> trainObj)
        {
            double sum = 0.0;
            if (testObj.Count != trainObj.Count)
            {
                throw new System.ArgumentException("the number of elements in Z must match the number of elements in Y");
            }
            for (int i = 2; i < 4; i++)
            {
                sum = sum + Math.Pow(Math.Abs(testObj[i] - trainObj[i]), 2);
            }
            if (sum == 0)
            {
                return double.MaxValue;
            }
            else
            {
                return 1 / sum;
            }
        }
    }
}
