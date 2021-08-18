using System;
using System.Collections.Generic;
using System.Text;
using XnLogger;



namespace CoreToolSet.Models
{
    public class TableRowItem
    {
        private string ClassName = "TableRowItem";
        public List<string> RowData = new List<string>();
        public bool IsHeaderRow = false;

        private XnLogger.Logger logger = new Logger();
        private string LogMsg;


        public TableRowItem(List<string> rowData,bool isHeaderRow = false)
        {
            IsHeaderRow = isHeaderRow;
            RowData.Clear();
            foreach (string rowItem in rowData)
            {
                RowData.Add(rowItem);
            }
        }

        public TableRowItem(bool isHeaderRow)
        {
            IsHeaderRow = isHeaderRow;
            RowData.Clear();

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
