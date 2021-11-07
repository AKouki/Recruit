namespace Recruit.Client.Services
{
    public class AppState
    {
        private string? _userAvatar;

        public string? UserAvatar
        {
            get { return _userAvatar; }
            set
            {
                _userAvatar = value;
                OnChange?.Invoke();
            }
        }

        public event Action? OnChange;
    }
}
