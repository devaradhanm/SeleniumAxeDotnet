using Newtonsoft.Json;
using System.Collections.Generic;

namespace Selenium.Axe
{
    [JsonObject]
    public class RunOnlyOptions
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }

    }

    [JsonObject]
    public class RuleOptions
    {
        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }

    [JsonObject]
    public class AxeRunOptions
    {
        [JsonProperty("runOnly")]
        public RunOnlyOptions RunOnly { get; set; }

        [JsonProperty("rules")]
        public Dictionary<string, RuleOptions> Rules { get; set; }

    }
}
