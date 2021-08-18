using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet.Models;
using XnLogger;

namespace CoreToolSet.Models
{
    public class TableOutputItem
    {
        private string ClassName = "TableOutputItem";
        public List<TableRowItem> TableData = new List<TableRowItem>();

        private XnLogger.Logger logger = new Logger();
        private string LogMsg;



        public TableOutputItem()
        {
            string funcName = $"{ClassName}.TableOutputItem";
            LogMsg = $"{funcName} instantiated.";
            logger.Write(LogMsg,funcName,LogConstants.LOG_INFO);
        }
        public TableOutputItem(List<TableRowItem> tableData)
        {
            string funcName = $"{ClassName}.TableOutputItem";
            try
            {
                LogMsg = $"{funcName} instantiated.";
                logger.Write(LogMsg, funcName, LogConstants.LOG_INFO);
                TableData = tableData;
            }
            catch (Exception e)
            {
                LogMsg = $"{funcName} could not be instantiated.\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }

        }

        public void AddRow(TableRowItem TableRowItem)
        {
            string funcName = $"{ClassName}.AddRow";

            try
            {
                TableData.Add(TableRowItem);
            }
            catch (Exception e)
            {
                LogMsg = $"An error has occurred while attempting to add a record.\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }
        }

        public string GetTable()
        {
            string funcName = $"{ClassName}.GetTable";
            string lineText = "";
            string outString = "";

            try
            {
                foreach (var lineItemText in TableData)
                {
                    lineText = "|";
                    foreach (var cellItemText in lineItemText.RowData)
                    {
                        lineText += cellItemText;
                    }
                    lineText += "\n";
                    outString += lineText;

                }

                return outString;
            }

            catch (Exception e)
            {
                LogMsg = $"An unhandled Exception occurred.\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }


        }

    }
}
