using ClassroomReservationSystem.Components.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassroomReservationSystem.Services
{
    public class ReservationService
    {
        private readonly string _connectionString;

        public ReservationService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            List<Reservation> reservations = new();

            using SqlConnection conn = new(_connectionString);
            string query = @"
                SELECT r.ReservationId, r.Date, r.StartTime, r.EndTime,
                       r.AssignedCourseId, ac.InstructorId, ac.CourseId,
                       u.FullName, u.UserName, u.Role,
                       c.CourseCode, c.Title, c.PreReqCode,
                       cr.ClassroomId, cr.RoomNumber, cr.Capacity, cr.Department
                FROM Reservations r
                JOIN AssignedCourses ac ON r.AssignedCourseId = ac.AssignedCourseId
                JOIN Users u ON ac.InstructorId = u.UserId
                JOIN Courses c ON ac.CourseId = c.CourseId
                JOIN Classrooms cr ON r.ClassroomId = cr.ClassroomId";

            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Reservation reservation = new()
                {
                    ReservationId = reader.GetInt32(reader.GetOrdinal("ReservationId")),
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                    Classroom = new Classroom
                    {
                        ClassroomId = reader.GetInt32(reader.GetOrdinal("ClassroomId")),
                        RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                        Department = reader.GetString(reader.GetOrdinal("Department"))
                    },
                    Course = new AssignedCourse
                    {
                        AssignedCourseId = reader.GetInt32(reader.GetOrdinal("AssignedCourseId")),
                        InstructorId = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        Instructor = new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Role = (UserRole)reader.GetInt32(reader.GetOrdinal("Role"))
                        },
                        Course = new Course
                        {
                            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                            CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            PreReqCode = reader.IsDBNull(reader.GetOrdinal("PreReqCode"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PreReqCode"))
                        }
                    }
                };

                reservations.Add(reservation);
            }

            return reservations;
        }

        public async Task<bool> AddReservationAsync(Reservation reservation)
        {
            bool exists = await ReservationExistsAsync(reservation.Classroom.ClassroomId, reservation.Date, reservation.StartTime, reservation.EndTime);
            if (exists)
            {
                return false;
            }

            using SqlConnection conn = new(_connectionString);
            string query = @"
                INSERT INTO Reservations (AssignedCourseId, ClassroomId, Date, StartTime, EndTime)
                VALUES (@assignedCourseId, @classroomId, @date, @startTime, @endTime)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@assignedCourseId", reservation.Course.AssignedCourseId);
            cmd.Parameters.AddWithValue("@classroomId", reservation.Classroom.ClassroomId);
            cmd.Parameters.AddWithValue("@date", reservation.Date);
            cmd.Parameters.AddWithValue("@startTime", reservation.StartTime);
            cmd.Parameters.AddWithValue("@endTime", reservation.EndTime);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "DELETE FROM Reservations WHERE ReservationId = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", reservationId);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ReservationExistsAsync(int classroomId, DateTime date, TimeSpan start, TimeSpan end)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
                SELECT COUNT(*)
                FROM Reservations
                WHERE ClassroomId = @classroomId AND Date = @date
                      AND StartTime = @startTime AND EndTime = @endTime";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@classroomId", classroomId);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@startTime", start);
            cmd.Parameters.AddWithValue("@endTime", end);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}
