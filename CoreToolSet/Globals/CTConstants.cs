using System;
using System.Collections.Generic;
using System.Text;

namespace CoreToolSet.Globals
{
    public class CTConstants
    {

        // ----------------------------------------------------------------------
        // DRIVER CONSTANTS
        // ----------------------------------------------------------------------
        public const string FIREFOX_DRIVER_NAME = "geckodriver.exe";
        public const string CHROME_DRIVER_NAME = "chromedriver.exe";
        public const string IE_DRIVER_NAME = "IEDriverServer.exe";
        public const string MSEDGE_DRIVER_NAME = "MicrosoftWebDriver.exe";
        public const string MSEDGE_DRIVER_NAME_LEGACY = "msedgedriver.exe";
        public static readonly string[] DEFAULT_DRIVER_DIRECTORIES =  { "C:/ProgramData/SeleniumDrivers",
                                                                        "./Drivers",
                                                                        "../../../Drivers",
                                                                        "../../Drivers",
                                                                        "../Drivers"
        };


        // ----------------------------------------------------------------------
        // WAITING CONSTANTS
        // ----------------------------------------------------------------------
        public const int DEFAULT_MINIMUM_WAIT_TIME = 3;
        public const int DEFAULT_MAXIMUM_WAIT_TIME = 7;




    }
}
