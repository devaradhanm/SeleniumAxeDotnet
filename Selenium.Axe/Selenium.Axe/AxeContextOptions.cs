using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Axe
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AxeContextOptions
    {
        [JsonProperty("include")]
        public string[][] Include { get; set; }
        [JsonProperty("exclude")]
        public string[][] Exclude { get; set; }
    }
}
