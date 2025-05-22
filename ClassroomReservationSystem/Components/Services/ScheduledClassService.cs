using ClassroomReservationSystem.Components.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassroomReservationSystem.Services
{
    public class ScheduledClassService
    {
        private readonly string _connectionString;

        public ScheduledClassService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<ScheduledClass>> GetAllScheduledClassesAsync()
        {
            List<ScheduledClass> classes = new();

            using SqlConnection conn = new(_connectionString);
            string query = @"
        SELECT sc.ScheduledClassId, sc.ClassroomId, sc.Day, sc.StartTime, sc.EndTime,
               sc.AssignedCourseId, ac.InstructorId, ac.CourseId,
               u.FullName, u.UserName, u.Role,
               c.CourseCode, c.Title, c.PreReqCode
        FROM ScheduledClasses sc
        JOIN AssignedCourses ac ON sc.AssignedCourseId = ac.AssignedCourseId
        JOIN Users u ON ac.InstructorId = u.UserId
        JOIN Courses c ON ac.CourseId = c.CourseId";

            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ScheduledClass scheduledClass = new()
                {
                    ReservationId = reader.GetInt32(reader.GetOrdinal("ScheduledClassId")),
                    ClassroomId = reader.GetInt32(reader.GetOrdinal("ClassroomId")),
                    Day = (DayOfWeek)reader.GetInt32(reader.GetOrdinal("Day")),
                    StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                    TimeSlots = CalculateTimeSlots(reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                                reader.GetTimeSpan(reader.GetOrdinal("EndTime"))),
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
                                         ? null : reader.GetString(reader.GetOrdinal("PreReqCode"))
                        }
                    }
                };

                classes.Add(scheduledClass);
            }

            return classes;
        }


        public async Task<bool> AddScheduledClassAsync(ScheduledClass sc)
        {
            
            bool exists = await ScheduledClassExistsAsync(sc.ClassroomId, sc.Day, sc.StartTime, sc.EndTime);
            if (exists)
            {
                return false; // Conflict detected — do not insert
            }

            using SqlConnection conn = new(_connectionString);
            string query = @"
        INSERT INTO ScheduledClasses (AssignedCourseId, ClassroomId, Day, StartTime, EndTime)
        VALUES (@assignedCourseId, @classroomId, @day, @startTime, @endTime)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@assignedCourseId", sc.Course!.AssignedCourseId);
            cmd.Parameters.AddWithValue("@classroomId", sc.ClassroomId);
            cmd.Parameters.AddWithValue("@day", (int)sc.Day);
            cmd.Parameters.AddWithValue("@startTime", sc.StartTime);
            cmd.Parameters.AddWithValue("@endTime", sc.EndTime);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }



        public async Task<bool> DeleteScheduledClassAsync(int scheduledClassId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "DELETE FROM ScheduledClasses WHERE ScheduledClassId = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", scheduledClassId);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ScheduledClassExistsAsync(int classroomId, DayOfWeek day, TimeSpan start, TimeSpan end)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
                SELECT COUNT(*)
                FROM ScheduledClasses
                WHERE ClassroomId = @classroomId AND Day = @day
                  AND ((StartTime = @startTime AND EndTime = @endTime))";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@classroomId", classroomId);
            cmd.Parameters.AddWithValue("@day", (int)day);
            cmd.Parameters.AddWithValue("@startTime", start);
            cmd.Parameters.AddWithValue("@endTime", end);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
        private int CalculateTimeSlots(TimeSpan start, TimeSpan end)
        {
            double totalMinutes = (end - start).TotalMinutes;
            return totalMinutes > 80 ? 2 : 1;
        }

    }
}
