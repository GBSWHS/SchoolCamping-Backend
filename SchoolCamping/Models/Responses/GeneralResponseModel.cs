namespace SchoolCamping.Models.Responses
{
    public class GeneralResponseModel
    {
        public bool Success { get; set; } = true;
        public object? Data { get; set; }
        public string? Message { get; set; }
    }
}