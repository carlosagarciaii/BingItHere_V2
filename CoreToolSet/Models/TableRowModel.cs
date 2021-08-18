using System;
using System.Collections.Generic;
using System.Text;
using XnLogger;



namespace CoreToolSet.Models
{
    public class TableRowModel
    {
        private string ClassName = "TableRowModel";
        public List<string> RowData = new List<string>();
        public bool IsHeaderRow = false;

        private XnLogger.Logger logger = new Logger();
        private string LogMsg;



        public TableRowModel()
        {
            string funcName = $"{ClassName}.TableRowModel [No Args]";

            try
            {
                RowData.Clear();
                LogMsg = $"{funcName} instantiated";
                logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);

            }
            catch (Exception e)
            {
                LogMsg = $"Unable to Instantiate {funcName}\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }


        }
        public TableRowModel(List<string> rowData,bool isHeaderRow = false)
        {
            string funcName = $"{ClassName}.TableRowModel [rowData, isHeader]";
            try
            {
                IsHeaderRow = isHeaderRow;
                RowData.Clear();
                foreach (string rowItem in rowData)
                {
                    RowData.Add(rowItem);
                }
                LogMsg = $"{funcName} instantiated with data";
                logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
            }
            catch (Exception e)
            {
                LogMsg = $"Unable to Instantiate {funcName}\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }
        }

        public TableRowModel(bool isHeaderRow)
        {
            string funcName = $"{ClassName}.TableRowModel [isHeaderRow]";
            try
            {
                IsHeaderRow = isHeaderRow;
                RowData.Clear();
                LogMsg = $"{funcName} instantiated";
                logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
            }
            catch (Exception e)
            {
                LogMsg = $"Unable to Instantiate {funcName}\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }
        }


        /// <summary>
        /// Adds a single item to the RowData
        /// </summary>
        /// <param name="item2Add"></param>
        public void AddItem(string item2Add)
        {
            string funcName = $"{ClassName}.AddItem";
            try
            {
                RowData.Add(item2Add);
            }
            catch (Exception e)
            {
                LogMsg = $"Could not add item.\n{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }


        }

        /// <summary>
        /// Appends a List to the RowData
        /// </summary>
        /// <param name="items2Add"></param>
        public void AddItem(List<string> items2Add)
        {
            string funcName = $"{ClassName}.AddItem (List)";
            try
            {
                foreach (string curItem in items2Add)
                {
                    RowData.Add(curItem);
                }
            }
            catch (Exception e)
            {
                LogMsg = $"Could Not Add a List Item.{e}";
                logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
                throw new Exception(LogMsg);
            }
        }

    }
}
