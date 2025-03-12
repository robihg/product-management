
namespace framework.DTO.GeneralSettingDTO.Responses
{
    public class ResRefUser
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
