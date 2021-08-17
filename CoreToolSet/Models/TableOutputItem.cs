using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet.Models;

namespace CoreToolSet.Models
{
    public class TableOutputItem
    {
        public List<TableListItem> TableData = new List<TableListItem>();


        public TableOutputItem(List<TableListItem> tableData)
        {
            TableData = tableData;

        }


        public void DisplayTable()
        {

            string lineText = "";


            foreach (var lineItemText in TableData)
            {
                lineText = "|";
                foreach (var cellItemText in lineItemText.RowData)
                {
                    lineText += cellItemText;
                }
                Console.Write(lineText);
            }



        }

    }
}
