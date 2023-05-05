using System.ComponentModel.DataAnnotations.Schema;

namespace JolDos2.Models
{
    [Table("Trip")]
    public class Trip
    {
        public int Id { get; set; }
        public string DriverId { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public DateTime DateOfTrip { get; set; }
        public int Seats { get; set; }
        public string Fare { get; set; }
        public string AutoInf { get; set; }
        public bool Status { get; set; }
    }
}
