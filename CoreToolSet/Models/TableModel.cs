using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet.Models;
using XnLogger;

namespace CoreToolSet.Models
{
    public class TableModel
    {
        private string ClassName = "TableModel";
        public List<TableRowModel> TableData = new List<TableRowModel>();

        private XnLogger.Logger logger = new Logger();
        private string LogMsg;



        public TableModel()
        {
            string funcName = $"{ClassName}.TableModel";
            LogMsg = $"{funcName} instantiated.";
            logger.Write(LogMsg,funcName,LogConstants.LOG_DEBUG);
        }
        public TableModel(List<TableRowModel> tableData)
        {
            string funcName = $"{ClassName}.TableModel";
            try
            {
                LogMsg = $"{funcName} instantiated.";
                logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
                TableData = tableData;
            }
            catch (Exception e)
            {
                LogMsg = $"{funcName} could not be instantiated.\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }

        }

        public void AddRow(TableRowModel tableRowModel)
        {
            string funcName = $"{ClassName}.AddRow";

            try
            {
                TableData.Add(tableRowModel);
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
