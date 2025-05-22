using ClassroomReservationSystem.Components.Models;
using Microsoft.Data.SqlClient;
using System.Data;
namespace ClassroomReservationSystem.Components.Services
{
    public class AssignedCourseService
    {
        private readonly string _connectionString;

        public AssignedCourseService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<bool> AssignCourseAsync(int instructorId, int courseId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
        INSERT INTO AssignedCourses (InstructorId, CourseId)
        VALUES (@instructorId, @courseId)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@instructorId", instructorId);
            cmd.Parameters.AddWithValue("@courseId", courseId);

            await conn.OpenAsync();
            try
            {
                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                return false;
            }
        }

        public async Task<List<AssignedCourse>> GetAllAssignedCoursesAsync()
        {
            List<AssignedCourse> list = new();

            using SqlConnection conn = new(_connectionString);
            string query = @"
        SELECT ac.AssignedCourseId, ac.InstructorId, ac.CourseId,
               u.FullName, u.UserName, u.Role,
               c.CourseCode, c.Title, c.PreReqCode
        FROM AssignedCourses ac
        JOIN Users u ON ac.InstructorId = u.UserId
        JOIN Courses c ON ac.CourseId = c.CourseId";

            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new AssignedCourse
                {
                    AssignedCourseId = reader.GetInt32(0),
                    InstructorId = reader.GetInt32(1),
                    CourseId = reader.GetInt32(2),
                    Instructor = new User
                    {
                        UserId = reader.GetInt32(1),
                        FullName = reader.GetString(3),
                        UserName = reader.GetString(4),
                        Role = (UserRole)reader.GetInt32(5)
                    },
                    Course = new Course
                    {
                        CourseId = reader.GetInt32(2),
                        CourseCode = reader.GetString(6),
                        Title = reader.GetString(7),
                        PreReqCode = reader.IsDBNull(8) ? null : reader.GetString(8)
                    }
                });
            }

            return list;
        }

        public async Task<bool> DeleteAssignedCourseAsync(int id, int courseId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "DELETE FROM AssignedCourses WHERE AssignedCourseId = @id AND CourseId= @courseId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@CourseId", courseId);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<bool> IsCourseAlreadyAssignedAsync(int instructorId, int courseId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT COUNT(1) FROM AssignedCourses WHERE InstructorId = @instructor AND CourseId = @course";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@instructor", instructorId);
            cmd.Parameters.AddWithValue("@course", courseId);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}