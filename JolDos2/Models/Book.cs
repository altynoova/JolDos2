namespace JolDos2.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string BookedPassengerId { get; set; }
        public string AcceptedPassengerId { get; set; }

    }
}
