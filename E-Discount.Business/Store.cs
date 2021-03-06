﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Discount.Business
{
    public class Store
    {
        public Address_Components[] address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public string formatted_phone_number { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public Opening_Hours opening_hours { get; set; }
        public Photo[] photos { get; set; }
        public string place_id { get; set; }
        public int price_level { get; set; }
        public float rating { get; set; }
        public string reference { get; set; }
        public Review[] reviews { get; set; }
        public string scope { get; set; }
        public string[] types { get; set; }
        public string url { get; set; }
        public int user_ratings_total { get; set; }
        public int utc_offset { get; set; }
        public string vicinity { get; set; }
        public string website { get; set; }
        public object[] html_attributions { get; set; }
        public string tz { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public float k { get; set; }
        public float B { get; set; }
    }

    public class Opening_Hours
    {
        public bool open_now { get; set; }
        public Period[] periods { get; set; }
        public string[] weekday_text { get; set; }
    }

    public class Period
    {
        public Close close { get; set; }
        public Open open { get; set; }
    }

    public class Close
    {
        public int day { get; set; }
        public string time { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public long nextDate { get; set; }
    }

    public class Open
    {
        public int day { get; set; }
        public string time { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public long nextDate { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public string[] html_attributions { get; set; }
        public int width { get; set; }
    }

    public class Review
    {
        public Aspect[] aspects { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string language { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
        public int time { get; set; }
    }

    public class Aspect
    {
        public int rating { get; set; }
        public string type { get; set; }
    }

}
