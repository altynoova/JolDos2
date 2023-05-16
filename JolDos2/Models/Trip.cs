using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JolDos2.Models
{
    [Table("Trip")]
    public class Trip
    {
        [Key]
        public int Trip_id { get; set; }
        public string DriverId { get; set; }

        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime DateOfTrip { get; set; }
        public int Seats { get; set; }
        public string Fare { get; set; }
        public string AutoInf { get; set; }
        public bool TripStatus { get; set; }

    }
}
