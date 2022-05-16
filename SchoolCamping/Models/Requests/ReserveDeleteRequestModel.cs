using System.Text.Json.Serialization;

namespace SchoolCamping.Models.Requests
{
    public class ReserveDeleteRequestModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("pass")]
        public string PassCode { get; set; }
    }
}
