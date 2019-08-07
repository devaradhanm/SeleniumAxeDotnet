using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using Selenium.Axe.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeBuilderTest
    {
        private Mock<IWebDriver> webDriverMock;
        private Mock<IJavaScriptExecutor> jsExecutorMock;
        private Mock<ITargetLocator> targetLocatorMock;
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly string testAxeResult = JsonConvert.SerializeObject(new
        {
            results = new
            {
                violations = new object[] { },
                passes = new object[] { },
                inapplicable = new object[] { },
                incomplete = new object[] { },
                timestamp = DateTimeOffset.Now,
                url = "www.test.com",
            }
        });

        [TestInitialize]
        public void TestInitialize()
        {
            webDriverMock = new Mock<IWebDriver>();
            jsExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
            targetLocatorMock = new Mock<ITargetLocator>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowWhenDriverIsNull()
        {
            //arrange / act /assert
            var axeBuilder = new AxeBuilder(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowWhenOptionsAreNull()
        {
            //arrange
            var driver = new Mock<IWebDriver>();

            // act / assert
            var axeBuilder = new AxeBuilder(driver.Object, null);
        }

        [TestMethod]
        public void ShouldHandleIfOptionsAndContextNotSet()
        {

            SetupAxeInjectionCall();
            SetupScanCall(null,"{}");

            var builder = new AxeBuilder(webDriverMock.Object);
            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();

        }

        [TestMethod]
        public void ShouldPassContextIfIncludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupAxeInjectionCall();
            SetupScanCall(expectedContext.ToString(), "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Include("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldPassContextIfExcludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Exclude = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupAxeInjectionCall();
            SetupScanCall(expectedContext.ToString(), "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Exclude("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [TestMethod]
        public void ShouldPassRuleConfig()
        {
            var expectedRules = new List<string> { "rule1", "rule2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "rule",
                    Values = expectedRules
                },
                Rules = new Dictionary<string, RuleOptions>()
                {
                   { "excludeRule1", new RuleOptions(){ Enabled = false} },
                   { "excludeRule2", new RuleOptions(){ Enabled = false } }
                }

            });

            SetupAxeInjectionCall();
            SetupScanCall(null, expectedOptions.ToString());

            var builder = new AxeBuilder(webDriverMock.Object)
                .DisableRules("excludeRule1", "excludeRule2")
                .WithRules("rule1", "rule2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldPassRunOptionsWithTagConfig()
        {
            var expectedTags = new List<string> { "tag1", "tag2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = expectedTags
                },
            });

            SetupAxeInjectionCall();
            SetupScanCall(null, expectedOptions.ToString());

            var builder = new AxeBuilder(webDriverMock.Object)
                .WithTags("tag1", "tag2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        private void VerifyAxeResult(AxeResult result)
        {
            result.Should().NotBeNull();
            result.Inapplicable.Should().NotBeNull();
            result.Incomplete.Should().NotBeNull();
            result.Passes.Should().NotBeNull();
            result.Violations.Should().NotBeNull();

            result.Inapplicable.Length.Should().Be(0);
            result.Incomplete.Length.Should().Be(0);
            result.Passes.Length.Should().Be(0);
            result.Violations.Length.Should().Be(0);
        }
        private void SetupAxeInjectionCall()
        {
            webDriverMock
          .Setup(d => d.FindElements(It.IsAny<By>()))
          .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0)));

            webDriverMock.Setup(d => d.SwitchTo()).Returns(targetLocatorMock.Object);

            jsExecutorMock
                .Setup(js => js.ExecuteScript(Resources.axe_min)).Verifiable();

        }

        private void SetupScanCall(string serializedContext, string serialzedOptions)
        {
            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                Resources.scan,
                It.Is<string>(context => context == serializedContext),
                It.Is<string>(options => options == serialzedOptions))).Returns(testAxeResult);
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

    }
}
