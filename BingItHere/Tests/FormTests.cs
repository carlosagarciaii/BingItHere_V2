using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CoreToolSet;
using CoreToolSet.Controllers;


namespace BingItHere.Tests
{
        class FormTest
        {

            public FormTest(BrowserController coreTools)
            {
                //Form Tests
                coreTools.NavTo("https://www.seleniumeasy.com/test/basic-checkbox-demo.html");
                coreTools.FindElement("isAgeSelected", "id");
                Console.WriteLine("Get Value:\t" + coreTools.GetProperty("value"));
                coreTools.Click();
                Console.WriteLine("Get Value:\t" + coreTools.GetProperty("value"));


                coreTools.NavTo("https://www.seleniumeasy.com/test/basic-first-form-demo.html");
                coreTools.FindElement("at-cv-lightbox-close", "id", false);
                coreTools.Click();
                Thread.Sleep(1000);
                coreTools.FindElement("user-message", "id", true);

                Thread.Sleep(3000);
                coreTools.SendKeys("    This is my test   I Test   ");
                Thread.Sleep(5000);


                Console.WriteLine("Get Value:\t" + coreTools.GetProperty("value"));
                Console.WriteLine("Get Value:\t" + coreTools.GetAttribute("placeholder"));

            }

        }

    }
