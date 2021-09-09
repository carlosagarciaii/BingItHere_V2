﻿using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet;


namespace BingItHere.Tests
{
    class TableTest
    {

        public TableTest(CoreTools coreTools)
        {
            // Table Tests
            coreTools.NavTo("https://en.wikipedia.org/wiki/List_of_Virtual_Boy_games");
            coreTools.FindElement("//table[@id='softwarelist']", "xpath");
            coreTools.Table2List();

        }



    }
}
