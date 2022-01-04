namespace Recruit.Client.Services
{
    public class AppState
    {
        public string? UserAvatar { get; private set; }
        public void SetAvatar(string? userAvatar)
        {
            UserAvatar = userAvatar;
            OnChange?.Invoke();
        }

        public event Action? OnChange;
    }
}
