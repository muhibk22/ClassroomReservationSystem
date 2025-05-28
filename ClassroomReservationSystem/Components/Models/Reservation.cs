using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class Reservation : ReservationBase
    {
        public DateTime Date { get; set; }
        public int ReservedByUserId { get; set; }
    }

}
