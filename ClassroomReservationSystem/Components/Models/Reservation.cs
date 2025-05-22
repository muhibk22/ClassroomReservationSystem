using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class Reservation : ReservationBase
    {
        public DateTime Date { get; set; }
        public string Purpose { get; set; }
        public int ReservedByUserId { get; set; }

        public override bool ConflictsWith(ScheduledClass other)
        {
            if ((int)other.Day != (int)Date.DayOfWeek || other.ClassroomId != ClassroomId)
                return false;

            return TimeOverlap(TimeSlots, other.TimeSlots);
        }

        public override bool ConflictsWith(Reservation other)
        {
            if (Date != other.Date || ClassroomId != other.ClassroomId)
                return false;

            return TimeOverlap(TimeSlots, other.TimeSlots);
        }

        private bool TimeOverlap(int slots1, int slots2)
        {
            return (slots1 & slots2) != 0; // bitwise overlap check
        }
    }

}
