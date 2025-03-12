namespace framework.DTO.GeneralSettingDTO.Requests
{
    public class ReqRefUser
    {
        public Guid? Guid { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
        public required string Password { get; set; }
    }
}
