
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using E_Discount.DB_Model;

namespace E_Discount.Business
{
    public class StoreProvider
    {
        static SQL_Server_ModelDataContext mydb = new SQL_Server_ModelDataContext();
        static JavaScriptSerializer myConvert = new JavaScriptSerializer();
        public static string GetNearbyStoresByCategory(double latitude, double longitude, double radius, int categoryID, int maxReturns)
        {
            var result = mydb.GetNearbyStoresByCategory(latitude, longitude, radius, categoryID, maxReturns);
            return myConvert.Serialize(result);
        }
        public static string InsertStore(string userid, string longitude, string latitude, string storeName, string phone, string street_number, string route, string city, string state, string postal_code, string country, string price, string website, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(longitude) || string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(street_number) || string.IsNullOrEmpty(route) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(postal_code) || string.IsNullOrEmpty(country))
                {
                    return Strings.StoreAlert_Location;
                }
                if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(price))
                {
                    return Strings.StoreAlert_Info;
                }
                LOCATION location = mydb.LOCATIONs.SingleOrDefault(l => l.LATITUDE == Convert.ToDouble(latitude) && l.LONGITUDE == Convert.ToDouble(longitude));
                if (location == null)
                {
                    location = new LOCATION() { LONGITUDE = Convert.ToDouble(longitude), LATITUDE = Convert.ToDouble(latitude) };
                    mydb.LOCATIONs.InsertOnSubmit(location);
                }
                ADDRESS address = mydb.ADDRESSes.SingleOrDefault(a => a.ADDRESS_LINE1 == street_number + " " + route && a.CITY == city && a.STATE_PROVINCE_REGION == state && a.POSTAL_CODE == postal_code && a.COUNTRY == country);
                if (address == null)
                {
                    address = new ADDRESS() { ADDRESS_LINE1 = street_number + " " + route, CITY = city, STATE_PROVINCE_REGION = state, POSTAL_CODE = postal_code, COUNTRY = country };
                    mydb.ADDRESSes.InsertOnSubmit(address);
                }
                STORE store = new STORE() { NAME = storeName, DESCRIPTION = description, PHONE = phone, WEBSITE = website, OWNER = userid, PRICE_LEVEL = Convert.ToInt32(price) };
                store.LOCATION = location;
                store.ADDRESS = address;
                mydb.SubmitChanges();
                return Strings.StoreAlert_Success;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string getStore(string userid)
        {
            var result = mydb.GetStoresByOwner(userid).FirstOrDefault();
            return myConvert.Serialize(result);
        }
        public static void InsertGoogleStore(string storejson)
        {
            Store s = myConvert.Deserialize<Store>(storejson);
            try
            {
                if (s == null)
                { return; }
                Random r = new Random();
                int i = r.Next(0, 6);
                //string[] users = new string[6] { "gdabli", "harshisame", "nari.dontula", "rungta.sachin", "SiChen", "YongZhuang" };
                string[] users = new string[6] { "Gaurav_Dabli", "Harshita_Gupta", "Harshita_Gupta", "Nari_Dontula", "Nari_Dontula", "Yong_Zhuang" };
                if (mydb.STOREs.SingleOrDefault(ss => ss.PLACE_ID == s.place_id) != null)
                {
                    return;
                }
                //{ "geometry": { "location": { "k": 42.360128, "B": -71.055454 } }, "name": "Cheers", "types": ["cafe", "bar", "restaurant", "food", "establishment"], "vicinity": "1 S Market St, Boston", "html_attributions": [] }
                STORE store = new STORE() { NAME = s.name, PLACE_ID = s.place_id, PRICE_LEVEL = s.price_level, RATING = s.rating, PHONE = s.formatted_phone_number, WEBSITE = s.website, OWNER = users[i], GOOGLE_URL = s.url };
                LOCATION location = new LOCATION() { LATITUDE = s.geometry.location.k, LONGITUDE = s.geometry.location.B };
                mydb.LOCATIONs.InsertOnSubmit(location);
                store.LOCATION = location;
                ADDRESS address = new ADDRESS() { ADDRESS_LINE1 = s.vicinity.Split(',')[0], CITY = s.vicinity.Split(',')[1] };
                if (s.address_components != null)
                {
                    foreach (Address_Components ad in s.address_components)
                    {
                        foreach (string type in ad.types)
                        {
                            switch (type)
                            {
                                case "country":
                                    {
                                        address.COUNTRY = ad.long_name;
                                        break;
                                    }
                                case "postal_code":
                                    {
                                        address.POSTAL_CODE = ad.long_name;
                                        break;
                                    }
                                case "administrative_area_level_1":
                                    {
                                        address.STATE_PROVINCE_REGION = ad.long_name;
                                        break;
                                    }
                            }
                        }
                    }
                }
                mydb.ADDRESSes.InsertOnSubmit(address);
                store.ADDRESS = address;
                if (s.opening_hours != null)
                {
                    foreach (Period period in s.opening_hours.periods)
                    {
                        PERIOD p = new PERIOD() { OPEN_DAY = period.open.day, CLOSE_DAY = period.close.day, OPEN_TIME = Convert.ToDateTime(period.open.hours + ":" + period.open.minutes).TimeOfDay, CLOSE_TIME = Convert.ToDateTime(period.close.hours + ":" + period.close.minutes).TimeOfDay };
                        store.PERIODs.Add(p);
                    }
                }
                if (s.reviews != null)
                {
                    foreach (Review review in s.reviews)
                    {
                        if (string.IsNullOrEmpty(review.author_name) || review.author_name == "A Google User")
                        {
                            continue;
                        }
                        USER user = mydb.USERs.SingleOrDefault(u => u.FIRST_NAME + " " + u.LAST_NAME == review.author_name);
                        if (user == null)
                        {
                            string[] name = review.author_name.Split(' ');
                            if (name.Length > 1)
                            {
                                user = new USER() { FIRST_NAME = name[0], LAST_NAME = name[1], USER_ID = name[0] + "_" + name[1], EMAIL = name[0] + "." + name[1] + "@gmail.com", PASSWORD = "123456", ROLE = "0", REGISTER_DATE = DateTime.Now };
                            }
                            else if (name.Length > 0)
                            {
                                user = new USER() { FIRST_NAME = name[0], LAST_NAME = "", USER_ID = name[0], EMAIL = name[0] + "@gmail.com", PASSWORD = "123456", ROLE = "0", REGISTER_DATE = DateTime.Now };
                            }
                            mydb.USERs.InsertOnSubmit(user);
                        }
                        if (user != null)
                        {
                            FEEDBACK feed = new FEEDBACK() { COMMENT = review.text, RATING = review.rating  };
                            feed.IS_LIKED = feed.RATING > 2.5 ? 1 : 0;
                            feed.USER = user;
                            feed.STORE = store;
                            mydb.FEEDBACKs.InsertOnSubmit(feed);
                        }
                    }
                }
                if (s.types != null)
                {
                    foreach (string type in s.types)
                    {
                        CATEGORY catergory = mydb.CATEGORies.SingleOrDefault(c => c.NAME == type);
                        if (catergory == null)
                        {
                            catergory = new CATEGORY() { NAME = type, LAYER = 1, PARENT = 0 };
                            mydb.CATEGORies.InsertOnSubmit(catergory);
                        }
                        STORE_CATEGORY s_c = new STORE_CATEGORY();
                        s_c.CATEGORY = catergory;
                        s_c.STORE = store;
                        mydb.STORE_CATEGORies.InsertOnSubmit(s_c);
                    }
                }
                mydb.SubmitChanges();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
