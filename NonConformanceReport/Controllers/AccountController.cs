using FPDAL.Data;
using NCRBAL.Business;
using NCRBAL.Interface;
using NCRMODEL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NonConformanceReport.Models;

namespace NonConformanceReport.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        IAccount login = new AccountAccess();
        public ActionResult Index()
        {
            LoginModel model = new LoginModel();
            model.EncryptedUserId = Function.ReadCookie("EncryptedUserId");

            return View(model);
        }
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (model.UserID != "" && model.Password == "123456")
            {
                switch (model.UserID.ToUpper())
                {
                    case "QUALITY CONTROL1":
                        Session["UNIT"] = "NCR_GENERATED_UNIT1";
                        return RedirectToAction("GenerateNCR", "NCR", false);
                    
                    case "QUALITY CONTROL2":
                        Session["UNIT"] = "NCR_GENERATED";
                        return RedirectToAction("GenerateNCR", "NCR", false);
                        
                    default:
                        
                        return RedirectToAction("index", "Account", false);
                      
                }
            }
            ViewBag.Message = "Invalid Email or Password.";
            return View(model);
        }
        public ActionResult LogOut()
        {
            //Function.DeleteCookie("EncryptedUserId");
            //Function.DeleteCookie("FP_LoggedUserName");
            //Function.DeleteCookie("FP_LoggedUserType");
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "account");
        }
    }
}