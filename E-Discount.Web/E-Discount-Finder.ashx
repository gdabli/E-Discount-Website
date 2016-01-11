<%@ WebHandler Language="C#" Class="E_Discount_Finder" %>

using System;
using System.Web;
using E_Discount.Business;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using E_Discount.DB_Model;

public class E_Discount_Finder : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string query = context.Request.QueryString["query"] as string;
        string result = GetSolutions(query, context);
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    private string GetSolutions(string query, HttpContext context)
    {
        try
        {
            switch (query)
            {
                case "login":
                    {
                        string loginID = context.Request.QueryString["loginID"];
                        string password = context.Request.QueryString["password"];
                        return UserProvider.ValidateUser(loginID, password);
                    }
                case "logoff":
                    {
                        HttpContext.Current.Session.Clear();
                        return "";
                    }
                case "checkLogin":
                    {
                        if (HttpContext.Current.Session!=null&&HttpContext.Current.Session["user"] != null)
                        {
                            USER user = HttpContext.Current.Session["user"] as USER;
                            return UserProvider.ValidateUser(user.USER_ID, user.PASSWORD);
                        }
                        return "";
                    }
                case "modifyUser":
                    {
                        string firstName = context.Request.QueryString["fname"];
                        string lastName = context.Request.QueryString["lname"];
                        string loginID = context.Request.QueryString["loginID"];
                        string password = context.Request.QueryString["password"];
                        string email = context.Request.QueryString["email"];
                        return UserProvider.UpdatetUserInfo(firstName, lastName, loginID, password, email);
                    }
                case "getOwner":
                    {
                        if (HttpContext.Current.Session!=null&&HttpContext.Current.Session["user"] != null)
                        {
                            USER user = HttpContext.Current.Session["user"] as USER;
                            string userjson= UserProvider.GetUser(user.USER_ID);
                            string storejson = StoreProvider.getStore(user.USER_ID);
                            return userjson + "___" + storejson;
                        }
                        return "";
                        
                    }
                case "modifyOwner":
                    {
                        string firstName = context.Request.QueryString["fname"];
                        string lastName = context.Request.QueryString["lname"];
                        string loginID = context.Request.QueryString["loginID"];
                        string password = context.Request.QueryString["password"];
                        string email = context.Request.QueryString["email"];
                        return UserProvider.UpdatetUserInfo(firstName, lastName, loginID, password, email);
                    }
                case "register":
                    {
                        string firstName = context.Request.QueryString["fname"];
                        string lastName = context.Request.QueryString["lname"];
                        string loginID = context.Request.QueryString["loginID"];
                        string password = context.Request.QueryString["password"];
                        string email = context.Request.QueryString["email"];
                        return UserProvider.InsertUser(firstName, lastName, loginID, password, email, "0");
                    }
                case "registerAsOwner":
                    {
                        string longitude = context.Request.QueryString["longitude"];
                        string latitude = context.Request.QueryString["latitude"];
                        string bname = context.Request.QueryString["bname"];
                        string street_number = context.Request.QueryString["street_number"];
                        string route = context.Request.QueryString["route"];
                        string city = context.Request.QueryString["city"];
                        string state = context.Request.QueryString["state"];
                        string postal_code = context.Request.QueryString["postal_code"];
                        string country = context.Request.QueryString["country"];
                        string price = context.Request.QueryString["price"];
                        string firstName = context.Request.QueryString["fname"];
                        string lastName = context.Request.QueryString["lname"];
                        string loginID = context.Request.QueryString["loginID"];
                        string password = context.Request.QueryString["password"];
                        string phone = context.Request.QueryString["phone"];
                        string email = context.Request.QueryString["email"];
                        string website = context.Request.QueryString["website"];
                        string description = context.Request.QueryString["description"];
                        string result = UserProvider.InsertUser(firstName, lastName, loginID, password, email, "1");
                        StoreProvider.InsertStore(loginID, longitude, latitude, bname, phone, street_number, route, city, state, postal_code, country, price, website, description);
                        return result;
                    }
                case "discount":
                    {
                        try
                        {
                            int sid =Convert.ToInt32( context.Request.QueryString["sid"]);
                            double discount = Convert.ToDouble(context.Request.QueryString["discount"]);
                            TimeSpan start_time =Convert.ToDateTime( context.Request.QueryString["start_time"]).TimeOfDay;
                            TimeSpan end_time = Convert.ToDateTime(context.Request.QueryString["end_time"]).TimeOfDay;
                            DateTime available_from = Convert.ToDateTime(context.Request.QueryString["available_from"]).Date;
                            DateTime available_to = Convert.ToDateTime(context.Request.QueryString["available_to"]).Date;
                            string description = context.Request.QueryString["description"];
                            int times =Convert.ToInt32( context.Request.QueryString["times"]);
                            DiscountProvider.InsertDiscount(sid, discount, start_time, end_time, available_from, available_to, description, times);
                            return "success";
                        }
                        catch
                        {
                            return "";
                        }
                    }
                case "feedback":
                    {
                        try
                        {
                            string pid = context.Request.QueryString["pid"];
                            int isliked = Convert.ToInt32(context.Request.QueryString["isliked"]);
                            int rating = Convert.ToInt32(context.Request.QueryString["rating"]);
                            string comment = context.Request.QueryString["comment"];
                            FeedbackProvider.InsertFeedback(pid, isliked, rating, comment);
                            return "success";
                        }
                        catch
                        {
                            return "";
                        }
                    }
                case "byCategory":
                    {
                        double latitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["latitude"]))
                        {
                            latitude = Convert.ToDouble(context.Request.QueryString["latitude"]);
                        }
                        double longitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["longitude"]))
                        {
                            longitude = Convert.ToDouble(context.Request.QueryString["longitude"]);
                        }
                        double radius = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["radius"]))
                        {
                            radius = Convert.ToDouble(context.Request.QueryString["radius"]);
                        }
                        int categoryID = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["category"]))
                        {
                            categoryID = Convert.ToInt32(context.Request.QueryString["category"]);
                        }
                        string result = StoreProvider.GetNearbyStoresByCategory(latitude, longitude, radius, categoryID, 100);
                        return result;
                    }
                case "category":
                    {
                        CategoryProvider cp = new CategoryProvider();
                        string result = cp.GetCategories();
                        return result;
                    }

                case "savedata":
                    {
                        string json = new System.IO.StreamReader(context.Request.InputStream).ReadToEnd();
                        StoreProvider.InsertGoogleStore(json);
                        return "success";
                    }
                case "default":
                    {
                        double latitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["latitude"]))
                        {
                            latitude = Convert.ToDouble(context.Request.QueryString["latitude"]);
                        }
                        double longitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["longitude"]))
                        {
                            longitude = Convert.ToDouble(context.Request.QueryString["longitude"]);
                        }
                        double radius = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["radius"]))
                        {
                            radius = Convert.ToDouble(context.Request.QueryString["radius"]);
                        }
                        string stores = DefaultRecommendation.Recommendation(latitude, longitude, radius, 100, "YongZhuang");
                        return stores;
                    }
                case "knn":
                    {
                        double latitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["latitude"]))
                        {
                            latitude = Convert.ToDouble(context.Request.QueryString["latitude"]);
                        }
                        double longitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["longitude"]))
                        {
                            longitude = Convert.ToDouble(context.Request.QueryString["longitude"]);
                        }
                        double radius = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["radius"]))
                        {
                            radius = Convert.ToDouble(context.Request.QueryString["radius"]);
                        }
                        int categoryID = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["category"]))
                        {
                            categoryID = Convert.ToInt32(context.Request.QueryString["category"]);
                        }
                        string stores = KNN.RecommendationByKNN(latitude, longitude, radius, categoryID, 100,"YongZhuang");
                        return stores;
                    }
                case "aprioir":
                    {
                        double latitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["latitude"]))
                        {
                            latitude = Convert.ToDouble(context.Request.QueryString["latitude"]);
                        }
                        double longitude = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["longitude"]))
                        {
                            longitude = Convert.ToDouble(context.Request.QueryString["longitude"]);
                        }
                        double radius = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["radius"]))
                        {
                            radius = Convert.ToDouble(context.Request.QueryString["radius"]);
                        }
                        int categoryID = 0;
                        if (!string.IsNullOrEmpty(context.Request.QueryString["category"]))
                        {
                            categoryID = Convert.ToInt32(context.Request.QueryString["category"]);
                        }
                        string stores = Aprioir.RecommendationByAprioir(latitude, longitude, radius, categoryID, 100, "YongZhuang");
                        return stores;
                    }
                default:
                    {
                        string resultJson = "none";
                        return resultJson;
                    }
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}



