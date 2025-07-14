using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonConformanceReport.Models
{
    public class Function
    {
        public class CookieModel
        {
            private static double _Hour = 8;
            public String Key { get; set; }
            public String Value { get; set; }
            public Double Hour { get { return Convert.ToDouble(Value); } set { Value = Convert.ToString(_Hour); } }
        }
        public static Boolean Authenticate()
        {
            try
            {
                if (HttpContext.Current.Session["UserStatus"] == null && Convert.ToBoolean(HttpContext.Current.Session["UserStatus"]) == false)
                    return true;
                else if (String.IsNullOrEmpty(ReadCookie("EncryptedSessionToken")) == false && String.IsNullOrEmpty(ReadCookie("EncryptedUserId")) == false)
                    return true;
                else
                    return true;

            }
            catch { return false; }
        }
        public static String ReadCookie(String Key)
        {
            try
            {
                string keySession = HttpContext.Current.Session[Key] as string;

                if (!String.IsNullOrEmpty(keySession))
                {
                    return Convert.ToString(HttpContext.Current.Session[Key]);
                }
                else if (HttpContext.Current.Request.Cookies[Key] != null)
                {

                    //Fetch the Cookie using its Key.
                    HttpCookie nameCookie = HttpContext.Current.Request.Cookies[Key];

                    //If Cookie exists fetch its value.
                    HttpContext.Current.Session[Key] = nameCookie[Key];
                    return nameCookie[Key];
                }
                else
                    return null;
            }
            catch { return null; }
        }

        public static void DeleteCookie(String Key)
        {
            try
            {
                //Fetch the Cookie using its Key.
                HttpCookie nameCookie = HttpContext.Current.Request.Cookies[Key];
                if (nameCookie != null)
                {
                    nameCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(nameCookie);
                    //If Cookie exists fetch its value.
                    HttpContext.Current.Session[Key] = null;
                }
            }
            catch { }
        }

        public static void WriteCookie(String Key, String Value, Double Hour = 8)
        {
            try
            {
                // store value into session
                HttpContext.Current.Session[Key] = Value;

                //Create a Cookie with a suitable Key.
                HttpCookie nameCookie = new HttpCookie(Key);
                //Set the Cookie value.
                nameCookie.Values[Key] = Value;
                //Set the Expiry date.
                nameCookie.Expires = DateTime.Now.AddDays(Hour);

                //Add the Cookie to Browser.
                HttpContext.Current.Response.Cookies.Add(nameCookie);
            }
            catch (Exception ex) { String err = ex.Message; }
        }
        public static void WriteCookie(List<CookieModel> parms)
        {
            try
            {
                if (parms != null && parms.Count > 0)
                {
                    foreach (var wc in parms)
                    {
                        if (String.IsNullOrEmpty(wc.Key) == false && String.IsNullOrEmpty(wc.Value) == false)
                        {
                            //Create a Cookie with a suitable Key.
                            HttpCookie nameCookie = new HttpCookie(wc.Key);
                            //Set the Cookie value.
                            nameCookie.Values[wc.Key] = wc.Value;
                            //Set the Expiry date.
                            nameCookie.Expires = DateTime.Now.AddDays(wc.Hour);
                            //Add the Cookie to Browser.
                            HttpContext.Current.Response.Cookies.Add(nameCookie);
                        }
                    }
                }
            }
            catch (Exception ex) { String err = ex.Message; }
        }
    }
}