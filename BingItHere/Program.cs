using System;
using BingItHere.Tests;
using System.Threading;
using XnLogger;
using XnLogger.Model;
using CoreToolSet.Controllers;


namespace BingItHere
{
    class Program
    {


        static void Main(string[] args)
        {


            BrowserController coreTools = new BrowserController("ff", XnLogger.LogConstants.LOG_DEBUG);




            /*
                        FormTest formTest = new FormTest(coreTools);
                        Thread.Sleep(5000);
            */

                        FindElementTests findElementTests = new FindElementTests(coreTools);
                        Thread.Sleep(5000);
            

            /*
            TableTest tableTest = new TableTest(coreTools);
            Thread.Sleep(5000);
            */

            coreTools.CloseBrowser();

            
        }
    }
}
