namespace ClassroomReservationSystem.Components.Services
{
    public class UserState
    {
        public User? CurrentUser { get; private set; }

        public event Action? OnChange;
        public bool IsLoggedIn => CurrentUser != null;
        //public bool IsAdmin = true;
        public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
        public bool IsFaculty => CurrentUser?.Role == UserRole.Faculty;


        public void SetUser(User user)
        {
            CurrentUser = user;
            OnChange?.Invoke();
            Console.WriteLine($"{user.UserName} has logged in as {IsAdmin}");
        }

        public void Logout()
        {
            CurrentUser = null;
            OnChange?.Invoke();
        }
    }

}
