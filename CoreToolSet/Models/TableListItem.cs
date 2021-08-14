using System;
using System.Collections.Generic;
using System.Text;

namespace CoreToolSet.Models
{
    public class TableListItem
    {

        public int RowNum { get; set; }
        public List<string> RowData = new List<string>();

        public TableListItem(int rowNum)
        {
            RowNum = rowNum;
        }

        public void AddItem(string item2Add)
        {
            RowData.Add(item2Add);
            
        }

    }
}
