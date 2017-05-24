using Newtonsoft.Json;
namespace CosmosHackApi.Models
{
    public class Dog
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("furColor")]
        public string FurColor { get; set; }
    }
}