using System.ComponentModel.DataAnnotations;

namespace framework.DTO.GeneralSettingDTO.Requests
{
    public class ReqRefUser
    {
        public Guid? Guid { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public required string Name { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Username cannot exceed 10 characters.")]
        public required string Username { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public required string Password { get; set; }
    }
}
