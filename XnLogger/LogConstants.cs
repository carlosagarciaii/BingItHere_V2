using System;
using System.Collections.Generic;
using System.Text;
using XnLogger.Model;

namespace XnLogger
{
    public class LogConstants
    {


        // ----------------------------------------------------------------------
        // LOGGING CONSTANTS
        // ----------------------------------------------------------------------
        public const string LOGFILE_NAME = "_LogFile.log";
        public const string LOGFILE_FOLDER_NAME = "LogFiles";
        public const int MAX_LOGFILE_SIZE = 50000000;   //50MB -> 50,000,000
        public static readonly XnLogger.Model.LogLevel LOG_CRITICAL = new LogLevel("Critical", 1);
        public static readonly XnLogger.Model.LogLevel LOG_CRIT = LOG_CRITICAL;
        public static readonly XnLogger.Model.LogLevel LOG_ERROR = new LogLevel("Error", 2);
        public static readonly XnLogger.Model.LogLevel LOG_ERR = LOG_ERROR;
        public static readonly XnLogger.Model.LogLevel LOG_WARNING = new LogLevel("Warning", 3);
        public static readonly XnLogger.Model.LogLevel LOG_WARN = LOG_WARNING;
        public static readonly XnLogger.Model.LogLevel LOG_INFO = new LogLevel("Info", 4);
        public static readonly XnLogger.Model.LogLevel LOG_DEBUG = new LogLevel("Debug", 5);
        public static readonly XnLogger.Model.LogLevel LOG_TRACE = new LogLevel("Trace", 7);


    }
}
