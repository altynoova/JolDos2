using System.ComponentModel.DataAnnotations;

namespace JolDos2.Models
{
    public class DriverViewModel
    {
            [Key]
            public int Id { get; set; }
            public int Trip_id { get; set; }
            public string FromLoc { get; set; }
            public string ToLoc { get; set; }
            public DateTime DateOfTrip { get; set; }
            public int Seats { get; set; }
            public string Fare { get; set; }
            public string AcceptedPassengers { get; set; }
            public string BookedPassengers { get; set; }

    }
}
