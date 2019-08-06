using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeContextOptionsTest
    {
        [TestMethod]
        public void ShouldSerializeEmptyObject()
        {
            var context = new AxeContextOptions();
            JsonConvert.SerializeObject(context).Should().Be("{}");

        }

        [TestMethod]
        public void ShouldSerializeObjectWithPropertiesDefined()
        {
            var context = new AxeContextOptions()
            {
                Include = new string[][] { new string[] { "#if1", "#idiv1" } },
                Exclude = new string[][] { new string[] { "#ef1", "#ediv1" } }
            };

            var expectedContent = "{\"include\":[[\"#if1\",\"#idiv1\"]],\"exclude\":[[\"#ef1\",\"#ediv1\"]]}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }
    }
}
