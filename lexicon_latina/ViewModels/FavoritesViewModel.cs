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
    private readonly TatoebaSentenceService _tatoebaService = new();

    public ObservableCollection<LatinEntry> Favorites => _favoritesService.Entries;

    public bool NoFavorites => Favorites.Count == 0;

    public bool HasFavorites => Favorites.Count > 0;

    public ICommand RemoveFavoriteCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PlayTtsCommand { get; }
    public ICommand ShowSentencesCommand { get; }

    private readonly System.Windows.Media.MediaPlayer _mediaPlayer = new();

    public FavoritesViewModel()
    {
        RemoveFavoriteCommand = new RelayCommand(RemoveFavorite);
        CopyCommand = new RelayCommand(CopyWord);
        PlayTtsCommand = new RelayCommand(PlayTts);
        ShowSentencesCommand = new RelayCommand(async parameter => await ShowSentencesAsync(parameter));

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

    private void CopyWord(object? parameter)
    {
        if (parameter is LatinEntry entry && !string.IsNullOrEmpty(entry.Word))
        {
            try
            {
                System.Windows.Clipboard.SetText(entry.Word);
            }
            catch { }
        }
    }

    private void PlayTts(object? parameter)
    {
        string? textToSpeak = null;
        if (parameter is LatinEntry entry)
        {
            textToSpeak = entry.Word;
        }
        else if (parameter is string text)
        {
            textToSpeak = text;
        }

        if (!string.IsNullOrEmpty(textToSpeak))
        {
            try
            {
                string url = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=la&q={Uri.EscapeDataString(textToSpeak)}";
                _mediaPlayer.Open(new Uri(url));
                _mediaPlayer.Play();
            }
            catch { }
        }
    }

    private async Task ShowSentencesAsync(object? parameter)
    {
        if (parameter is LatinEntry entry)
        {
            if (entry.IsExpanded)
            {
                entry.IsExpanded = false;
                return;
            }

            entry.IsExpanded = true;

            if (entry.Sentences.Count == 0 && !entry.IsLoadingSentences)
            {
                entry.IsLoadingSentences = true;
                entry.SentencesStatusMessage = "Cümleler yükleniyor...";
                entry.Sentences.Clear();

                try
                {
                    var sentences = await _tatoebaService.GetSentencesAsync(entry.Word);
                    if (sentences != null && sentences.Count > 0)
                    {
                        entry.SentencesStatusMessage = string.Empty;
                        foreach (var sentence in sentences)
                        {
                            entry.Sentences.Add(sentence);
                        }
                    }
                    else
                    {
                        entry.SentencesStatusMessage = "Bu kelimeyi içeren örnek cümle bulunamadı.";
                    }
                }
                catch (Exception)
                {
                    entry.SentencesStatusMessage = "Cümleler yüklenirken bir hata oluştu.";
                }
                finally
                {
                    entry.IsLoadingSentences = false;
                }
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
