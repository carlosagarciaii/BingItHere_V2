using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet.Models;
using XnLogger;

namespace CoreToolSet.Models
{
    public class TableOutputItem
    {
        public List<TableListItem> TableData = new List<TableListItem>();

        private XnLogger.Logger logger = new Logger();
        private string LogMsg;
        public TableOutputItem(List<TableListItem> tableData)
        {
            TableData = tableData;

        }


        public string GetTable()
        {
            string funcName = "GetTable";
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
