using FPDAL.Data;
using NCRBAL.Interface;
using NCRDAL.Data;
using NCRMODEL.Model;
using PPRDAL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCRBAL.Business
{
    public class AccountAccess : IAccount
    {
        AccountData userData = new AccountData();
        public LoginRespone Login(LoginModel request)
        {
            LoginRespone serviceResponse = new LoginRespone();
            UserModel userInfo = new UserModel();
            serviceResponse.ReturnCode = "0";
            serviceResponse.ReturnMessage = "Login failed";
            serviceResponse.UserInfo = userInfo;
            int returnResult = 0;
            string Email = string.Empty, UserId = string.Empty;
            DataSet ds = userData.Login(request, out returnResult);

            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    returnResult = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"]);
                    if (returnResult > 0)
                    {
                        UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserId"]);
                        Email = Convert.ToString(ds.Tables[0].Rows[0]["Email"]);
                        string encryptedPassword = Convert.ToString(ds.Tables[0].Rows[0]["Password"]);
                        if (encryptedPassword == request.Password)
                        {
                            userInfo = UserInfo(ds, out returnResult);
                            if (returnResult > 0)
                            {
                                serviceResponse.ReturnCode = "1";
                                serviceResponse.ReturnMessage = "Login successfuly.";
                                serviceResponse.UserInfo = userInfo;
                            }

                            else if (returnResult == -1)
                            {
                                serviceResponse.ReturnCode = "-1";
                                serviceResponse.ReturnMessage = "Technical error.";

                            }
                            else if (returnResult == -3)
                            {
                                serviceResponse.ReturnCode = "0";
                                serviceResponse.ReturnMessage = "Invalid Email or Password.";
                            }
                        }
                        else
                        {
                            serviceResponse.ReturnCode = "0";
                            serviceResponse.ReturnMessage = "Invalid Email or Password.";
                        }
                    }
                    else if (returnResult == -3)
                    {
                        serviceResponse.ReturnCode = "0";
                        serviceResponse.ReturnMessage = "This user not exists";
                    }
                    else
                    {
                        serviceResponse.ReturnCode = "0";
                        serviceResponse.ReturnMessage = "Invalid Email or Password";
                    }
                }
                else if (returnResult == -1)
                {
                    serviceResponse.ReturnCode = "-1";
                    serviceResponse.ReturnMessage = "Technical error.";

                }
                else if (returnResult == -3)
                {
                    serviceResponse.ReturnCode = "0";
                    serviceResponse.ReturnMessage = "Invalid Email or Password.";
                }
                return serviceResponse;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "AccountAccess", "Login");
                return null;
            }

        }
        private UserModel UserInfo(DataSet ds, out int returnResult)
        {
            UserModel userInfo = new UserModel();
            returnResult = 1;
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            userInfo.UserId = Convert.ToInt32(row["UserId"]);
                            if (userInfo.UserId > 0)
                            {
                                returnResult = userInfo.UserId;
                                userInfo.FirstName = Convert.ToString(row["UName"]);
                                userInfo.CompanyId = Convert.ToInt32(row["CompanyId"]);
                                userInfo.Email = Convert.ToString(row["Email"]);
                                userInfo.Mobile = Convert.ToString(row["MobileNo"]);
                                userInfo.CreatedBy = Convert.ToString(row["CreatedBy"]);
                                userInfo.UserType = Convert.ToInt32(row["UserType"]);
                                userInfo.Level = Convert.ToInt32(row["Level"]);
                                userInfo.DSLevel = Convert.ToInt32(row["DSLevel"]);
                                userInfo.PNICode = Convert.ToString(row["EmployeeID"]);
                                userInfo.Password = Convert.ToString(row["Password"]);
                            }
                            else
                            {
                                returnResult = userInfo.UserId;
                            }

                        }

                    }
                }
                return userInfo;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "AccountAccess", "UserInfo");
                return null;
            }

        }

        //public LoginModel CheckUserEmail(LoginModel model)
        //{
        //    LoginModel serviceResponse = new LoginModel();
        //    int returnResult = 0;
        //    String _subject = "Password Change";
        //    DataSet ds = userData.CheckUserEmail(model, out returnResult);
        //    try
        //    {
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            returnResult = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"]);
        //            if (returnResult > 0)
        //            {
        //                serviceResponse.ReturnCode = returnResult;
        //                serviceResponse.OTP = UtilityAccess.OtpGenerator();
        //                String _body = "Dear User,The one time password (OTP) to reset your password is:" + serviceResponse.OTP;
        //                Int32 RetVal = UtilityAccess.SendEmail("PPR Team", model.Email, _subject, _body);
        //            }
        //        }
        //        else
        //        {
        //            serviceResponse.ReturnCode = 0;
        //            serviceResponse.ReturnMessage = "Invalid Email.";
        //        }
        //        return serviceResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLogger.LogError(ex, "AccountAccess", "CheckUserEmail");
        //        return null;
        //    }

        //}

        //public LoginModel ChangePassword(LoginModel model)
        //{
        //    LoginModel serviceResponse = new LoginModel();
        //    int returnResult = 0;
        //    DataSet ds = userData.ChangeUserPassword(model, out returnResult);
        //    try
        //    {
        //        if (returnResult > 0)
        //        {
        //            serviceResponse.ReturnCode = returnResult;
        //            serviceResponse.ReturnMessage = MsgResponse.Message(returnResult);
        //        }
        //        return serviceResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLogger.LogError(ex, "AccountAccess", "ChangePassword");
        //        return null;
        //    }

        //}
    }
}
