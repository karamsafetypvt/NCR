using FPDAL.Data;
using Infotech.ClassLibrary;
using NCRMODEL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using System.Web;
using System.Configuration;

namespace NCRDAL.Data
{
    public class NCRData : ConnectionObjects
    {
        public DataSet SelectAll()
        {
            DataSet myDataSet = null;
            SqlParameter[] parameters ={
                                            new SqlParameter("@TableName",HttpContext.Current.Session["Unit"].ToString()),
                                       };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.GetNCRData",parameters);
                //ReturnResult = Convert.ToInt32(parameters[3].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "SelectAll");
                return null;
            }
            
        }
        public DataSet DataByPOID(NCRModel model)
        {
            DataSet myDataSet = null;
            SqlParameter[] parameters ={
                                            new SqlParameter("@PONUmber",model.PO),
                                       };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.DataByPOID", parameters);
                //ReturnResult = Convert.ToInt32(parameters[3].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "DataByPOID");
                return null;
            }
            
        }
        public DataSet DataByMRNID(NCRModel model)
        {
            DataSet myDataSet = null;
            SqlParameter[] parameters ={
                                            new SqlParameter("@MRNNUmber",model.MRNno),
                                            new SqlParameter("@PONUmber",model.PO),
                                       };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.DataByMRNID", parameters);
                //ReturnResult = Convert.ToInt32(parameters[3].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "DataByMRNID");
                return null;
            }
            
        }
        public DataSet GetItemDescription(NCRModel model)
        {
            DataSet myDataSet = null;
            SqlParameter[] parameters ={
                                            new SqlParameter("@MRNNUmber",model.MRNno),
                                            new SqlParameter("@PONUmber",model.PO),
                                            new SqlParameter("@ItemNUmber",model.ItemCode),
                                       };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.GetItemDescription", parameters);
                //ReturnResult = Convert.ToInt32(parameters[3].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "DataByMRNID");
                return null;
            }
            
        }
        public DataSet AddorEdit(NCRModel entity, out int ReturnResult,out int NCRId)
        {
            DataSet myDataSet = null;
            ReturnResult = 0;
            NCRId = 0;
            SqlParameter[] parameters ={
                                             
             new SqlParameter("@returnResult", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, System.String.Empty, DataRowVersion.Default, null),
             new SqlParameter("@NCRId", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, System.String.Empty, DataRowVersion.Default, null),
             new SqlParameter("@Vendor", entity.VendorCode),
             new SqlParameter("@TableName", entity.TableName),
             new SqlParameter("@Ncr_No", entity.NCRNumber),
             new SqlParameter("@Contact", entity.Contact),
             new SqlParameter("@Phone", entity.Phone),
             new SqlParameter("@Address", entity.Address),
             new SqlParameter("@Email", entity.Email),
             new SqlParameter("@Description", entity.ItemDescription),
             new SqlParameter("@PO_Number", entity.PO),
             new SqlParameter("@Details", entity.DetailofNonConformance),
             new SqlParameter("@MRN", entity.MRNno),
             new SqlParameter("@Total_Received", entity.TotalRecieved),
             new SqlParameter("@Total_Reject", entity.TotalRejected),
            new SqlParameter("@Item_Code", entity.ItemCode),
            new SqlParameter("@file1", entity.ImageName1),
            new SqlParameter("@file2", entity.ImageName2),
            new SqlParameter("@file3", entity.ImageName3),
            new SqlParameter("@file4", entity.ImageName4),
            new SqlParameter("@filepath1", entity.ImagePath1),
            new SqlParameter("@filepath2", entity.ImagePath2),
            new SqlParameter("@filepath3", entity.ImagePath3),
            new SqlParameter("@filepath4", entity.ImagePath4),
            };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "dbo.New_sp_Generate_Unit1", parameters);
                ReturnResult = Convert.ToInt32(parameters[0].Value);
                NCRId = Convert.ToInt32(parameters[1].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "AddorEdit");
                ReturnResult = -1;
                return null;
            }
            
        }
        public string NCRCountt()
        {
            try
            {
                string TableName= HttpContext.Current.Session["Unit"].ToString();
                object ds1;
                if (TableName == "NCR_GENERATED_UNIT1")
                {
                    ds1 = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(id)+89 from " + TableName);//89 ADDED DUE TO MIGRATED FROM OLD DB
                }
                else
                {
                    ds1 = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "select count(id) from " + TableName);
                }
                return ds1.ToString();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "NCRCountt");
                return null;
            }
        }
        public String NCRDataRefresh()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NCRDB"].ConnectionString);
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("sp_NCR_Refresh", con);
                cmd.CommandTimeout = 3600;
                cmd.CommandType = CommandType.StoredProcedure;
                int ds1 = cmd.ExecuteNonQuery();
                return ds1.ToString();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "NCRDataRefresh");
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            //try
            //{
            //    int ds1 = SqlHelper.ExecuteNonQuery(ConnectionString,CommandType.StoredProcedure, "sp_NCR_Refresh");
            //    return ds1.ToString();
            //}
            //catch(Exception ex)
            //{
            //    ApplicationLogger.LogError(ex, "NCRData", "NCRDataRefresh");
            //    return null;
            //}
        }
        public DataSet DataForGeneratePDF(int NCRNumber) 
        {
            DataSet myDataSet = null;
            string TableName = HttpContext.Current.Session["Unit"].ToString();
            SqlParameter[] parameters ={
                                            new SqlParameter("@NCRNumber",NCRNumber),
                                       };
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select *,convert(varchar(20),Generated_date,103) as GenerateDate from " + TableName+" where id='" + NCRNumber + "'", parameters);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "DataForGeneratePDF");
                return null;
            }
        }
        public DataSet NCREmailDetail(string NCRNumber)
        {
            DataSet myDataSet = new DataSet();
            string TableName = HttpContext.Current.Session["Unit"].ToString();
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select * from "+ TableName + " where id=" + NCRNumber);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "NCREmailDetail");
                return null;
            }
        }
        public int UpdateNCREmailStatus(string NCRNumber,out int ReturnResult)
        {
            ReturnResult = 0;
            int myDataSet = 0;
            string TableName = HttpContext.Current.Session["Unit"].ToString();
            try
            {
                myDataSet = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "update "+TableName+ " set Status='Send',LastSendMailDate=getdate() where id=" + NCRNumber);
                //ReturnResult = Convert.ToInt32(parameters[0].Value);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "UpdateNCREmailStatus");
                return 0;
            }
        }
        public DataSet ReportDetail(ReportModel model)
        {
            DataSet myDataSet = null;
            string Month = model.Month;string Year = model.Year;String TableName = HttpContext.Current.Session["Unit"].ToString();
            try
            {
                myDataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "select FORMAT(Generated_date, 'dd-MM-yyyy hh:mm tt') as Generated_date,Ncr_No,VENDOR,Item_Code,cast((convert(bigint,[MRN])) as varchar(20)) as MRN,Details,Email,Description,PO_Number,Total_Received,Total_Reject,FORMAT(LastSendMailDate, 'dd-MM-yyyy hh:mm tt') as LastSendMailDate from " + TableName + " where (MONTH(Generated_date)= " + Month + " or " + Month + "=0) and (YEAR(Generated_date)=" + Year + " or " + Year + "=0)"); 
                return myDataSet;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ex, "NCRData", "ReportDetail");
                return null;
            }
        }
    }
}
