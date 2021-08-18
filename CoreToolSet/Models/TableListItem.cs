using System;
using System.Collections.Generic;
using System.Text;

namespace CoreToolSet.Models
{
    public class TableListItem
    {

        public List<string> RowData = new List<string>();
        public bool IsHeaderRow = false;


        public TableListItem(List<string> rowData,bool isHeaderRow = false)
        {
            IsHeaderRow = isHeaderRow;
            RowData.Clear();
            foreach (string rowItem in rowData)
            {
                RowData.Add(rowItem);
            }
        }

        public TableListItem(bool isHeaderRow)
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
            RowData.Add(item2Add);
            
        }

        /// <summary>
        /// Appends a List to the RowData
        /// </summary>
        /// <param name="items2Add"></param>
        public void AddItem(List<string> items2Add)
        {

            foreach (string curItem in items2Add)
            {
                RowData.Add(curItem);
            }

        }



    }
}
