
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace NCRBAL.Business
{
    public class UtilityAccess : System.Web.Mvc.ActionFilterAttribute
    {
        //public static string BaseUrl = ConfigurationManager.AppSettings["baseurl"].ToString();
        public enum DropdownType
        {
            All = 0,
            Required = 1,
            NoRequired = 2,
        }
        //private UtilityData utilityData = new UtilityData();
        public static List<SelectListItem> MRNNumberList(DataTable dt, Int32 Type)
        {
            List<SelectListItem> _items = new List<SelectListItem>();
            try
            {
                DropdownType drpType = (DropdownType)Type;
                switch (drpType)
                {
                    case DropdownType.All:
                        _items.Add(new SelectListItem { Value = "-1", Text = "All" });
                        break;
                    case DropdownType.Required:
                        _items.Add(new SelectListItem { Value = "", Text = "--Select--" });
                        break;
                    case DropdownType.NoRequired:
                        _items.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                        break;
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _items.Add(new SelectListItem { Value = Convert.ToString(row["Value"]), Text = Convert.ToString(row["Text"]) });
                    }
                }
                return _items;
            }
            catch (Exception ex)
            {
                ///ApplicationLogger.LogError(ex, "UtilityAccess", "RenderCountryList");
                return _items;
            }
        }

        public static List<SelectListItem> ItemCodeList(DataTable dt, Int32 Type)
        {
            List<SelectListItem> _items = new List<SelectListItem>();
            try
            {
                DropdownType drpType = (DropdownType)Type;
                switch (drpType)
                {
                    case DropdownType.All:
                        _items.Add(new SelectListItem { Value = "-1", Text = "All" });
                        break;
                    case DropdownType.Required:
                        _items.Add(new SelectListItem { Value = "", Text = "--Select--" });
                        break;
                    case DropdownType.NoRequired:
                        _items.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                        break;
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _items.Add(new SelectListItem { Value = Convert.ToString(row["Value"]), Text = Convert.ToString(row["Text"]) });
                    }
                }
                return _items;
            }
            catch (Exception ex)
            {
                ///ApplicationLogger.LogError(ex, "UtilityAccess", "RenderCountryList");
                return _items;
            }
        }
        public static List<SelectListItem> DRPMRNLIST()
        {
            List<SelectListItem> _items = new List<SelectListItem>();
            //_items.Add(new SelectListItem { Value = "FirstAid", Text = "First Aid" });
            //_items.Add(new SelectListItem { Value = "Ambulance", Text = "Ambulance" });
            //_items.Add(new SelectListItem { Value = "EmergencyKit", Text = "Emergency Kit" });
            return _items;
        }
        public static List<SelectListItem> YearList()
        {
            List<SelectListItem> _items = new List<SelectListItem>();
            _items.Add(new SelectListItem { Value = "0", Text = "All Year" });
            DateTime PreYr = DateTime.Now.AddYears(-1);
            for (int i = Convert.ToInt32(PreYr.Year); i <= DateTime.Now.Year; i++)
            {
                _items.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            return _items;
        }
        public static List<SelectListItem> MonthList()
        {
            List<SelectListItem> _items = new List<SelectListItem>();
            _items.Add(new SelectListItem { Value = "0", Text = "All Month" });
            for (int month = 1; month <= 12; month++)
            {
                string monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                _items.Add(new SelectListItem { Value = month.ToString().PadLeft(2, '0'), Text = monthName });
                
            }
            return _items;
        }
        public static int SendEmail(String emailTo, String CC_EMail, String subject, String emailBody,bool blnIsHtml, Attachment attachment)
        {
            try
            {
                string smtpFromEmail = ConfigurationManager.AppSettings["smtpFromEmail"].ToString();
                string smtpMailPassword = ConfigurationManager.AppSettings["smtpMailPassword"].ToString();
                string smtpClient = ConfigurationManager.AppSettings["smtpClient"].ToString();
                MailMessage msg = new MailMessage();
                msg.To.Add(emailTo);
                //msg.To.Add("keshav.kashyap@karam.in");
                string CC = CC_EMail + "," + ConfigurationManager.AppSettings["CC"].ToString();
                //string CC = "dimpal.gupta@samparksoftwares.com"; 
                msg.CC.Add(CC.Trim());
                msg.From = new MailAddress(smtpFromEmail,"NON CONFORMANCE REPORT");
                msg.Subject = subject;
                msg.Body = emailBody;
                msg.IsBodyHtml = blnIsHtml;
                msg.Attachments.Add(attachment);
                /******** Using Gmail Domain ********/
                SmtpClient client = new SmtpClient(smtpClient, 587);
                //SmtpClient client = new SmtpClient(smtpClient, 2525);
                client.Credentials = new System.Net.NetworkCredential(smtpFromEmail, smtpMailPassword);
                //client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Send(msg);         // Send our email.
                msg = null;
                return 1;
            }
            catch (Exception exc)
            {
                return -1;
            }
        }
        //-------------26may-----
    }
    public class MsgResponse
    {
        public static String Message(Int32 responseType)
        {
            if (responseType == 0)
                return "We hit a snag, please try again after some time.";
            else if (responseType == 1)
                return "Success";
            else if (responseType == 2)
                return "Retrieved successfully";
            else if (responseType == -5)
                return "Authentication failed";
            else if (responseType == -3)
                return "Email already exists!";
            else if (responseType == -4)
                return "Cannot process!";
            else if (responseType == -1)
                return "Technical error";
            else if (responseType == -2)
                return "No record found";
            else if (responseType == -16)
                return "The old password you have entered is incorrect";
            else if (responseType == 11)
                return "Data saved successfully";
            else if (responseType == 12)
                return "Data updated successfully";
            else if (responseType == 13)
                return "Data deleted successfully";
            else if (responseType == 14)
                return "Payment card set as primary successfully";
            else if (responseType == 15)
                return "Payment processed successfully";
            else if (responseType == 16)
                return "Your password has been changed successfully";
            else if (responseType == 17)
                return "Successfully disabled!!";
            else if (responseType == 18)
                return "Your account has been re-activated.";
            else if (responseType == 19)
                return "We have recieved your account delete request. Your account will be deleted with in 7 days. If you want you can re-activate your account with in 7 days";
            else if (responseType == 20)
                return "Data archived successfully";
            else if (responseType == 21)
                return "We have received your message and would like to thank you for writing to us. we will reply by phone / email as soon as possible.";
            else if (responseType == 23)
                return "Record already exist!";
            else if (responseType == 24)
                return "Data not available, please change your search criteria !";
            else if (responseType == 25)
                return "Your transaction is under process!";
            else if (responseType == 26)
                return "Your refund is successfull!";
            else if (responseType == 27)
                return "The credit card has expired";
            else if (responseType == -28)
                return "SubActivity can not be same.";
            else if (responseType == 29)
                return "Forwarded Successfully.";
            else if (responseType == 30)
                return "Closed Successfully.";
            else if (responseType == 31)
                return "Approved Successfully.";
            else
                return "We hit a snag, please try again after some time.";
        }
    }
}
