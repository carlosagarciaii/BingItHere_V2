using System;
using BingItHere.Tests;
using System.Threading;
using XnLogger;
using XnLogger.Model;


namespace BingItHere
{
    class Program
    {


        static void Main(string[] args)
        {


            CoreToolSet.CoreTools coreTools = new CoreToolSet.CoreTools("ff", XnLogger.LogConstants.LOG_DEBUG);


            FormTest formTest = new FormTest();
            formTest.Run(coreTools);

            Thread.Sleep(5000);


            FindElementTests findElementTests = new FindElementTests();
            findElementTests.Run(coreTools);

            Thread.Sleep(10000);
            coreTools.CloseBrowser();


        }
    }
}
