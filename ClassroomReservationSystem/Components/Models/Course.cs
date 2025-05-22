using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course code is required")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string? PreReqCode { get; set; }
    }
}
