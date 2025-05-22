using System.ComponentModel.DataAnnotations;

namespace ClassroomReservationSystem.Components.Models
{
    public class Classroom
    {
        public int ClassroomId { get; set; }
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }
        public string Department { get; set; }

    }

}
