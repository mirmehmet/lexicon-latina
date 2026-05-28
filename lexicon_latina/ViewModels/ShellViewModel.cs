
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using lexicon_latina.Helpers;

namespace lexicon_latina.ViewModels
{

    public enum AppScreen
    {
        Home,
        Dictionary,
        Favorites,
        History
    }

    public class ShellViewModel : INotifyPropertyChanged
    {
        private AppScreen _currentScreen = AppScreen.Home;

        public AppScreen CurrentScreen
        {
            get => _currentScreen;
            set
            {
                _currentScreen = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(IsHome));
                OnPropertyChanged(nameof(IsDictionary));
                OnPropertyChanged(nameof(IsFavorites));
                OnPropertyChanged(nameof(IsHistory));
            }
        }

        public bool IsHome       => CurrentScreen == AppScreen.Home;

        public bool IsDictionary => CurrentScreen == AppScreen.Dictionary;

        public bool IsFavorites  => CurrentScreen == AppScreen.Favorites;

        public bool IsHistory    => CurrentScreen == AppScreen.History;

        public ICommand GoToDictionaryCommand { get; }

        public ICommand GoToFavoritesCommand  { get; }

        public ICommand GoToHistoryCommand    { get; }

        public ICommand GoHomeCommand         { get; }

        public ShellViewModel()
        {

            GoToDictionaryCommand = new RelayCommand(_ => CurrentScreen = AppScreen.Dictionary);
            GoToFavoritesCommand  = new RelayCommand(_ => CurrentScreen = AppScreen.Favorites);
            GoToHistoryCommand    = new RelayCommand(_ => CurrentScreen = AppScreen.History);
            GoHomeCommand         = new RelayCommand(_ => CurrentScreen = AppScreen.Home);

            Services.HistoryService.SearchRequested += _ => CurrentScreen = AppScreen.Dictionary;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
