using System.Text.Json;
using Microsoft.JSInterop;

namespace ClassroomReservationSystem.Components.Services
{
    public class UserState
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "UserStateStorage";

        public User? CurrentUser { get; private set; }

        public event Action? OnChange;

        public bool IsLoggedIn => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
        public bool IsFaculty => CurrentUser?.Role == UserRole.Faculty;

        public UserState(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // to keep state after refresh
        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Inside initialize async");
                var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
                Console.WriteLine($"Read from sessionStorage: {json}");
                if (!string.IsNullOrEmpty(json))
                {
                    var user = JsonSerializer.Deserialize<User>(json);
                    if (user != null)
                    {
                        CurrentUser = user;
                        OnChange?.Invoke();
                    }
                    else
                    {
                        Console.WriteLine("user already set");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading user from sessionStorage: {ex.Message}");
            }
        }

        public async Task SetUserAsync(User user)
        {
            CurrentUser = user;
            OnChange?.Invoke();

            try
            {
                var json = JsonSerializer.Serialize(user);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
                Console.WriteLine($"Saved to session storage {json}");
            }
            catch
            {
                Console.WriteLine("failed to save to session storage");
            }

            Console.WriteLine($"{user.UserName} has logged in as {IsAdmin}");
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            OnChange?.Invoke();

            try
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
                Console.WriteLine("Session deleted");
            }
            catch
            {
                
            }
        }
    }
}
