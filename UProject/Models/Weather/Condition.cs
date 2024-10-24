using Newtonsoft.Json;

namespace UProject.Models.Weather
{
    public class Condition
    {
        [JsonProperty("text")]
        public string Description { get; set; }
    }
}
