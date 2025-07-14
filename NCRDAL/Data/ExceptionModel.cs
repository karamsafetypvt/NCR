using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NCRMODEL.Model;
using PPRDAL.Data;

namespace FPDAL.Data
{
    public enum LoggingType
    {
        All = 0,
        FileSystem = 1,
        Database = 2,
        WindowsEventLog = 3
    }

    public class ApplicationLogger
    {
        //static LoggingType logType = (LoggingType)Convert.ToInt32(ConfigurationManager.AppSettings["LoggingType"]);
        static LoggingType logType = (LoggingType)Convert.ToInt32(2);
        static ILogger logger;
        public static void LogError(Exception ex, String className, String methodName)
        {
            switch (logType)
            {
                case LoggingType.Database:
                    logger = new DatabaseLogger();
                    break;
                case LoggingType.FileSystem:
                    logger = new FileSystemLogger();
                    break;
                case LoggingType.WindowsEventLog:
                    logger = new WindowsEventLogger();
                    break;
                case LoggingType.All:
                    logger = new FileSystemLogger();
                    if (logger != null)
                        logger.WriteException(ex, className, methodName);
                    logger = new DatabaseLogger();
                    break;

            }

            if (logger != null)
                logger.WriteException(ex, className, methodName);

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


    /// <summary>
    /// Log application exceptions int text file.
    /// </summary>
    public class FileSystemLogger : ILogger
    {
        public void WriteException(Exception ex, String Class, String Method)
        {
            ExceptionData.LogToFile(ex, Class, Method);
        }
    }

    public class WindowsEventLogger : ILogger
    {
        public void WriteException(Exception ex, String Class, String Method)
        {

        }
    }
}

