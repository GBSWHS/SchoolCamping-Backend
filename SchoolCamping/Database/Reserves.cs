using System;
using System.Collections.Generic;

namespace SchoolCamping.Database
{
    public partial class Reserves
    {
        public int Id { get; set; }
        public DateOnly ReservedAt { get; set; }
        public string? Mates { get; set; }
        public string Passcode { get; set; } = null!;
        public string? Teacher { get; set; }
    }
}
