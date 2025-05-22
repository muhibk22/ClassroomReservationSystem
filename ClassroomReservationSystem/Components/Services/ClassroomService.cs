using ClassroomReservationSystem.Components.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassroomReservationSystem.Components.Services
{
    public class ClassroomService
    {
        private readonly string _connectionString;

        public ClassroomService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Add new classroom
        public async Task<bool> AddClassroomAsync(Classroom classroom)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
                INSERT INTO Classrooms (RoomNumber, Capacity, Department)
                VALUES (@roomNumber, @capacity, @department)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@roomNumber", classroom.RoomNumber);
            cmd.Parameters.AddWithValue("@capacity", classroom.Capacity);
            cmd.Parameters.AddWithValue("@department", classroom.Department);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        // Delete classroom by ID
        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "DELETE FROM Classrooms WHERE ClassroomId = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", classroomId);

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        // Get all classrooms
        public async Task<List<Classroom>> GetAllClassroomsAsync()
        {
            List<Classroom> classrooms = new();

            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Classrooms ORDER BY RoomNumber";

            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                classrooms.Add(new Classroom
                {
                    ClassroomId = reader.GetInt32(reader.GetOrdinal("ClassroomId")),
                    RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                    Department = reader.GetString(reader.GetOrdinal("Department"))
                });
            }

            return classrooms;
        }

        // Check if a classroom already exists (by RoomNumber)
        public async Task<bool> ClassroomExistsAsync(string roomNumber)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT COUNT(*) FROM Classrooms WHERE RoomNumber = @roomNumber";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@roomNumber", roomNumber);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}
