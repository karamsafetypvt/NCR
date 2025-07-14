using System;

namespace NCRMODEL.Model
{
    public interface IException
    {
        Nullable<Int32> ExceptionId { get; set; }
        String ExceptionType { get; set; }
        String BaseType { get; set; }
        String Title { get; set; }
        String Message { get; set; }
        String StackTrace { get; set; }
        String HelpLink { get; set; }

        String Class { get; set; }
        String Method { get; set; }
        String SysDate { get; set; }
        Int32 UserId { get; set; }

        void ExceptionAdd(Exception ex, String Class, String Method);
    }

    public interface ILogger
    {
        // void WriteException(Exception ex);
        void WriteException(Exception ex, String ClassName, String MethodName);
    }
}
