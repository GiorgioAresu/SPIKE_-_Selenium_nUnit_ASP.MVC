using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Selenium.Tests
{
    [TestFixture]
    public class IndexFixtures
    {
        private Process _iisProcess;
        private int _port = 6006;
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;

        [TestFixtureSetUp]
        public void Setup()
        {
            var thread = new Thread(StartIisExpress) { IsBackground = true };

            thread.Start();
        }

        private void StartIisExpress()
        {
            var thisPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var appPath = new Uri(System.IO.Path.Combine(thisPath, @"..\..\..\Selenium")).LocalPath;
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = string.Format("/path:\"{0}\" /port:{1}", appPath, _port)
            };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                                ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = $@"{programfiles}\IIS Express\iisexpress.exe";

            try
            {
                _iisProcess = new Process { StartInfo = startInfo };

                _iisProcess.Start();
                _iisProcess.WaitForExit();
            }
            catch
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }

        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
            baseURL = $"http://localhost:{_port}/";
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [TestCase("Application name", "Home Page")]
        [TestCase("Home", "Home Page")]
        [TestCase("About", "About")]
        [TestCase("Contact", "Contact")]
        [TestCase("Register", "Register")]
        [TestCase("Log in", "Log in")]
        public void NavbarLinks(string linkName, string title)
        {
            driver.Navigate().GoToUrl(baseURL);
            driver.FindElement(By.LinkText(linkName)).Click();
            Assert.True(driver.Title.StartsWith(title));
        }
    }
}