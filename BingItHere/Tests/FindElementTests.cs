using System;
using System.Collections.Generic;
using System.Text;
using CoreToolSet;
using CoreToolSet.Controllers;



namespace BingItHere.Tests
{

    class FindElementTests
    {
        public FindElementTests(BrowserController coreTools)
        {

            // Find Element Tests
            coreTools.NavTo("http://www.ltaat.com");


            coreTools.FindElement("//*[contains(text(),'LTAAT')]");
            coreTools.SetAttribute("class", "");
            coreTools.SetAttribute("style", "text-align:right,font-size:200");

            Console.WriteLine("Attribute:\t" + coreTools.GetAttribute("style"));


            coreTools.FindElement("//a[text()='About the Founder']");

            coreTools.FindElements("//a");
            coreTools.Click();


        }

    }





}
