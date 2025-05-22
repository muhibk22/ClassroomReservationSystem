using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class ScheduledClass : ReservationBase
    {
        public DayOfWeek Day { get; set; }

        public override bool ConflictsWith(ScheduledClass other)
        {
            if (Day != other.Day || ClassroomId != other.ClassroomId)
                return false;

            return TimeOverlap(TimeSlots, other.TimeSlots);
        }

        public override bool ConflictsWith(Reservation other)
        {
            if ((int)Day != (int)other.Date.DayOfWeek || ClassroomId != other.ClassroomId)
                return false;

            return TimeOverlap(TimeSlots, other.TimeSlots);
        }

        private bool TimeOverlap(int slots1, int slots2)
        {
            return (slots1 & slots2) != 0;
        }
    }

}
