using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using XnLogger.Model;
using XnLogger;


namespace XnLogger
{
    public class Logger
    {
        private object p;

        private XnLogger.Model.LogLevel HighestLogLevel { get; set; }
        private string LogFileName { get; set; }


        /// <summary>
        /// Constructor for Logger
        /// <para>-- highestLogLevel2Set = The highest Log Level to set for the Logger Instance. 
        /// <br> --- --- NOTE: All levels lower than the set level will be ignored by this Logger Instance</br>
        /// <br>-- logFileName = The name for the LogFile. (Default defined by LogConstants.LOGFILE_NAME) </br></para>
        /// </summary>
        /// <param name="highestLogLevel2Set"></param>
        /// <param name="logFileName"></param>
        public Logger(XnLogger.Model.LogLevel highestLogLevel2Set = null, string logFileName = LogConstants.LOGFILE_NAME)
        {
            HighestLogLevel = (highestLogLevel2Set == null) ? LogConstants.LOG_INFO : highestLogLevel2Set;
            LogFileName = logFileName;

        }

        public Logger(object p, string logFileName)
        {
            this.p = p;
            LogFileName = logFileName;
        }

        /// <summary>
        /// Writes to the LogFile and Console.
        /// <para>- message = the message to output
        /// <br>- functionName = The name of the function to tag in the log file</br>
        /// <br>- severityLevel = The severity of the message</br></para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="functionName"></param>
        /// <param name="severityLevel"></param>
        public void Write(string message, string functionName, XnLogger.Model.LogLevel severityLevel = null)
        {
            if (severityLevel == null)
            {
                severityLevel = LogConstants.LOG_INFO;
            }

            string LogMessage = $"{GetTimeStamp()}  |  {severityLevel.Name}  |  {functionName}  |  {message}";
            string targetFile = $"{GetWorkingDir()}/{LogConstants.LOGFILE_FOLDER_NAME}/{LogFileName}";
            string logFileFolder = $"{ GetWorkingDir()}/{LogConstants.LOGFILE_FOLDER_NAME}";

            string writeMessage = $"SANITY CHECK---------------\n\tLogMessage:\t{LogMessage}\n\ttargetFile:\t{targetFile}\n\tlogFileFolder:\t{logFileFolder}";
            Console.Write(writeMessage);

            if (HighestLogLevel.Value >= severityLevel.Value)
            {

                //Find or Create Logfile Directory

                if (!Directory.Exists(logFileFolder))
                {
                    Directory.CreateDirectory(logFileFolder);
                    Thread.Sleep(3000);
                }


                // Set Logfile
                FileInfo logFile = new FileInfo(targetFile);


                // Backup Logfile & Delete Original If Too Big
                if (logFile.Exists && logFile.Length > LogConstants.MAX_LOGFILE_SIZE)
                {
                    string targetNewFile = $"{logFileFolder}/{GetTimeStamp(false)}_{LogConstants.LOGFILE_NAME}";
                    if (File.Exists(targetNewFile)) { }
                    logFile.CopyTo(targetNewFile);
                    Thread.Sleep(2000);
                    logFile.Delete();
                    Thread.Sleep(5000);
                }

                //Create New Logfile if Does Not Exist
                //ALSO, Write to Logfile
                if (!logFile.Exists)
                {
                    using (StreamWriter streamWriter = logFile.CreateText())
                    {
                        streamWriter.Write($"{LogMessage}\n");
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                }
                else
                {
                    using (StreamWriter streamWriter = logFile.AppendText())
                    {
                        streamWriter.Write($"{LogMessage}\n");
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                Console.WriteLine(LogMessage);
            }
            if (severityLevel == LogConstants.LOG_CRITICAL)
            {

                throw new Exception(LogConstants.LOG_CRITICAL.Name + LogMessage);
            }

        }


        /// <summary>
        /// Gets the Current Date/Time and Creates a TimeStamp
        /// </summary>
        /// <param name="returnFullTimeStamp"></param>
        /// <returns></returns>
        private string GetTimeStamp(bool returnFullTimeStamp = true)
        {
            string outDate = "";
            string outTime = "";
            DateTime now = DateTime.Now;

            string Year = "0000" + now.Year.ToString();
            Year = Year.Substring(Year.Length - 4, 4);

            string Month = "00" + now.Month.ToString();
            Month = Month.Substring(Month.Length - 2, 2);

            string Day = "00" + now.Day.ToString();
            Day = Day.Substring(Day.Length - 2, 2);

            string Hour = "00" + now.Hour.ToString();
            Hour = Hour.Substring(Hour.Length - 2, 2);

            string Minute = "00" + now.Minute.ToString();
            Minute = Minute.Substring(Minute.Length - 2, 2);

            string Second = "00" + now.Second.ToString();
            Second = Second.Substring(Second.Length - 2, 2);

            string Millisecond = "0000" + now.Millisecond.ToString();
            Millisecond = Millisecond.Substring(Millisecond.Length - 4, 4);

            outDate = $"{Year}{Month}{Day}";
            outTime = (returnFullTimeStamp) ? $"-{Hour}:{Minute}:{Second}.{Millisecond}" : "";
            return outDate + outTime;

        }


        /// <summary>
        /// Returns the Current Working Directory
        /// <para>May be redundant.... not sure</para>
        /// </summary>
        /// <returns></returns>
        private string GetWorkingDir()
        {
            string outString = System.IO.Directory.GetCurrentDirectory();
            return outString;
        }

    }
}
