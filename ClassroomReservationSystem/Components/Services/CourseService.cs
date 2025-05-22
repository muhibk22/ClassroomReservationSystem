using ClassroomReservationSystem.Components.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassroomReservationSystem.Components.Services
{
    public class CourseService
    {
        private readonly string _connectionString;

        public CourseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            List<Course> courses = new();

            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Courses";

            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                courses.Add(new Course
                {
                    CourseId = reader.GetInt32(0),
                    CourseCode = reader.GetString(1),
                    Title = reader.GetString(2),
                    PreReqCode = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }

            return courses;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Courses WHERE CourseId = @courseId";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@courseId", courseId);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Course
                {
                    CourseId = reader.GetInt32(0),
                    CourseCode = reader.GetString(1),
                    Title = reader.GetString(2),
                    PreReqCode = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }

            return null;
        }

        public async Task<bool> AddCourseAsync(Course course)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
        INSERT INTO Courses (CourseCode, Title, PreReqCode)
        VALUES (@code, @title, @preReq)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@code", course.CourseCode);
            cmd.Parameters.AddWithValue("@title", course.Title);
            cmd.Parameters.AddWithValue("@preReq", (object?)course.PreReqCode ?? DBNull.Value);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
        UPDATE Courses
        SET CourseCode = @code, Title = @title, PreReqCode = @preReq
        WHERE CourseId = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", course.CourseId);
            cmd.Parameters.AddWithValue("@code", course.CourseCode);
            cmd.Parameters.AddWithValue("@title", course.Title);
            cmd.Parameters.AddWithValue("@preReq", (object?)course.PreReqCode ?? DBNull.Value);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "DELETE FROM Courses WHERE CourseId = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", courseId);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> CourseCodeExistsAsync(string courseCode)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT COUNT(1) FROM Courses WHERE CourseCode = @code";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@code", courseCode);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}