using System.ComponentModel.DataAnnotations;

[Serializable]
public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; }

    [Required(ErrorMessage="Username is required")]
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }

    public bool IsAdmin() => Role == UserRole.Admin;
    public bool IsFaculty() => Role == UserRole.Faculty;
}