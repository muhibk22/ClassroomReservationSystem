namespace ClassroomReservationSystem.Components.Services
{
    public class UserState
    {
        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;
        //public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
        public bool IsAdmin = true;
        //public bool IsFaculty => CurrentUser?.Role == UserRole.Faculty;
        public bool IsFaculty = false;

        public void SetUser(User user)
        {
            CurrentUser = user;
            Console.WriteLine($"{user.UserName} has logged in as {IsAdmin}");
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }

}
