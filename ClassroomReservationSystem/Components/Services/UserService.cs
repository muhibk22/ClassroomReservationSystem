using Microsoft.Data.SqlClient;

namespace ClassroomReservationSystem.Components.Services
{
    public class UserService 
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Users WHERE UserName = @username AND Password = @password";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password); // NOTE: Use hashed passwords in production
            Console.WriteLine("Executed sql command");
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    UserName = reader.GetString(2),
                    Password = reader.GetString(3),
                    Role = (UserRole)reader.GetInt32(4)
                };
            }

            return null;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Users";
            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    UserName = reader.GetString(2),
                    Password = reader.GetString(3),
                    Role = (UserRole)reader.GetInt32(4)
                });
            }

            return users;
        }
        public async Task<List<User>> GetAllFacultyAsync()
        {
            var users = new List<User>();
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Users where Role= 1";
            using SqlCommand cmd = new(query, conn);
            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    UserName = reader.GetString(2),
                    Password = reader.GetString(3),
                    Role = (UserRole)reader.GetInt32(4)
                });
            }

            return users;
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM Users WHERE UserId = @id";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await conn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    UserName = reader.GetString(2),
                    Password = reader.GetString(3),
                    Role = (UserRole)reader.GetInt32(4)
                };
            }

            return null;
        }

        public async Task<bool> RegisterUserAsync(User user, UserRole role)
        {
            using SqlConnection conn = new(_connectionString);
            string query = @"
            INSERT INTO Users (FullName, UserName, Password, Role)
            VALUES (@fullName, @userName, @password, @role)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@fullName", user.FullName);
            cmd.Parameters.AddWithValue("@userName", user.UserName);
            cmd.Parameters.AddWithValue("@password", user.Password); // Hashing recommended
            cmd.Parameters.AddWithValue("@role", (int)role);

            await conn.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

}
