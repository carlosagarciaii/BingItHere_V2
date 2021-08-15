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

namespace CoreToolSet
{
	public class CoreTools
	{

		//  ---------------------------------------------------------------
		//  GENERAL
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------


		private Logger logger = new Logger(LogConstants.LOG_INFO);

		string LogMsg;
		private IWebDriver Driver { get; set; }
		private string BrowserName { get; set; }
		private string DriverFileName { get; set; }
		private string DriverFilePath { get; set; }
		private string ElementSelector { get; set; }
		private string LocatorStrategy { get; set; }
		private By ElementLocator { get; set; }
		public IWebElement Element { get; set; }
		public List<IWebElement> ElementList { get; set; }



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

			switch (BrowserName.ToLower())
			{
				case "ff":
				case "firefox":
					DriverFileName = CTConstants.FIREFOX_DRIVER_NAME;
					break;
				case "chrome":
				case "google":
					DriverFileName = CTConstants.CHROME_DRIVER_NAME;
					break;
				case "ie":
				case "iexplore":
					DriverFileName = CTConstants.IE_DRIVER_NAME;
					break;
				case "edge":
				case "msedge":
					DriverFileName = CTConstants.MSEDGE_DRIVER_NAME;
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
					logger.Write($"Driver Path Found:\t{DriverFilePath}", funcName, LogConstants.LOG_INFO);
					fullDriverFilePath = driverDirPathItem + "/" + DriverFileName;

					RenameEdgeDriver(driverDirPathItem);

					if (File.Exists(fullDriverFilePath))
					{
						logger.Write($"Driver File Found:\t{fullDriverFilePath}", funcName, LogConstants.LOG_INFO);
						DriverFilePath = driverDirPathItem + "/";
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
				switch (BrowserName.ToLower())
				{
					case "ff":
						return new FirefoxDriver(DriverFilePath);

					case "firefox":
						return new FirefoxDriver(DriverFilePath);
					case "chrome":
						return new ChromeDriver(DriverFilePath);
					case "google":
						return new ChromeDriver(DriverFilePath);
					case "ie":
						return new InternetExplorerDriver(DriverFilePath);
					case "iexplore":
						return new InternetExplorerDriver(DriverFilePath);
					case "edge":
						return new EdgeDriver(DriverFilePath);
					case "msedge":
						return new EdgeDriver(DriverFilePath);
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
		public CoreTools(string browserName, XnLogger.Model.LogLevel setLogLevel , string logFileName = LogConstants.LOGFILE_NAME)
		{
			string funcName = "CoreTools";
			logger = new Logger( setLogLevel, logFileName);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			BrowserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				Driver = CreateSession();
				Driver.Manage().Window.Maximize();
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

		public CoreTools(string browserName, string logFileName = LogConstants.LOGFILE_NAME)
		{
			string funcName = "CoreTools";
			logger = new Logger(LogConstants.LOG_INFO, logFileName);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			BrowserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				Driver = CreateSession();

				Driver.Manage().Window.Maximize();
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
		public CoreTools(string browserName)
		{
			string funcName = "OpenBrowser";
			logger = new Logger(LogConstants.LOG_INFO);

			logger.Write("Opening Browser", funcName, LogConstants.LOG_INFO);
			BrowserName = browserName;

			try
			{
				SetDriverFileName();

				SetDriverFilePath();

				Driver = CreateSession();

				Driver.Manage().Window.Maximize();
			}
			catch (Exception e)
			{
				LogMsg = $"Error while attempting to create the Browser Session\n{e}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_CRITICAL);
				throw new Exception(LogMsg);
			}

		}




		//  ---------------------------------------------------------------
		//  CLOSING A SESSION
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------


		/// <summary>
		/// Closes the Browser.
		/// </summary>
		public void CloseBrowser()
		{
			string funcName = "CloseBrowser";

			logger.Write("Closing Browser Session", funcName, LogConstants.LOG_INFO);
			try
			{
				Driver.Close();
			}
			catch (Exception e)
			{
				string exceptionMsg = $"EXCEPTION:\n\tBrowser has failed to close.\n{e}";
				logger.Write(exceptionMsg, funcName, LogConstants.LOG_WARNING);
				throw new Exception(exceptionMsg);

			}

		}

		//  ---------------------------------------------------------------
		//  Navigating To pages
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------


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
			Driver.Url = goToURL;
			for (int retryCount = retryNumbers + 1; retryCount >= 0; retryCount--)
			{
				Driver.Navigate();

				for (int i = 0; i < waitInSec; i++)
				{
					try
					{
						Thread.Sleep(1000);
						ReadyState = (string)((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState;");
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
			logger.Write($"Success - Navigated to:\t{Driver.Url.ToString()}", funcName, LogConstants.LOG_INFO);

			if (hasHumanWait) { SimulateHumanWait(6, 10); }
		}


		//  ---------------------------------------------------------------
		//  Find Elements (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------



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
			ElementSelector = elementSelector;
			LocatorStrategy = locatorStrategy;
			switch (LocatorStrategy.ToLower())
			{
				case "xpath":
					LogMsg = $"Locator Strategy:\tXPATH\tElement:\t{elementSelector}";
					ElementLocator = By.XPath(elementSelector);
					break;
				case "css":
				case "cssselector":
					LogMsg = $"Locator Strategy:\tCssSelector\tElement:\t{elementSelector}";
					ElementLocator = By.CssSelector(elementSelector);
					break;
				case "id":
					LogMsg = $"Locator Strategy:\tID\tElement:\t{elementSelector}";
					ElementLocator = By.Id(elementSelector);
					break;
				case "name":
					LogMsg = $"Locator Strategy:\tName\tElement:\t{elementSelector}";
					ElementLocator = By.Name(elementSelector);
					break;
				case "tagname":
					LogMsg = $"Locator Strategy:\tTagName\tElement:\t{elementSelector}";
					ElementLocator = By.TagName(elementSelector);
					break;
				case "classname":
					LogMsg = $"Locator Strategy:\tClassName\tElement:\t{elementSelector}";
					ElementLocator = By.ClassName(elementSelector);
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
			Element = null;

			SetLocator(elementSelector, locatorStrategy);
			logger.Write($"Finding Element [{elementSelector}]", funcName, LogConstants.LOG_DEBUG);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					Element = Driver.FindElement(ElementLocator);
					break;
				}
				catch
				{
					LogMsg = $"Locator not found:\t{elementSelector}\t|\t{locatorStrategy}";

					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);

				}
			}

			if (Element == null)
			{
				LogMsg = $"Unable to Locate Element:\t{ElementSelector} using strategy {locatorStrategy}.";
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
			ElementList = new List<IWebElement>();


			SetLocator(elementSelector, locatorStrategy);

			int waitLoopCounter = (waitTimeSec < 1) ? 1 : waitTimeSec;
			for (int waitCount = waitLoopCounter; waitCount > 0; waitCount--)
			{
				Thread.Sleep(1000);
				try
				{
					ElementList = new List<IWebElement>(Driver.FindElements(ElementLocator));

					break;
				}
				catch
				{
					LogMsg = $"Locator not found:\t{elementSelector}\t|\t{locatorStrategy}";
					logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);

				}
			}

			if (ElementList.Count == 0)
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
				foreach (var element in ElementList)
				{
					ListOfElements += $"|{element.ToString()}|\n";
				}
				LogMsg = $"Elements Found:\t{ElementList.Count.ToString()}\n\t{ListOfElements}";
				logger.Write(LogMsg, funcName, LogConstants.LOG_DEBUG);
			}

		}



		//  ---------------------------------------------------------------
		//  Element Interactions (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------



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
				Element.Click();
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
						outValue = Element.GetAttribute("innerText");
						break;
					case "innerhtml":
					case "html":
						outValue = Element.GetAttribute("innerHTML");
						break;

					default:
						outValue = Element.GetAttribute(attribute2Get);
						break;
				}
			}
			catch (Exception e)
			{
				LogMsg = $"ERROR:\tThe attribute provided [{attribute2Get}] does not match a valid type for element [{Element}]. \n{e}";
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
				outValue = Element.GetProperty(property2Get);
			}
			catch (Exception e)
			{
				LogMsg = $"ERROR:\tThe attribute provided [{property2Get}] does not match a valid type for element [{Element}] . \n{e}";
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
				Element.SendKeys(sendValue);
			}
			catch (Exception e)
			{
				LogMsg = $"Unable to Send text [{sendValue}] to Element [{Element}]\n{e}";
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

				var testItem = ((IJavaScriptExecutor)Driver).ExecuteScript(jScript);
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
			switch (LocatorStrategy.ToLower())
			{
				case ("xpath"):
					jsOutString = $"document.evaluate(\"{ElementSelector}\", document,null, XPathResult.ANY_TYPE,null).FIRST_ORDERED_NODE_TYPE";
					break;
				case ("css"):
				case ("cssselector"):
					jsOutString = $"document.querySelector(\"{ElementSelector}\");";
					break;
				case ("id"):
					jsOutString = $"document.getElementById(\"{ElementSelector}\");";
					break;
				case ("name"):
					jsOutString = $"document.getElementsByName(\"{ElementSelector}\");";
					break;
				case ("classname"):
					jsOutString = $"document.getElementsByClassName(\"{ElementSelector}\");";
					break;
				case ("tagname"):
					jsOutString = $"document.getElementsByTagName(\"{ElementSelector}\");";
					break;

				default:
					LogMsg = "Locator Strategy does not match viable options. ";
					logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
					throw new Exception(LogMsg);

			}

			if (jsOutString == "")
			{
				LogMsg = $"Location Strategy [{LocatorStrategy}] not supported.";
				logger.Write(LogMsg, funcName, LogConstants.LOG_ERROR);
				throw new Exception(LogMsg);
			}

			return jsOutString;
		}




		//  ---------------------------------------------------------------
		//  Table Interactions (FE)
		//  ---------------------------------------------------------------
		//  ---------------------------------------------------------------


		public void Table2List(int headerRow = 1,bool textOnly = true)
		{
			string funcName = "Table2List";
			//			throw new Exception("Not Yet Implemented");


			try
			{
	
				// Declare Variables
				By tableRowsSelector = By.XPath("//tr");
				By tableHeadersSelector = By.XPath("//th");
				By tableCellsLocator = By.XPath("//td");
				List<IWebElement> tableRowsElements = new List<IWebElement>();		// The Rows for the Table
//				List<IWebElement> tableHeaderElements = new List<IWebElement>();	// The Header Rowfor the table
				List<IWebElement> tableCellsElements = new List<IWebElement>();		// The Cells for the current Row
				List<TableListItem> tableContents = new List<TableListItem>();		// The table in List form with all Rows as a new list item 
				List<string> tableCellsText = new List<string>();

				// Get <TR> tags
				tableRowsElements.Clear();
				tableRowsElements.Add((IWebElement)Element.FindElements(tableRowsSelector));

				for (int rowNum = headerRow - 1; tableRowsElements.Count -1 > rowNum;rowNum++)
				{

					//Check for Header
					tableCellsElements.Clear();
					tableCellsElements.Add((IWebElement)tableRowsElements[rowNum].FindElements(tableHeadersSelector));

					if (tableCellsElements.Count > 0)
					{
						// Get Header Row Contents
//						for (int cellNum = 0;cellNum < tableCellsElements.Count - 1; cellNum++)
						foreach (var curTableCell in tableCellsElements)
						{
							tableCellsText.Add(curTableCell.Text);

						}
						tableContents.Add(new TableListItem(tableCellsText));
						
					}


				}




				string tableHTML = GetAttribute("innerHTML");
				logger.Write(tableHTML, funcName);

			}
			catch (Exception e)
			{
				LogMsg = $"";
				logger.Write(LogMsg, funcName, LogConstants.LOG_WARNING);
				throw new Exception(LogMsg);
			}
		}


	}


}
