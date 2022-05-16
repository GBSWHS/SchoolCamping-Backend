using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SchoolCamping.Models.Requests
{
    public class ReserveRequestModel
    {
        [MaxLength(50)]
        [JsonPropertyName("mates")]
        public string Mates { get; set; }

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [MaxLength(4)]
        [JsonPropertyName("pass")]
        public string PassCode { get; set; }

        [MaxLength(4)]
        [JsonPropertyName("teacher")]
        public string Teacher { get; set; }
    }
}
