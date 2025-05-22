namespace ClassroomReservationSystem.Components.Models
{
    public abstract class ReservationBase
    {
        public int ReservationId { get; set; }
        public AssignedCourse? Course { get; set; }
        public int ClassroomId { get; set; }
        public int TimeSlots { get; set; }

        public abstract bool ConflictsWith(ScheduledClass other);
        public abstract bool ConflictsWith(Reservation other);
    }

}
