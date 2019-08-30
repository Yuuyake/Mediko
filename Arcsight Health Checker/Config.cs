using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcsight_Health_Checker {
    public partial class Config {
        [JsonProperty("loggerID")]
        public string loggerID { get; set; }

        [JsonProperty("loggerPass")]
        public string loggerPass { get; set; }

        [JsonProperty("csirtMail")]
        public string csirtMail { get; set; }

        [JsonProperty("firefoxLoc")]
        public string firefoxLoc { get; set; }

        [JsonProperty("loggerUrls")]
        public List<string> loggerUrls { get; set; }
        /// <summary>
        /// 
        /// </summary>
    }
}
