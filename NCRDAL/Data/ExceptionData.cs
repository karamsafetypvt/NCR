using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Infotech.ClassLibrary;
using NCRMODEL.Model;
//using PPR.Models;
namespace PPRDAL.Data
{
    public class ExceptionData : ConnectionObjects
    {
        static String _error;

        /// <summary>
        /// This function is used to store any exception occured in application into Exception datatable.
        /// In this function store procedure "ims.IMS_ExceptionsInsert" is used to store exception detail.
        /// 
        /// </summary>
        /// <param name="model">Passing ExceptionModel property values related to exception.</param>
        /// <returns>
        /// It returns status 0/1 as per 
        /// </returns>
       /* public static Int32 ExceptionAdd(ExceptionModel model)
            {
            Int32 myInt = 1;
                SqlParameter[] parameters ={
                                            new SqlParameter("@ExceptionId", model.ExceptionId??0),
                                            new SqlParameter("@ExceptionType",model.ExceptionType),
                                            new SqlParameter("@BaseType",model.BaseType) ,
                                            new SqlParameter("@Title",model.Title) ,
                                            new SqlParameter("@Message",model.Message) ,
                                            new SqlParameter("@StackTrace",model.StackTrace) ,
                                            new SqlParameter("@HelpLink",model.HelpLink) ,
                                            new SqlParameter("@Class",model.Class) ,
                                            new SqlParameter("@Method",model.Method) ,
                                            new SqlParameter("@UserId ",model.UserId ),
                                            new System.Data.SqlClient.SqlParameter("ReturnValue", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, 0, 0, System.String.Empty, System.Data.DataRowVersion.Default, null),
                                        };
                try
                {

                myInt = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "ims.IMS_ExceptionsInsert", parameters);
                    return myInt;
                }
                catch (Exception ex)
                {
                    _error = ex.Message;                  
                    return 1;
                }
                finally
                {
                }
            }*/


        /// <summary>
        /// This function is used to store any exception occured in application into Exception datatable.
        /// In this function store procedure "ims.IMS_ExceptionsInsert" is used to store exception detail.
        /// 
        /// </summary>
        /// <param name="model">Passing ExceptionModel property values related to exception.</param>
        /// <returns>
        /// It returns status 0/1 as per 
        /// </returns>
        public static Int32 LogToDatabase(Exception ex, String Class, String Method)
        {
            Int32 myInt = 1;
            SqlParameter[] parameters ={
                                            //new SqlParameter("@ExceptionId", 0),
                                            new SqlParameter("@ExceptionType",ex.GetType().ToString()),
                                            new SqlParameter("@BaseType",ex.GetType().BaseType.ToString()) ,
                                            new SqlParameter("@Title",ex.HResult.ToString()) ,
                                            new SqlParameter("@Message",ex.Message) ,
                                            new SqlParameter("@StackTrace",ex.StackTrace) ,
                                            new SqlParameter("@HelpLink",ex.HelpLink) ,
                                            new SqlParameter("@Class",Class) ,
                                            new SqlParameter("@Method",Method) ,
                                            //new SqlParameter("@LoggedUserId",Function.ReadCookie("EncryptedUserId")) ,
                                            new SqlParameter("@LoggedUserId",1) ,
                                            new System.Data.SqlClient.SqlParameter("ReturnValue", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, 0, 0, System.String.Empty, System.Data.DataRowVersion.Default, null),
                                        };
            try
            {

                myInt = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "dbo.PPR_ExceptionsInsert", parameters);
                return myInt;
            }
            catch (Exception ex1)
            {
                _error = ex1.Message;
                return -1;
            }
            finally
            {
            }
        }

        public static Int32 LogToFile(Exception ex, String Class, String Method)
        {
            try
            {
                String filename = "";//HttpContext.Current.Server.MapPath("~/App_Data/ErrorLog.txt");
                if (!File.Exists(filename))
                    File.Create(filename);

                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine("===================================================================================================");
                sw.WriteLine("Date :" + DateTime.Now);
                sw.WriteLine("ExceptionType :" + ex.GetType().ToString());
                sw.WriteLine("BaseType :" + ex.GetType().BaseType.ToString());
                sw.WriteLine("Title :" + ex.HResult.ToString());
                sw.WriteLine("Message :" + ex.Message);
                sw.WriteLine("StackTrace :" + ex.StackTrace);
                sw.WriteLine("HelpLink :" + ex.HelpLink ?? "");
                sw.WriteLine("Class :" + Class);
                sw.WriteLine("Method :" + Method);
                sw.WriteLine("===================================================================================================");
                sw.Flush();
                return 1;
            }
            catch (Exception ex1)
            {
                _error = ex1.Message;
                return 0;
            }
            finally
            {
            }
        }


        /// <summary>
        /// Log application exceptions into database.
        /// </summary>
        public class DatabaseLogger : ILogger
        {
            public void WriteException(Exception ex, String Class, String Method)
            {
                ExceptionData.LogToDatabase(ex, Class, Method);
            }
        }
    }
}
