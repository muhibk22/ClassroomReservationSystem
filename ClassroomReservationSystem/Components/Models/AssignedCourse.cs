using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class AssignedCourse
    {
        public int AssignedCourseId { get; set; }
        public int InstructorId { get; set; }
        public int CourseId { get; set; }

        public User? Instructor { get; set; }
        public Course? Course { get; set; }
    }

}
