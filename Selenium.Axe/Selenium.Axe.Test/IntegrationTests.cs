using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace Selenium.Axe.Test
{
    [TestClass]
    [TestCategory("Integration")]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private const string TargetTestUrl = "https://www.google.ca/";

        private const string SearchTextBoxSelector = "//input[@title='Search']";

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver?.Quit();
            _webDriver?.Dispose();
        }


        [TestMethod]
        [DataRow("Chrome")]
        //[DataRow("Firefox")]
        public void TestAnalyzeTarget(string browser)
        {
            var expectedToolOptions = new AxeRunOptions()
            {
                Rules = new Dictionary<string, RuleOptions>()
                {
                    {"color-contrast", new RuleOptions{ Enabled = false} }
                },
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = new List<string>() { "wcag2a" }
                }
            };

            this.InitDriver(browser);
            LoadTestPage();

            var builder = new AxeBuilder(_webDriver).WithTags("wcag2a").DisableRules("color-contrast");

            var results = builder.Analyze();
            results.Should().NotBeNull(nameof(results));
            JsonConvert.SerializeObject(results.ToolOptions).Should().Be(JsonConvert.SerializeObject(expectedToolOptions));


        }

        [TestMethod]
        [DataRow("Chrome")]
        //[DataRow("Firefox")]
        public void RunScanOnGivenElement(string browser)
        {
            this.InitDriver(browser);
            LoadTestPage();

            AxeResult results = _webDriver.Analyze(_webDriver.FindElement(By.XPath(SearchTextBoxSelector)));
            results.Should().NotBeNull(nameof(results));
        }


        private void LoadTestPage()
        {
            _webDriver.Navigate().GoToUrl(TargetTestUrl);

            // wait for email input box is found
            _wait.Until(drv => drv.FindElement(By.XPath(SearchTextBoxSelector)));
        }
        private void InitDriver(string browser)
        {
            switch (browser.ToUpper())
            {
                case "CHROME":
                    ChromeOptions options = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    options.AddArgument("no-sandbox");
                    options.AddArgument("--log-level=3");
                    options.AddArgument("--silent");

                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
                    service.SuppressInitialDiagnosticInformation = true;
                    _webDriver = new ChromeDriver(Environment.CurrentDirectory, options);

                    break;

                case "FIREFOX":
                    _webDriver = new FirefoxDriver();
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            _wait = new WebDriverWait(_webDriver, TimeSpan.FromMinutes(4));
            _webDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(3);
            _webDriver.Manage().Window.Maximize();
        }
    }
}
