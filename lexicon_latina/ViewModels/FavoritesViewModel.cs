using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using lexicon_latina.Helpers;
using lexicon_latina.Models;
using lexicon_latina.Services;

namespace lexicon_latina.ViewModels;

public class FavoritesViewModel : INotifyPropertyChanged
{
    private readonly FavoritesService _favoritesService = FavoritesService.Instance;

    public ObservableCollection<LatinEntry> Favorites => _favoritesService.Entries;

    public bool NoFavorites => Favorites.Count == 0;

    public bool HasFavorites => Favorites.Count > 0;

    public ICommand RemoveFavoriteCommand { get; }

    public FavoritesViewModel()
    {
        RemoveFavoriteCommand = new RelayCommand(RemoveFavorite);

        Favorites.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(NoFavorites));
            OnPropertyChanged(nameof(HasFavorites));
        };
    }

    private void RemoveFavorite(object? parameter)
    {
        if (parameter is LatinEntry entry)
        {
            _favoritesService.Remove(entry.Word);
            entry.IsFavorited = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
