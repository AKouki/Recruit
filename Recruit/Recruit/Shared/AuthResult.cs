namespace Recruit.Shared
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public string? FullName {  get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
