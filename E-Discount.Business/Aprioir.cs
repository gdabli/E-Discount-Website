using E_Discount.DB_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using E_Discount.DB_Model;
using System.Collections;

namespace E_Discount.Business
{
    public class Aprioir
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        static int minSuppCount = 2;
        public static string RecommendationByAprioir(double latitude, double longitude, double radius, int categoryID, int maxReturns, string userid)
        {
            var nearbyStores = mydb.GetNearbyStoresByCategory(latitude, longitude, radius, categoryID, maxReturns).ToList(); // should tolist it before use this query result
            Hashtable frequencyList = new Hashtable();
            Hashtable sidContainer = new Hashtable();
                var youlike =  //new List<int>(){3,5};
                    (from s in nearbyStores
                     join f in mydb.FEEDBACKs on s.STORE_ID equals f.STORE_ID
                     where f.IS_LIKED == 1 && f.USER_ID==userid
                     select f.STORE_ID).Distinct();
                var keyfrequency = //new List<KeyFrequency>(){ new KeyFrequency(1),new KeyFrequency(2),new KeyFrequency(3),new KeyFrequency(4),new KeyFrequency(5),new KeyFrequency(6),new KeyFrequency(7)};
                    (from s in nearbyStores
                     join f in mydb.FEEDBACKs on s.STORE_ID equals f.STORE_ID
                     where f.IS_LIKED == 1
                     select new KeyFrequency(f.STORE_ID)).Distinct().ToList();

                var trainingDS = 
                    (from s in nearbyStores
                     join f in mydb.FEEDBACKs on s.STORE_ID equals f.STORE_ID
                     where f.IS_LIKED == 1
                     group f.STORE_ID by f.USER_ID into g
                     select new AprioirTrainingObj() { UserID=g.Key, LikedStores=g.ToList()}).ToList();
                         
                    //new List<List<int>>() { new List<int>() { 1, 2, 3 }, new List<int>() { 1, 4, 5 }, new List<int>() { 1, 2 }, new List<int>() { 1,2,3,7}};//example in your photo
                               //new List<List<int>>() { new List<int>() { 1, 2, 3, 4, 5 }, new List<int>() { 3, 4, 5 }, new List<int>() { 2, 4, 7 }, new List<int>() { 4, 5 }, new List<int>() { 7, 2, 5 }, new List<int>() { 1, 3, 6 }, new List<int>() { 2, 3, 7 }, new List<int>() { 3, 1, 2 }, new List<int>() { 2, 4, 6 }, new List<int>() { 3, 1, 5 }, new List<int>() { 2, 3, 5 } };// an other example
               
                int keyNumber = 1;
                RecursiveClassify(keyfrequency, trainingDS, frequencyList, sidContainer, ref keyNumber);
                if (sidContainer[keyNumber] == null)
                {
                    keyNumber--;
                }
                var recommendationList = (sidContainer[keyNumber] as List<int>).Except(youlike).ToList();
                var stores = from s in nearbyStores
                             join r in recommendationList on s.STORE_ID equals r
                             select s;
                return myConvert.Serialize(stores);
        }
        public static void RecursiveClassify(List<KeyFrequency> keyfrequency, List<AprioirTrainingObj> trainingDS, Hashtable frequencyList, Hashtable sidContainer, ref int keyNumber)
        {
            int count;
            List<KeyFrequency> nextKeyfrequency = new List<KeyFrequency>();
            List<List<int>> tempKeyList = new List<List<int>>();
            List<int> sidList = new List<int>();
            for (int i = 0; i < keyfrequency.Count; i++)
            {
                count = 0;
                foreach (AprioirTrainingObj trainingObj in trainingDS)
                {
                    bool haskey =true;
                    if (trainingObj == null)
                    { continue; }
                    foreach (int sid in keyfrequency[i].Key)
                    { 
                        int isExist=trainingObj.LikedStores.FirstOrDefault(s => s == sid);
                        if (isExist == 0)
                        {
                            haskey = false;
                        }
                    }
                    if (haskey)
                    {
                        count++;
                    }
                }
                keyfrequency[i].Frequency = count;
                if (count >= minSuppCount)
                {
                    tempKeyList.Add(keyfrequency[i].Key);
                    foreach (int sid in keyfrequency[i].Key)
                    {
                        sidList.Add(sid);
                    }
                }
            }
            if (tempKeyList.Count > 0)
            {
                frequencyList.Add(keyNumber, keyfrequency);
                sidList = sidList.Distinct().ToList();
                sidContainer.Add(keyNumber, sidList); 
                List<int> currentKey;
                foreach (List<int> key in tempKeyList)
                {
                    foreach (int sid in sidList)
                    {
                        currentKey = new List<int>();
                        currentKey.AddRange(key);
                        int isExist = currentKey.FirstOrDefault(k => k == sid);
                        if (isExist == 0)
                        {
                            currentKey.Add(sid);
                            var isExistKey = nextKeyfrequency.FirstOrDefault(kf => kf.Key.OrderByDescending(k => k).SequenceEqual(currentKey.OrderByDescending(k => k)));
                            if (isExistKey == null)
                            {
                                nextKeyfrequency.Add(new KeyFrequency(currentKey));
                            }
                        }
                    }
                }
                keyNumber++;
                RecursiveClassify(nextKeyfrequency, trainingDS, frequencyList,sidContainer, ref keyNumber);
            }
        }
    }
    public class KeyFrequency
    {
        public int Frequency { get; set; }
        public List<int> Key { get; set; }
        public KeyFrequency()
        {
            Key = new List<int>();
            Frequency = 0;
        }
        public KeyFrequency(int key):this()
        {
            this.Key.Add(key);
        }
        public KeyFrequency(List<int> key)
            : this()
        {
            this.Key = key;
        }
    }
    public class AprioirTrainingObj
    {
        public string UserID { get; set; }
        public List<int> LikedStores{get;set;}
        public AprioirTrainingObj()
        {
            LikedStores = new List<int>();
        }
    }
}
