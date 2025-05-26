using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class ScheduledClass : ReservationBase
    {
        [Required]
        public DayOfWeek Day { get; set; }

    }


}
