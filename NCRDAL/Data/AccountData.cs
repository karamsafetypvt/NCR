using FPDAL.Data;
using Infotech.ClassLibrary;
using NCRMODEL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCRDAL.Data
{
    public class AccountData : ConnectionObjects
    {
        public DataSet Login(LoginModel request, out int returnResult)
        {
            returnResult = 0;
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();

            //returnResult = 0;
            //DataSet ds = new DataSet();
            //try
            //{
            //    SqlParameter[] parameters ={

            //                                new SqlParameter("@UserId",request.UserID),
            //                                //new SqlParameter("@Timezone",request.Timezone??0),
            //                            };
            //    ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.PPR_UserLogin", parameters);
            //}
            //catch (Exception ex)
            //{
            //    returnResult = -1;
            //    ApplicationLogger.LogError(ex, "AccountData", "Login");
            //}

            //return ds;

           
            try
            {
                ds1 = SqlHelper.ExecuteDataset(masterConnectionString, CommandType.Text, "select * from Master_Requestor where DepartmentID in (9,37)");

                DataTable dt = new DataTable();
                dt = AddNewTableData2(ds1);

                dt.TableName = "tblItem";
                DataSet myDataSet = new DataSet("myDataSet");
                myDataSet.Tables.Add(dt);
                string constr = ConfigurationManager.ConnectionStrings["DBPPR"].ConnectionString;
                SqlConnection con = new SqlConnection(constr);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand("dbo.PPR_UserLogin", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@UserId", request.UserID));
                cmd.Parameters.Add("@activitydata", SqlDbType.Xml).Value = (myDataSet == null ? null : myDataSet.GetXml());
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
               
            }
            catch (Exception ex)
            {
                returnResult = -1;
                ApplicationLogger.LogError(ex, "AccountData", "Login");
            }

            return ds;
        }

        public static DataTable AddNewTableData2(DataSet ds)
        {
            DataTable dtSetupSeriesRow = new DataTable();
            dtSetupSeriesRow = new DataTable();
            dtSetupSeriesRow.Columns.Add("Employee", typeof(System.String));
            dtSetupSeriesRow.Columns.Add("UserName", typeof(System.String));
            dtSetupSeriesRow.Columns.Add("Email", typeof(System.String));
            dtSetupSeriesRow.Columns.Add("Mobileno", typeof(System.String));
            dtSetupSeriesRow.Columns.Add("Password", typeof(System.String));
            try
            {
                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    //foreach (var item in ds.Tables)
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var item = ds.Tables[0].Rows[i]["EMPLOYEE_ID"] ?? null;
                        //models.ProjectID = item.ProjectID;
                        DataRow dtRow = dtSetupSeriesRow.NewRow();
                        dtRow["Employee"] = ds.Tables[0].Rows[i]["EMPLOYEE_ID"] ?? null;
                        dtRow["UserName"] = ds.Tables[0].Rows[i]["NAME"] ?? null;
                        dtRow["Email"] = ds.Tables[0].Rows[i]["Email ID"] ?? null;
                        dtRow["Mobileno"] = ds.Tables[0].Rows[i]["MOBILE NUMBER"] ?? null;
                        dtRow["Password"] = ds.Tables[0].Rows[i]["Password"] ?? null;
                        dtSetupSeriesRow.Rows.Add(dtRow);
                    }
                }
                return dtSetupSeriesRow;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "DevelopmentPlanAccess", "AddNewTableData2");
                return null;
            }

        }


        public DataSet CheckUserEmail(LoginModel request, out int returnResult)
        {
            returnResult = 0;
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters ={

                                            new SqlParameter("@UserId",request.UserID),
                                            new SqlParameter("@Email",request.Email),
                                            new SqlParameter("@returnResult", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, System.String.Empty, DataRowVersion.Default, null),
                                        };
                ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.Sp_CheckUseeEmail", parameters);
                returnResult = Convert.ToInt32(parameters[2].Value);
                return ds;
            }
            catch (Exception ex)
            {
                returnResult = -1;
                ApplicationLogger.LogError(ex, "AccountData", "CheckUserEmail");
                return ds;
            }
            finally
            {
            }
        }
        public DataSet ChangeUserPassword(LoginModel request, out int returnResult)
        {
            returnResult = 0;
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters ={

                                            new SqlParameter("@UserId",request.UserID),
                                            new SqlParameter("@Email",request.Email),
                                            new SqlParameter("@Password",request.Password),
                                            new SqlParameter("@returnResult", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, System.String.Empty, DataRowVersion.Default, null),
                                        };
                ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.Sp_PPRChangeUserPassword", parameters);
                returnResult = Convert.ToInt32(parameters[3].Value);
                return ds;
            }
            catch (Exception ex)
            {
                returnResult = -1;
                ApplicationLogger.LogError(ex, "AccountData", "ChangeUserPassword");
                return ds;
            }
            finally
            {
            }
        }
    }
}
