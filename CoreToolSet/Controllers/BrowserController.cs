using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XnLogger;
using CoreToolSet.Models;
using CoreToolSet.Globals;
using System.Reflection;
using Newtonsoft.Json;

namespace CoreToolSet.Controllers
{
	public class BrowserController
	{

		//  ---------------------------------------------------------------
		//  GENERAL
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------


		private Logger logger = new Logger(LogConstants.LOG_INFO);

		string LogMsg;
		private IWebDriver driver { get; set; }
		private string browserName { get; set; }
		private string driverFileName { get; set; }
		private string driverFilePath { get; set; }
		private string elementSelector { get; set; }
		private string locatorStrategy { get; set; }
		private By elementLocator { get; set; }
		public IWebElement element { get; set; }
		public List<IWebElement> elementList { get; set; }



		/// <summary>
		/// Adds a random wait duration to simulate human hesitancy.
		/// <para>minWait = Minimum wait time in seconds (default is defined in CTConstants.DEFAULT_MINIMUM_WAIT_TIME)
		/// <br>maxWait = Maximum wait time in seconds (default is defined in CTConstants.DEFAULT_MAXIMUM_WAIT_TIME)</br></para>
		/// </summary>
		/// <param name="minWait"></param>
		/// <param name="maxWait"></param>

		public void SimulateHumanWait(int minWait = CTConstants.DEFAULT_MINIMUM_WAIT_TIME, int maxWait = CTConstants.DEFAULT_MAXIMUM_WAIT_TIME)
		{
			string funcName = "SimulateHumanWait";
			try
			{
				Random randNum = new Random();
				int waitTime = randNum.Next(minWait, maxWait);
				Thread.Sleep(waitTime * 1000);
			}
			catch (Exception e)
			{
				LogMsg = $"Failed to Initiate Sleep Timer. minWait {minWait.ToString()} | maxWait {maxWait.ToString()}\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}
		}

		//  ---------------------------------------------------------------
		//  CREATING A SESSION
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region Creating A Session

		/// <summary>
		/// <para>Returns a String for the filename of the WebDriver</para>
		/// <para>browserName = The name of the browser
		/// <br>---Gecko = FF or FireFox</br>
		/// <br>---Chrome = Google or Chrome</br>
		/// <br>---IE = IE or IExplore</br>
		/// <br>---MSEdge = Edge or MSEdge</br></para>
		/// </summary>
		/// <returns>String value for the driver's file name</returns>
		private void SetDriverFileName()
		{
			string funcName = "SetDriverFileName";

			switch (browserName.ToLower())
			{
				case "ff":
				case "firefox":
					driverFileName = CTConstants.FIREFOX_DRIVER_NAME;
					break;
				case "chrome":
				case "google":
					driverFileName = CTConstants.CHROME_DRIVER_NAME;
					break;
				case "ie":
				case "iexplore":
					driverFileName = CTConstants.IE_DRIVER_NAME;
					break;
				case "edge":
				case "msedge":
					driverFileName = CTConstants.MSEDGE_DRIVER_NAME;
					break;
				default:
					string LogMsg = "The Browser Provided does not match an acceptable value.";
					logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
					throw new Exception(LogMsg);

			}

		}


		/// <summary>
		/// Returns a String for the full path to the Driver File
		/// <para>DriverFileName = The name of the Driver File (IE: Geckodriver.exe)</para>
		/// </summary>
		/// <param name="driverFileName"></param>
		/// <returns></returns>
		private void SetDriverFilePath()
		{
			string funcName = "SetDriverFilePath";
			bool isFoundDriverDirPath = false;
			string fullDriverFilePath;


			foreach (string driverDirPathItem in CTConstants.DEFAULT_DRIVER_DIRECTORIES)
			{
				if (Directory.Exists(driverDirPathItem))
				{
					isFoundDriverDirPath = true;
					logger.Write($"Driver Path Found:\t{driverFilePath}", funcName, LogConstants.LOG_INFO);
					fullDriverFilePath = driverDirPathItem + "/" + driverFileName;

					RenameEdgeDriver(driverDirPathItem);

					if (File.Exists(fullDriverFilePath))
					{
						logger.Write($"Driver File Found:\t{fullDriverFilePath}", funcName, LogConstants.LOG_INFO);
						driverFilePath = driverDirPathItem + "/";
						return;
					}
				}
			}
			if (!isFoundDriverDirPath)
			{
				LogMsg = "EXCEPTION:\n\tDriver Directory Cannot Be Found";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}

			LogMsg = "EXCEPTION:\n\tDriver File Cannot Be Found";
			logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
			throw new Exception(LogMsg);
		}


		/// <summary>
		/// Renames the Edge Driver from msedgedriver.exe to MicrosoftWebDriver.exe
		/// <para>driverPath = the path to the driver</para>
		/// </summary>
		/// <param name="driverPath"></param>
		private void RenameEdgeDriver(string driverPath)
		{
			string funcName = "RenameEdgeDriver";

			if (!File.Exists(driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME) && File.Exists(driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME_LEGACY))
			{
				File.Copy(driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME_LEGACY, driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME);
				LogMsg = $"{CTConstants.MSEDGE_DRIVER_NAME} copied as {CTConstants.MSEDGE_DRIVER_NAME_LEGACY} to run application.";
				logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);
			}

			if (!File.Exists(driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME) && !File.Exists(driverPath + "/" + CTConstants.MSEDGE_DRIVER_NAME_LEGACY))
			{
				LogMsg = $"Could not Locate usable EDGE DRIVER. Please place a viable EDGE DRIVER with the name {CTConstants.MSEDGE_DRIVER_NAME} or {CTConstants.MSEDGE_DRIVER_NAME_LEGACY} in the Drivers folder.";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
			}


		}

		/// <summary>
		/// <para>Creates the IWebDriver session based on the browser selected</para>
		/// <para>browserName = The name of the browser
		/// <br>---Gecko = FF or FireFox</br>
		/// <br>---Chrome = Google or Chrome</br>
		/// <br>---IE = IE or IExplore</br>
		/// <br>---MSEdge = Edge or MSEdge</br></para>
		/// </summary>
		/// <param name="browserName"></param>
		/// <returns></returns>
		private IWebDriver CreateSession()
		{
			string funcName = "CreateSession";
			try
			{
				switch (browserName.ToLower())
				{
					case "ff":
						return new FirefoxDriver(driverFilePath);

					case "firefox":
						return new FirefoxDriver(driverFilePath);
					case "chrome":
						return new ChromeDriver(driverFilePath);
					case "google":
						return new ChromeDriver(driverFilePath);
					case "ie":
						return new InternetExplorerDriver(driverFilePath);
					case "iexplore":
						return new InternetExplorerDriver(driverFilePath);
					case "edge":
						return new EdgeDriver(driverFilePath);
					case "msedge":
						return new EdgeDriver(driverFilePath);
					default:
						string LogMsg = "Unable to Locate WebDriver";
						logger.Write(LogMsg, funcName);
						throw new Exception(LogMsg);
				}
			}
			catch (Exception e)
			{
				LogMsg = $"Error while Attempting to Create Session\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}
		}



		/// <summary>
		/// Instantiates the class and Opens the browser session 
		/// <para><br>browserName = the name of the browser to open</br>
		/// <br>-Options:</br>
		/// <br>---FireFox, FF</br>
		/// <br>---Google, Chrome</br>
		/// <br>---IE, IExplore</br>
		/// <br>---Edge, MSEdge</br>
		/// <br>loggingLevel = The highest severity to log (default: INFO)</br>
		/// <br>logFileName = The name for the LogFile (Default defined in LogConstants.LOGFILE_NAME)</br>
		/// </para>
		/// </summary>
		/// <param name="browserName"></param>
		/// <param name="setLogLevel"></param>
		/// <param name="logFileName"></param>
		public BrowserController(string browserName, XnLogger.Model.LogLevel setLogLevel , string logFileName = LogConstants.LOGFILE_NAME)
		{
			string funcName = "CoreTools";
			logger = new Logger( setLogLevel, logFileName);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			this.browserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				driver = CreateSession();
				driver.Manage().Window.Maximize();
			}
			catch (Exception e)
			{
				LogMsg = $"Error while attempting to create the Browser Session\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}
		}

		/// <summary>
		/// Instantiates the class and Opens the browser session 
		/// <para><br>browserName = the name of the browser to open</br>
		/// <br>-Options:</br>
		/// <br>---FireFox, FF</br>
		/// <br>---Google, Chrome</br>
		/// <br>---IE, IExplore</br>
		/// <br>---Edge, MSEdge</br>
		/// <br>logFileName = The name for the LogFile (Default defined in LogConstants.LOGFILE_NAME)</br>
		/// </para>
		/// </summary>
		/// <param name="browserName"></param>
		/// <param name="logFileName"></param>

		public BrowserController(string browserName, string logFileName = LogConstants.LOGFILE_NAME)
		{
			string funcName = "CoreTools";
			logger = new Logger(LogConstants.LOG_INFO, logFileName);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			this.browserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				driver = CreateSession();

				driver.Manage().Window.Maximize();
			}
			catch (Exception e)
			{
				LogMsg = $"Error while attempting to create the Browser Session\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}
		}

		/// <summary>
		/// Instantiates the class and Opens the browser session 
		/// <para><br>browserName = the name of the browser to open</br>
		/// <br>-Options:</br>
		/// <br>---FireFox, FF</br>
		/// <br>---Google, Chrome</br>
		/// <br>---IE, IExplore</br>
		/// <br>---Edge, MSEdge</br>
		/// </para>
		/// </summary>
		/// <param name="browserName"></param>
		public BrowserController(string browserName)
		{
			string funcName = "OpenBrowser";
			logger = new Logger(LogConstants.LOG_INFO);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			this.browserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				driver = CreateSession();

				driver.Manage().Window.Maximize();
			}
			catch (Exception e)
			{
				LogMsg = $"Error while attempting to create the Browser Session\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}

		}

        #endregion


        //  ---------------------------------------------------------------
        //  CLOSING A SESSION
        //  ---------------------------------------------------------------
        //  ---------------------------------------------------------------

        #region Closing a Session
        /// <summary>
        /// Closes the Browser.
        /// </summary>
        public void CloseBrowser()
		{
			string funcName = "CloseBrowser";

			logger.Write("Closing Browser Session", funcName, LogConstants.LOG_INFO);
			try
			{
				driver.Close();
			}
			catch (Exception e)
			{
				string exceptionMsg = $"EXCEPTION:\n\tBrowser has failed to close.\n{e}";
				logger.Write(exceptionMsg, funcName, LogConstants.LOG_WARNING);
				throw new Exception(exceptionMsg);

			}

		}

        #endregion

        //  ---------------------------------------------------------------
        //  Navigating To pages
        //  ---------------------------------------------------------------
        //  ---------------------------------------------------------------

        #region Navigating Pages

        /// <summary>
        /// Navigates to the specified URL
        /// <para><br>goToURL = The URL to navigate to.</br>
        /// <br>retryNumber = Number of Times to Retry loading the page (Default 0)</br>
        /// <br>waitInSec = The number of seconds to wait before reloading the page.(Default 20) </br></para>
        /// </summary>
        /// <param name="goToURL"></param>
        /// <param name="retryNumbers"></param>
        /// <param name="waitInSec"></param>
        public void NavTo(string goToURL, int retryNumbers = 0, int waitInSec = 20, bool hasHumanWait = true)
		{
			string funcName = "NavTo";
			string ReadyState = "";
			driver.Url = goToURL;
			for (int retryCount = retryNumbers + 1; retryCount >= 0; retryCount--)
			{
				driver.Navigate();

				for (int i = 0; i < waitInSec; i++)
				{
					try
					{
						Thread.Sleep(1000);
						ReadyState = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState;");
						logger.Write($"Ready State:\t{ReadyState}", funcName, LogConstants.LOG_DEBUG);
						if (ReadyState.ToLower() == "complete") break;
					}
					catch (Exception e)
					{
						string LogMsg = $"\nEXCEPTION HAS OCCURRRED\n\t{e}";
						logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
						throw new Exception(LogMsg);
					}

				}
				if (ReadyState.ToLower() == "complete") break;
			}

			if (ReadyState.ToLower() != "complete") throw new Exception($"\nEXCEPTION:\n\tFailed to Load Page {goToURL}");
			logger.Write($"Success - Navigated to:\t{driver.Url.ToString()}", funcName, LogConstants.LOG_INFO);

			if (hasHumanWait) { SimulateHumanWait(6, 10); }
		}

        #endregion

        //  ---------------------------------------------------------------
        //  Find Elements (FE)
        //  ---------------------------------------------------------------
        //  ---------------------------------------------------------------

        #region Find Elements

        /// <summary>
        /// Sets the Locator and strategy to use.
        /// <para><br>-- elementSelector = The selector for the element (IE: #id, "//a", etc)</br>
        /// <br>-- locatorStrategy = The Strategy to use to locate the element </br>
        /// <br>-- -- -- xpath (default)</br>
        /// <br>-- -- -- css / cssselector</br>
        /// <br>-- -- -- name</br>
        /// <br>-- -- -- id</br>
        /// </para>
        /// </summary>
        /// <param name="elementSelector"></param>
        /// <param name="locatorStrategy"></param>

        public void SetLocator(string elementSelector, string locatorStrategy = "xpath")
		{
			string funcName = "SetLocator";
			this.elementSelector = elementSelector;
			this.locatorStrategy = locatorStrategy;
			switch (this.locatorStrategy.ToLower())
			{
				case "xpath":
                    LogMsg = $"Locator Strategy:\tXPATH\tElement:\t{elementSelector}";
                    elementLocator = By.XPath(elementSelector);
					break;
				case "css":
				case "cssselector":
                    LogMsg = $"Locator Strategy:\tCssSelector\tElement:\t{elementSelector}";
                    elementLocator = By.CssSelector(elementSelector);
					break;
				case "id":
                    LogMsg = $"Locator Strategy:\tID\tElement:\t{elementSelector}";
                    elementLocator = By.Id(elementSelector);
					break;
				case "name":
                    LogMsg = $"Locator Strategy:\tName\tElement:\t{elementSelector}";
                    elementLocator = By.Name(elementSelector);
					break;
				case "tagname":
                    LogMsg = $"Locator Strategy:\tTagName\tElement:\t{elementSelector}";
                    elementLocator = By.TagName(elementSelector);
					break;
				case "classname":
                    LogMsg = $"Locator Strategy:\tClassName\tElement:\t{elementSelector}";
                    elementLocator = By.ClassName(elementSelector);
					break;
				default:
                    LogMsg = $"EXCEPTION\tLOCATOR ERROR\nError:\tFE00001\n\tThe Locator Stragety Provided does not match a recognized strategy.\n\tLocator Strategy Provided:\t{locatorStrategy}\n\tLocator:\t{elementSelector}";
                    logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
					throw new Exception(LogMsg);

			}
			logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
		}

		/// <summary>
		/// Locates a single Element on the page that matches the locator and returns an IWebElement object
		/// <para><br>-- elementSelector = The locator for the element (IE: #id, "//a", etc)</br>
		/// <br>-- locatorStrategy = The Strategy to use to locate the element </br>
		/// <br>-- -- -- xpath (default)</br>
		/// <br>-- -- -- css / cssselector</br>
		/// <br>-- -- -- name</br>
		/// <br>-- -- -- id</br>
		/// <br>-- isRequired = Whether element is required. (Default: true)</br>
		/// <br>-- waitForElement = Wait for element to appear on page (true {default}/false)</br>
		/// <br>-- waitTimeSec = Number of seconds to wait for the element (default = 20)</br>
		/// </para>
		/// </summary>
		/// <param name="elementSelector"></param>
		/// <param name="locatorStrategy"></param>
		/// <param name="isRequired"></param>
		/// <param name="waitForElement"></param>
		/// <param name="waitTimeSec"></param>
		/// <returns></returns>

		public void FindElement(string elementSelector, string locatorStrategy = "xpath", bool isRequired = true, bool waitForElement = true, int waitTimeSec = 20)
		{
			string funcName = "FindElement";
			element = null;

			SetLocator(elementSelector, locatorStrategy);
			logger.Write($"Finding Element [{elementSelector}]", funcName, LogConstants.LOG_DEBUG);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					element = driver.FindElement(elementLocator);
					break;
				}
				catch
				{
					LogMsg = $"Locator not found:\t{elementSelector}\t|\t{locatorStrategy}";

					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);

				}
			}

			if (element == null)
			{
                LogMsg = $"Unable to Locate Element:\t{this.elementSelector} using strategy {locatorStrategy}.";
				if (isRequired)
				{
					logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
					throw new Exception(LogMsg);

				}
				else
				{
					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);
				}
			}

		}


		/// <summary>
		/// Locates all Elements on the page that match the locator and returns a List of IWebElement objects
		/// <para><br>-- elementLocator = The locator for the element (IE: #id, "//a", etc)</br>
		/// <br>-- locatorStrategy = The Strategy to use to locate the element </br>
		/// <br>-- -- -- xpath (default)</br>
		/// <br>-- -- -- css / cssselector</br>
		/// <br>-- -- -- name</br>
		/// <br>-- -- -- id</br>
		/// <br>-- isRequired = If element is required (Default true)</br>
		/// <br>-- waitForElement = Wait for element to appear on page (true {default}/false)</br>
		/// <br>-- waitTimeSec = Number of seconds to wait for the element (default = 20)</br>
		/// </para>
		/// </summary>
		/// <param name="elementSelector"></param>
		/// <param name="locatorStrategy"></param>
		/// <param name="isRequired"></param>
		/// <param name="waitForElement"></param>
		/// <param name="waitTimeSec"></param>
		/// <returns></returns>

		public void FindElements(string elementSelector, string locatorStrategy = "xpath", bool isRequired = true, bool waitForElement = true, int waitTimeSec = 20)
		{

			string funcName = "FindElements";
			elementList = new List<IWebElement>();


			SetLocator(elementSelector, locatorStrategy);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					elementList = new List<IWebElement>(driver.FindElements(elementLocator));

					break;
				}
				catch
				{
					LogMsg = $"Locator not found:\t{elementSelector}\t|\t{locatorStrategy}";
					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);

				}
			}

			if (elementList.Count == 0)
			{
				LogMsg = $"Unable to Locate Element:\t{elementSelector} using strategy {locatorStrategy}.";
				if (isRequired)
				{
					logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
					throw new Exception(LogMsg);
				}
				else
				{
					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);
				}
			}
			else
			{
				string ListOfElements = "\n";
				foreach (var element in elementList)
				{
					ListOfElements += $"|{element.ToString()}|\n";
				}
				LogMsg = $"Elements Found:\t{elementList.Count.ToString()}\n\t{ListOfElements}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
			}

		}

        #endregion

        //  ---------------------------------------------------------------
        //  Element Interactions (FE)
        //  ---------------------------------------------------------------
        //  ---------------------------------------------------------------

        #region Element Interactions

        /// <summary>
        /// Clicks on the Element
        /// <para>hasHumanWait = whether to simulate human hesitancy after a click (default: true)</para>
        /// </summary>
        /// <param name="hasHumanWait"></param>
        public void Click(bool hasHumanWait = true)
		{
			string funcName = "Click";
			try
			{
				logger.Write("Clicking on Element.", funcName, LogConstants.LOG_DEBUG);
				element.Click();
			}
			catch (Exception e)
			{
				LogMsg = $"ERROR:\tCannot Click with Selenium.\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);

			}
			if (hasHumanWait) { SimulateHumanWait(); }

		}


		/// <summary>
		/// Gets Element Attributes (IE: InnerText, InnerHTML)
		/// <para><br>--- attribute2Get = the attribute to get.</br>
		/// <br>--- --- innerHTML</br>
		/// <br>--- --- innerText</br>
		/// <br>--- --- Others as allowed by Selenium WebDriver</br></para>
		/// </summary>
		/// <param name="attribute2Get"></param>
		/// <returns></returns>
		public string GetAttribute(string attribute2Get = "innerText")
		{
			string funcName = "GetAttribute";
			string outValue = "";

			try
			{
				switch (attribute2Get.ToLower())
				{
					case "innertext":
					case "text":
						outValue = element.GetAttribute("innerText");
						break;
					case "innerhtml":
					case "html":
						outValue = element.GetAttribute("innerHTML");
						break;

					default:
						outValue = element.GetAttribute(attribute2Get);
						break;
				}
			}
			catch (Exception e)
			{
				LogMsg = $"ERROR:\tThe attribute provided [{attribute2Get}] does not match a valid type for element [{element}]. \n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}

			return outValue;

		}


		/// <summary>
		/// GetProperty retrieves the property from an element.
		/// <para>- property2Get = the Property to get. (Default: Value)</para>
		/// </summary>
		/// <param name="property2Get"></param>
		/// <returns></returns>
		public string GetProperty(string property2Get = "value")
		{
			string funcName = "GetProperty";
			string outValue = "";

			try
			{
				outValue = element.GetProperty(property2Get);
			}
			catch (Exception e)
			{
				LogMsg = $"ERROR:\tThe attribute provided [{property2Get}] does not match a valid type for element [{element}] . \n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}
			return outValue;

		}


		/// <summary>
		/// Sends keys to the element
		/// <para>sendValue = the text to send
		/// <br>doTrim = Trim all beginning and ending white space as well as removing double spaces</br></para>
		/// </summary>
		/// <param name="sendValue"></param>
		/// <param name="doTrim"></param>
		public void SendKeys(string sendValue, bool doTrim = true)
		{
			string funcName = "SendKeys";

			if (doTrim)
			{
				// Remove Double Spacing
				sendValue = Regex.Replace(sendValue, "\\s+", " ");
				// Left Trim/Right Trim
				sendValue = sendValue.Trim();
			}

			try
			{
				element.SendKeys(sendValue);
			}
			catch (Exception e)
			{
				LogMsg = $"Unable to Send text [{sendValue}] to Element [{element}]\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}

		}

		/// <summary>
		/// Set the Attribute for an Element
		/// </summary>
		/// <param name="attribute2Set"></param>
		/// <param name="value2Set"></param>
		public void SetAttribute(string attribute2Set, string value2Set)
		{

			string funcName = "SetAttribute";
			string jScript = "";

			try
			{

				logger.Write($"JavaScript to Run:\n{jScript}", funcName, LogConstants.LOG_DEBUG);

				var testItem = ((IJavaScriptExecutor)driver).ExecuteScript(jScript);
			}
			catch (Exception e)
			{
				LogMsg = $"Failed to Run JavaScript:\n{jScript}\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);

			}

		}



		/// <summary>
		/// Generates the JavaScript locator script 
		/// </summary>
		/// <returns></returns>
		public string LocateByJS()
		{
//			throw new Exception("Not Yet Implemented");
			string funcName = "LocateByJS";
			string jsOutString = "";
			switch (locatorStrategy.ToLower())
			{
				case ("xpath"):
					jsOutString = $"document.evaluate(\"{elementSelector}\", document,null, XPathResult.ANY_TYPE,null).FIRST_ORDERED_NODE_TYPE";
					break;
				case ("css"):
				case ("cssselector"):
					jsOutString = $"document.querySelector(\"{elementSelector}\");";
					break;
				case ("id"):
					jsOutString = $"document.getElementById(\"{elementSelector}\");";
					break;
				case ("name"):
					jsOutString = $"document.getElementsByName(\"{elementSelector}\");";
					break;
				case ("classname"):
					jsOutString = $"document.getElementsByClassName(\"{elementSelector}\");";
					break;
				case ("tagname"):
					jsOutString = $"document.getElementsByTagName(\"{elementSelector}\");";
					break;

				default:
					LogMsg = "Locator Strategy does not match viable options. ";
					logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
					throw new Exception(LogMsg);

			}

			if (jsOutString == "")
			{
				LogMsg = $"Location Strategy [{locatorStrategy}] not supported.";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}

			return jsOutString;
		}

		#endregion




		//  ---------------------------------------------------------------
		//  JavaScript Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region JavaScript

		public void RunJS(string javascriptCode)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			try
			{
				((IJavaScriptExecutor)driver).ExecuteScript($"{javascriptCode}");
			}
			catch (Exception e)
			{
				logger.Write(e.Message, funcName);

			}
		}

		public string RunJsStringOut(string javascriptCode)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			try
			{
				return (string)((IJavaScriptExecutor)driver).ExecuteScript($"{javascriptCode}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Thread.Sleep(5000);
				throw new Exception(e.Message);

			}
		}

		/// <summary>
		/// Uses JavaScript to find and click on an element <br />
		/// Should click elements that normal Selenium Click does not <br />
		/// Does not always return errors when click fails (JS limitation)
		/// </summary>
		/// <param name="xpath">Xpath to the element</param>
		public void ClickJS(string xpath)
		{
			string funcName = MethodBase.GetCurrentMethod().Name;
			string jScript = "";

			jScript += "function clickJS(xpath) \n";
			jScript += "{  \n";
			jScript += "	try \n";
			jScript += "	{ \n";
			jScript += "		let element = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE).singleNodeValue; \n";
			jScript += "		element.click(); \n";
			jScript += "	} \n";
			jScript += "	catch (e) \n";
			jScript += "	{ \n";
			jScript += "		return e; \n";
			jScript += "	} \n";
			jScript += "	return 'SUCCESS'; \n";
			jScript += "} \n";
			jScript += $"return clickJS(`{xpath}`);";

			string returnValue = "";
			try
			{

				returnValue = ((IJavaScriptExecutor)driver).ExecuteScript($"{jScript}").ToString();
				if (returnValue == "SUCCESS")
				{
					logger.Write($"{returnValue} Click on [{xpath}]", funcName);
				}
				else
				{
					logger.Write($"Failed to click on [{xpath}]\n\t{returnValue}", funcName);
				}

			}
			catch (Exception e)
			{
				logger.Write(e.Message, funcName);
			}


		}


		#endregion




		//  ---------------------------------------------------------------
		//  Table Interactions (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region Table Interactions
		public void Table2List(int headerRow = 1,bool textOnly = true)
		{
			string funcName = "Table2List";


			try
			{
	
				// Declare Variables
				By tableRowsSelector = By.XPath("//tr");
				By tableHeadersSelector = By.XPath("//th");
				By tableCellsSelector = By.XPath("//td");
				List<IWebElement> tableRowsElements = new List<IWebElement>();		// The Rows for the Table
//				List<IWebElement> tableHeaderElements = new List<IWebElement>();	// The Header Rowfor the table (May not be needed)
				List<IWebElement> tableCellsElements = new List<IWebElement>();		// The Cells for the current Row
				TableRowModel tableRowContents = new TableRowModel();				// The text contents of the table row
				TableModel table2Output = new TableModel();


				// Get <TR> tags
				tableRowsElements.Clear();
				foreach (var iElement in element.FindElements(tableRowsSelector))
				{
					tableRowsElements.Add(iElement);
				}


				for (int rowNum = headerRow - 1; tableRowsElements.Count -1 > rowNum;rowNum++)
				{

					//Check for Header
						//May be unnecessary.... 
					tableCellsElements.Clear();
					foreach (var iCellElement in tableRowsElements[rowNum].FindElements(tableHeadersSelector))
					{
						tableCellsElements.Add(iCellElement);
					}
					foreach (var iCellElement in tableRowsElements[rowNum].FindElements(tableHeadersSelector))
					{
						tableCellsElements.Add(iCellElement);
					}

					if (tableCellsElements.Count > 0)
					{
						// Get Header Row Contents
//						for (int cellNum = 0;cellNum < tableCellsElements.Count - 1; cellNum++)
						foreach (var curTableCell in tableCellsElements)
						{
							tableRowContents.AddItem(curTableCell.Text);
						}
						table2Output.AddRow(tableRowContents);
						
					}
					else
					{
						// Check for TD Cells
						tableCellsElements.Clear();
						foreach (var iCellElement in tableRowsElements[rowNum].FindElements(tableCellsSelector))
						{
							tableCellsElements.Add(iCellElement);
						}


						if (tableCellsElements.Count > 0)
						{
							foreach (var curTableCell in tableCellsElements)
							{
								tableRowContents.AddItem(curTableCell.Text);
							}
							table2Output.AddRow(tableRowContents);

						}
						else
						{
							LogMsg = $"Could not locate a TD nor TH cell in current Row. ";
							logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
							throw new Exception(LogMsg);
						}
					}
				}
				table2Output.GetTable();
// Old Notes?
//				string tableHTML = GetAttribute("innerHTML");
//				logger.Write(tableHTML, funcName);

			}
			catch (Exception e)
			{
				LogMsg = $"An Unhandled Exception occurred.\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);
				throw new Exception(LogMsg);
			}
		}

		#endregion




		//  ---------------------------------------------------------------
		//  JQX Element Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------

		#region Jqx Element Functions


		public void GetJqxGridColumn(string gridId, int columnNum = 1)
		{
			string elementXpath = $"//div[@id='{gridId}']//div[@role='row']/div[@columnindex={columnNum.ToString()}]/div";

			FindElements(elementXpath);

		}

		/// <summary>
		/// Reads a jqxGird then converts the data into a Dictionary.
		/// </summary>
		/// <param name="gridId">The Element ID for the jqxGrid</param>
		/// <param name="debug">Determines whether Javascript will be sent to SQL</param>
		/// <returns>JSON from the Grid as Dictionary (int,dynamic)</returns>
		public Dictionary<int, dynamic> JqxGridData(string gridId, bool debug = false)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			Dictionary<int, dynamic> output = new Dictionary<int, dynamic>();
			logger.Write($"Getting Grid Data for {gridId}", funcName);
			Console.WriteLine($"Getting Grid Data for {gridId}");
			string jscript = "";

			// Javascript to be run 
			jscript = "function getJqxTableData(gridId)	\n	{	\n";
			jscript += "	let xpath = `//div[@id='${gridId}']//div[@role='columnheader']//span`;	\n";
			jscript += "	let allHeaders = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n";
			jscript += "	let columnCount = allHeaders.snapshotLength;	\n	";
			jscript += "	xpath = `//div[@id='${gridId}']//div[@role='columnheader'][not(contains(@style,'display'))]//span` \n ";
			jscript += "	let visibleHeaders = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n	";
			jscript += "	let visibleHeaderItem = 0;	\n ";
			jscript += "	let tableHeaders = [];	\n ";
			jscript += "	for (i = 0; i < columnCount; i++)	\n	";
			jscript += "	{	\n	";
			jscript += "		let currentHeader = allHeaders.snapshotItem(i);	\n	";
			jscript += "		let currentVisible = visibleHeaders.snapshotItem(visibleHeaderItem);	\n ";
			jscript += "		if (currentHeader == currentVisible)	\n ";
			jscript += "		{	\n	";
			jscript += "			tableHeaders.push({ colTitle: currentVisible.innerText, colNum: i });	\n	";
			jscript += "			visibleHeaderItem++;	\n	";
			jscript += "		}	\n	";
			jscript += "	}	\n	";
			jscript += "	xpath = `//div[@id='${gridId}']//div[@role='row']/div/div`;	\n	";
			jscript += "	tableRaw = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE);	\n	";
			jscript += "	let rowCount = tableRaw.snapshotLength;	\n	";
			jscript += "	let resultsJson = new Object();	\n	";
			jscript += "	for (i = 0; i < rowCount; i++)	\n	";
			jscript += "	{	\n	";
			jscript += "	let rowJson = new Object();	\n  ";

			jscript += "        for (i2 = 0; i2 < tableHeaders.length; i2++) {	\n	";
			jscript += "            let curColumn = tableHeaders[i2]['colNum'];	\n	";
			jscript += "            let colTitle = tableHeaders[i2]['colTitle'];	\n	";
			jscript += "            xpath = `//div[@id='${gridId}']//div[@id='row${i}${gridId}']/div[@columnindex=${curColumn}]/div`;	\n	";
			jscript += "            let curElement = document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE).singleNodeValue;	\n	";
			jscript += "            try {	\n	";
			jscript += "                if (!curElement.innerText) {	\n	";
			jscript += "                    break;	\n	";
			jscript += "                }	\n	";
			jscript += "                rowJson[colTitle] = curElement.innerText;	\n	";
			jscript += "            }	\n	";
			jscript += "            catch (e) {	\n	";
			jscript += "                break;	\n	";
			jscript += "            }	\n	";
			jscript += "            if (rowJson[tableHeaders[0]['colTitle']]) {	\n	";
			jscript += "                resultsJson[i] = rowJson;	\n	";
			jscript += "            }	\n	";
			jscript += "        }	\n	";
			jscript += "    }	\n	";
			jscript += "    return resultsJson;	\n	";
			jscript += "}	\n	";

			jscript += $"return JSON.stringify(getJqxTableData('{gridId}'));";

			if (debug)
			{
				logger.Write($"Javascript\n\n{jscript}", funcName);
			}

			try
			{
				string resultsJson = ((IJavaScriptExecutor)driver).ExecuteScript($"{jscript}").ToString();
				output = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(resultsJson);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw new Exception($"Func: {funcName}({gridId}) || An error occurred while extracting data from a jqxGrid.\n{e.Message}");
			}


			return output;

		}


		/// <summary>
		/// Reads a jqxDropdown then converts the data into a String
		/// </summary>
		/// <param name="comboBoxId">The Element ID for the jqx Drop Down</param>
		/// <param name="debug">Determines wehther Javascript will be sent to SQL</param>
		/// <returns>JSON from the Grid as String </returns>
		public string JqxComboBoxData(string comboBoxId, bool debug = false)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;
			string output = "";
			string jScript = "";

			jScript += "function getJqxDropDownData(ddmsId)	\n";
			jScript += "{ \n ";
			jScript += "	let xpath = `//select[@id='${ddmsId}']/option[@selected]`; \n ";
			jScript += "	let returnValue = []; \n ";
			jScript += "	let elements = document.evaluate(xpath, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE); \n ";
			jScript += "	for (i = 0; i < elements.snapshotLength; i++) \n ";
			jScript += "	{ \n ";
			jScript += "		returnValue.push(elements.snapshotItem(i).innerText); \n ";
			jScript += "	} \n ";
			jScript += "	return returnValue.toString(); \n ";
			jScript += "} \n ";

			jScript += $"return JSON.stringify( getJqxDropDownData('{comboBoxId}') )";

			if (debug)
			{
				logger.Write($"Javascript\n\n{jScript}", funcName);
			}

			try
			{
				output = ((IJavaScriptExecutor)driver).ExecuteScript($"{jScript}").ToString();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw new Exception($"Func: {funcName}({comboBoxId}) || An error occurred while extracting data from a jqxGrid.\n{e.Message}");
			}

			return output;

		}

		/// <summary>
		/// Clicks the Option from a JQX Dropdown (Multi-Selcr or Single)
		/// </summary>
		/// <param name="comboBoxId">The Element ID for the Select Element</param>
		/// <param name="options">A Comma Separated list of values to select from the Drop Down</param>
		public void ClickJqxComboBoxOptions(string comboBoxId, string options)
		{

			string funcName = MethodBase.GetCurrentMethod().Name;

			try
			{
				string selectXpath = $"//select[@id='{comboBoxId}']";
				selectXpath = $"//select[@id='{comboBoxId}']/following-sibling::button";

				this.FindElement(selectXpath);
				//				this.ClickJS(selectXpath);
				this.Click();

				Thread.Sleep(250);
				var optionValues = options
										.Trim()
										.Replace("  ", " ")
										.Split(new string[] { "," }, StringSplitOptions.None);

				foreach (var opt in optionValues)
				{
					string optionXpath = $"//select[@id='{comboBoxId}']//option[contains(text(),'{opt}')]";
					optionXpath = $"//li[@class='optLookup']//input[contains(@name,'multiselect')][contains(@name,'{comboBoxId}')]/following-sibling::span[contains(text(),'{opt}')]";


					this.FindElement(optionXpath);
					//					this.ClickJS(optionXpath);
					this.Click();
					Thread.Sleep(250);
				}
			}
			catch (Exception e)
			{
				logger.Write(e.Message, funcName);
			}
		}


		#endregion





		//  ---------------------------------------------------------------
		//  General Functions
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------
		#region General Functions

		/// <summary>
		/// Returns the time differences in seconds
		/// </summary>
		/// <param name="startTime">The Start Time</param>
		/// <param name="endTime">The End Time</param>
		/// <returns>endTime - startTime as INT (seconds)</returns>
		public int TimeDiffInt(DateTime startTime, DateTime endTime)
		{

				int result = endTime.Subtract(startTime).Seconds + endTime.Subtract(startTime).Minutes * 60 + endTime.Subtract(startTime).Hours * 3600 + endTime.Subtract(startTime).Hours * 3600 * 24;
				return result;

		}

		
		/// <summary>
		/// Returns the time difference as TimeSpan
		/// </summary>
		/// <param name="startTime">The Start Time</param>
		/// <param name="endTime">The End Time</param>
		/// <returns>endTime - startTime as TimeSpan</returns>
		public TimeSpan TimeDiffTimeSpan(DateTime startTime, DateTime endTime)
		{
			TimeSpan result;

			result = endTime.Subtract(startTime);

			return result;
		}

		
		#endregion



	}


}
