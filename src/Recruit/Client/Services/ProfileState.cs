namespace Recruit.Client.Services
{
    public class ProfileState
    {
        public event Action? OnChange;
        public string? FullName { get; private set; }
        public string? UserAvatar { get; private set; }

        public void SetFullName(string? fullName)
        {
            FullName = fullName;
            OnChange?.Invoke();
        }

        public void SetAvatar(string? userAvatar)
        {
            UserAvatar = userAvatar;
            OnChange?.Invoke();
        }
    }
}
