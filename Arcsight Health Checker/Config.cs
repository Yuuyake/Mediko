using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcsight_Health_Checker {
    public partial class Config {
        [JsonProperty("loggerID")]
        public string proxyAdress { get; set; }

        [JsonProperty("loggerPass")]
        public string proxyUsername { get; set; }

        [JsonProperty("csirtMail")]
        public string csirtMail { get; set; }

        [JsonProperty("firefoxLoc")]
        public string proxyAdress { get; set; }

        [JsonProperty("loggerUrls")]
        public List<string> ibmApiKeys { get; set; }
        /// <summary>
        /// 
        /// </summary>
    }
}
