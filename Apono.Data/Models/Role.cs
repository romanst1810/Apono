namespace Apono.Data.Models
{
    public class Role
    {
        public string title { get; set; } = string.Empty;
        public List<string>? allowed_places { get; set; }
        public List<string>? sub_roles { get; set; }
    }
}
