using Newtonsoft.Json;
using System.Collections.Generic;

namespace Selenium.Axe
{
    [JsonObject]
    public class RunOnlyOptions
    {
        /// <summary>
        /// Specifies the context for runonly option. (can be "rule" or "tag")
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Has rules / tags that needs to be executed. (context is based on <see cref="Type"/>
        /// </summary>
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
        /// <summary>
        ///  Limit which rules are executed, based on names or tags
        /// </summary>
        [JsonProperty("runOnly")]
        public RunOnlyOptions RunOnly { get; set; }

        /// <summary>
        /// Allow customizing a rule's properties (including { enable: false })
        /// </summary>
        [JsonProperty("rules")]
        public Dictionary<string, RuleOptions> Rules { get; set; }

    }
}
