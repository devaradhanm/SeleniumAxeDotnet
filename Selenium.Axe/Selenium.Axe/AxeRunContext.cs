using Newtonsoft.Json;
using System.Collections.Generic;

namespace Selenium.Axe
{
    [JsonObject()]
    public class AxeRunContext
    {
        [JsonProperty("include")]
        public List<string[]> Include { get; set; }
        [JsonProperty("exclude")]
        public List<string[]> Exclude { get; set; }
    }
}
