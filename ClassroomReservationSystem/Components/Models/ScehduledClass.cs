using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class ScheduledClass : ReservationBase
    {
        [Required]
        public DayOfWeek Day { get; set; }

        public override bool ConflictsWith(ScheduledClass other)
        {
            if (Day != other.Day || ClassroomId != other.ClassroomId)
                return false;

            return TimeOverlap(StartTime, EndTime, other.StartTime, other.EndTime);
        }

        public override bool ConflictsWith(Reservation other)
        {
            if ((int)Day != (int)other.Date.DayOfWeek || ClassroomId != other.ClassroomId)
                return false;

            return TimeOverlap(StartTime, EndTime, other.StartTime, other.EndTime);
        }

        private bool TimeOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }
    }


}
