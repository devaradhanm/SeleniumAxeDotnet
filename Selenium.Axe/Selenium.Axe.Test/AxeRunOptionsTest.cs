using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeRunOptionsTest
    {
        [TestMethod]
        public void ShouldSerializeRunOnlyOption()
        {
            var options = new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = new List<string> { "tag1", "tag2" }
                }
            };

            var serializedObject = JsonConvert.SerializeObject(options);
            var expectedObject = "{\"runOnly\":{\"type\":\"tag\",\"values\":[\"tag1\",\"tag2\"]}}";
            serializedObject.Should().Be(expectedObject);
        }

        [TestMethod]
        public void ShouldSerializeRuleOptions()
        {
            var options = new AxeRunOptions()
            {
                Rules = new Dictionary<string, RuleOptions>
                {
                    {"enabledRule", new RuleOptions(){ Enabled = true} },
                    {"disabledRule", new RuleOptions(){ Enabled = false} },
                    {"rule3WithoutOptionsData", new RuleOptions(){ } },
                }
            };

            var serializedObject = JsonConvert.SerializeObject(options);

            var expectedObject = "{\"rules\":{\"enabledRule\":{\"enabled\":true},\"disabledRule\":{\"enabled\":false},\"rule3WithoutOptionsData\":{}}}";
            serializedObject.Should().Be(expectedObject);
        }
    }
}
