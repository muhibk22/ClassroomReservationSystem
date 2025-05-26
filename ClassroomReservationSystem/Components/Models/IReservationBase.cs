using ClassroomReservationSystem.Components.Models;
using System.ComponentModel.DataAnnotations;

public abstract class ReservationBase
{
    public int ReservationId { get; set; }
    [Required(ErrorMessage = "Please Select A Lecture")]
    public AssignedCourse Course { get; set; }

    [Required(ErrorMessage = "Please Select A Classroom")]
    public Classroom Classroom { get; set; }


    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }





}
