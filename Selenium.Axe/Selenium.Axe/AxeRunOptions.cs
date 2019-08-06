using Newtonsoft.Json;
using System.Collections.Generic;

namespace Selenium.Axe
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RunOnlyOptions
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }

    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RuleOptions
    {
        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AxeRunOptions
    {
        [JsonProperty("runOnly")]
        public RunOnlyOptions RunOnly { get; set; }

        [JsonProperty("rules")]
        public Dictionary<string, RuleOptions> Rules { get; set; }

    }
}
