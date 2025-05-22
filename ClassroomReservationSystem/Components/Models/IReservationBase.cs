using ClassroomReservationSystem.Components.Models;
using System.ComponentModel.DataAnnotations;

public abstract class ReservationBase
{
    public int ReservationId { get; set; }
    [Required(ErrorMessage = "Please Select A Lecture")]
    public AssignedCourse Course { get; set; }

    [Required(ErrorMessage = "Please Select A Classroom")]
    public int ClassroomId { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

   public int TimeSlots { get; set; }

    public abstract bool ConflictsWith(ScheduledClass other);
    public abstract bool ConflictsWith(Reservation other);
    protected int CalculateTimeSlots(TimeSpan start, TimeSpan end)
    {
        double totalMinutes = (end - start).TotalMinutes;
        return totalMinutes > 80 ? 2 : 1;
    }

}
