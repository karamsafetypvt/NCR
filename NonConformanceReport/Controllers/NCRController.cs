using NCRBAL.Business;
using NCRBAL.Interface;
using NCRMODEL.Model;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using FPDAL.Data;
using PPR.Controllers;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Web.UI;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NonConformanceReport.Controllers
{
    public class NCRController : BaseController
    {
        // GET: NCR
        INCR ncr = new NCRAccess();
        public ActionResult GenerateNCR()
        {
            string PettyID = "";
            NCRModel model = new NCRModel();
            model.MRNNoList = UtilityAccess.DRPMRNLIST();
            model.ItemCodeList = UtilityAccess.DRPMRNLIST();
            model.NCRNumber = Count(PettyID);
            model.NCRDate = DateTime.Today.ToShortDateString();
            return View(model);
        }
        [HttpGet]
        public bool ValidateEmails(string emails)
        {
            // Split the input string into individual email addresses
            string[] emailAddresses = emails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Define the regular expression pattern for a valid email address
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Use Regex.IsMatch to check if each email address matches the pattern
            foreach (string email in emailAddresses)
            {
                if (!Regex.IsMatch(email.Trim(), pattern))
                {
                    return false; // At least one email address is invalid
                }
            }

            return true; // All email addresses are valid
        }


        [HttpPost]
        public ActionResult GenerateNCR(NCRModel model, FormCollection data)
        {
            NCRModel ddsm = new NCRModel();
            String filePath = "";
            model.TableName = Session["Unit"].ToString();
            HttpPostedFileBase httpPostedFileBase = null;
            int count = default(int);
            int i = 0;
             
            foreach (var item in Request.Files)
            {
                //save profile pic
                httpPostedFileBase = Request.Files[i] as HttpPostedFileBase;
                if (httpPostedFileBase != null && httpPostedFileBase.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(httpPostedFileBase.FileName);
                    string ext = Path.GetExtension(httpPostedFileBase.FileName);
                    if (model.TableName == "NCR_GENERATED_UNIT1")
                    {
                        filePath = "~/Upload/UNIT1/";
                    }
                    else
                    {
                        filePath = "~/Upload/UNIT2/";
                    }
                    if (model.RefrenceImage1 != null)
                    {
                        model.ImagePath1 = filePath + fileName;
                        model.ImageName1 = fileName;
                        if (System.IO.File.Exists(Server.MapPath(model.ImagePath1[i].ToString())))
                        {
                            System.IO.File.Delete(Server.MapPath(model.ImagePath1));
                        }
                        if (!System.IO.Directory.Exists(Server.MapPath(filePath)))
                            System.IO.Directory.CreateDirectory(Server.MapPath(filePath));
                        httpPostedFileBase.SaveAs(Server.MapPath(filePath) + "/" + fileName);
                        model.RefrenceImage1 = null;
                    }
                    else if (model.RefrenceImage2 != null)
                    {
                        model.ImagePath2 = filePath + fileName;
                        model.ImageName2 = fileName;
                        if (System.IO.File.Exists(Server.MapPath(model.ImagePath2[i].ToString())))
                        {
                            System.IO.File.Delete(Server.MapPath(model.ImagePath2));
                        }
                        if (!System.IO.Directory.Exists(Server.MapPath(filePath)))
                            System.IO.Directory.CreateDirectory(Server.MapPath(filePath));
                        httpPostedFileBase.SaveAs(Server.MapPath(filePath) + "/" + fileName);
                        model.RefrenceImage2 = null;
                    }
                    else if (model.RefrenceImage3 != null)
                    {
                        model.ImagePath3 = filePath + fileName;
                        model.ImageName3 = fileName;
                        if (System.IO.File.Exists(Server.MapPath(model.ImagePath3[i].ToString())))
                        {
                            System.IO.File.Delete(Server.MapPath(model.ImagePath3));
                        }
                        if (!System.IO.Directory.Exists(Server.MapPath(filePath)))
                            System.IO.Directory.CreateDirectory(Server.MapPath(filePath));
                        httpPostedFileBase.SaveAs(Server.MapPath(filePath) + "/" + fileName);
                        model.RefrenceImage3 = null;
                    }
                    else if (model.RefrenceImage4 != null)
                    {
                        model.ImagePath4 = filePath + fileName;
                        model.ImageName4 = fileName;
                        if (System.IO.File.Exists(Server.MapPath(model.ImagePath4[i].ToString())))
                        {
                            System.IO.File.Delete(Server.MapPath(model.ImagePath4));
                        }
                        if (!System.IO.Directory.Exists(Server.MapPath(filePath)))
                            System.IO.Directory.CreateDirectory(Server.MapPath(filePath));
                        httpPostedFileBase.SaveAs(Server.MapPath(filePath) + "/" + fileName);
                        model.RefrenceImage4 = null;
                    }
                }
                i++;
            }
            ddsm = ncr.AddOrEdit(model);
            if (ddsm != null)
            {
                TempData["ReturnMessage"] = ddsm.ReturnMessage;
                TempData["ReturnCode"] = ddsm.ReturnCode;
                return RedirectToAction("GenerateNCR", "NCR");
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetNCRData(string POCode)
        {
            NCRModel model = new NCRModel();
            NCRModel result = new NCRModel();
            model.PO = POCode.ToString();
            result = ncr.DataByPOID(model);
            if (result != null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(model);
            }
        }
        [HttpPost]
        public JsonResult GetNCRItemData(string MRNCode, string PONumber)
        {
            NCRModel model = new NCRModel();
            NCRModel result = new NCRModel();
            model.MRNno = MRNCode.ToString();
            model.PO = PONumber.ToString();
            result = ncr.DataByMRNID(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetItemDescription(string MRNCode, string PONumber, string ItemCode)
        {
            NCRModel model = new NCRModel();
            NCRModel result = new NCRModel();
            model.MRNno = MRNCode.ToString();
            model.PO = PONumber.ToString();
            model.ItemCode = ItemCode.ToString();
            result = ncr.GetItemDescription(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewNCRData()
        {
            NCRModel result = new NCRModel();
            result = ncr.GetNCRData();
            return View(result);
        }
        public string Count(string PettyID)
        {
            PettyID = ""; int? ncr1 = null;
            string f1 = ""; string f2 = "";
            try
            {
                string count1 = ncr.NCRCountt();
                string[] FY = GetCurrentFinancialYear().ToString().Split('-', ' ');
                f1 = FY[0].Substring(2, 2); f2 = FY[1].Substring(2, 2);
                ncr1 = Convert.ToInt32(count1) + 1;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRController", "Count");
            }
            return PettyID = ncr1 + "(" + f1 + "-" + f2 + ")";
        }

        public static string GetCurrentFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;
            return FinYear.Trim();
        }
        public JsonResult btnRefresh_Click()
        {
            string result;
            try
            {
                result = ncr.NCRDataRefresh();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRController", "btnRefresh_Click");
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        //public void Download(string NCRNumber)
        //{ 
        //    string filePath  = "";
        //    try
        //    {
        //        if (Session["Unit"].ToString() == "NCR_GENERATED_UNIT1")
        //        {
        //            filePath = "~/PDF/UNIT1/";
        //        }
        //        else
        //        {
        //            filePath = "~/PDF/UNIT2/";
        //        }
        //        string id = NCRNumber;
        //        Response.ContentType = "Application/pdf";
        //        Response.AppendHeader("Content-Disposition", "attachment; filename=" + id + ".pdf");
        //        Response.TransmitFile(Server.MapPath(filePath + id + ".pdf"));
        //        Response.End();
        //        //Response.Headers.Clear();
        //        //Response.Redirect("ViewNCRData");
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLogger.LogError(ex, "NCRController", "Download");
        //        TempData["ReturnMessage"] = "PDF is not found.";
        //        TempData["ReturnCode"] = 99;
        //        Response.Redirect("ViewNCRData");
        //    }

        //}
        public ActionResult Download(string NCRNumber)
        {
            string filePath = "";
            try
            {
                if (Session["Unit"].ToString() == "NCR_GENERATED_UNIT1")
                {
                    filePath = "~/Upload/UNIT1/";
                }
                else
                {
                    filePath = "~/Upload/UNIT2/";
                }
                string id = NCRNumber;
                string fileName = id + ".pdf";
                string fullPath = Server.MapPath(filePath + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    // Return the file as a FileResult
                    TempData["file"] = "tue";
                    return File(fullPath, "application/pdf", fileName);
                }
                else
                {
                    TempData["ReturnMessage"] = "PDF is not found.";
                    TempData["ReturnCode"] = 99;
                    return RedirectToAction("ViewNCRData", "NCR"); // Redirect to some error page or another action
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRController", "Download");
                TempData["ReturnMessage"] = "An error occurred while processing the request.";
                TempData["ReturnCode"] = 99;
                return RedirectToAction("ViewNCRData", "NCR"); // Redirect to some error page or another action
            }
        }
        public ActionResult Sendmail(string NCRNumber, string Email)
        {
            NCRModel result = new NCRModel();
            result = ncr.NCREmailDetail(NCRNumber, Email);
            if (result.ReturnCode == 12)
            {
                TempData["ReturnMessage"] = result.ReturnMessage;
                TempData["ReturnCode"] = result.ReturnCode;
                return RedirectToAction("ViewNCRData", "NCR");
            }
            else { return View(); }

        }
        public ActionResult Report()
        {
            ReportModel model = new ReportModel();
            model.MonthList = UtilityAccess.MonthList();
            int currentMonth = DateTime.Now.Month;
            model.Month = currentMonth.ToString().PadLeft(2, '0');
            model.YearList = UtilityAccess.YearList();
            string currentYR = DateTime.Now.Year.ToString();
            model.Year = currentYR.ToString();
            return View(model);
        }
        [HttpPost]
        public string Report(string Year, string Month)
        {
            ReportModel model = new ReportModel();
            model.Month = Month;
            model.Year = Year;
            ReportModel result = new ReportModel();
            result.reportModelsList = ncr.ReportDetail(model);
            var list = JsonConvert.SerializeObject(result.reportModelsList,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });
            return list;
        }
    }
}