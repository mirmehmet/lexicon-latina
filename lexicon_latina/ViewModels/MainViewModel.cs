using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using lexicon_latina.Helpers;
using lexicon_latina.Models;
using lexicon_latina.Services;

namespace lexicon_latina.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly TranslationService    _translationService;
    private readonly GoogleTranslateDictionaryService  _dictionaryService;
    private readonly TatoebaSentenceService            _tatoebaService;

    private string _inputText = string.Empty;

    public string InputText
    {
        get => _inputText;
        set { _inputText = value; OnPropertyChanged(); }
    }

    private string _translatedText = string.Empty;

    public string TranslatedText
    {
        get => _translatedText;
        set { _translatedText = value; OnPropertyChanged(); }
    }

    private string _statusMessage = string.Empty;

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ObservableCollection<LatinEntry> Results { get; } = new();

    public ICommand SearchCommand { get; }

    public ICommand ToggleFavoriteCommand { get; }

    public ICommand CopyCommand { get; }

    public ICommand PlayTtsCommand { get; }

    public ICommand ShowSentencesCommand { get; }

    private readonly System.Windows.Media.MediaPlayer _mediaPlayer = new();

    public MainViewModel()
    {
        _translationService = new TranslationService();
        _dictionaryService     = new GoogleTranslateDictionaryService();
        _tatoebaService        = new TatoebaSentenceService();

        SearchCommand = new RelayCommand(async _ => await SearchAsync());

        ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);

        CopyCommand = new RelayCommand(CopyWord);

        PlayTtsCommand = new RelayCommand(PlayTts);

        ShowSentencesCommand = new RelayCommand(async parameter => await ShowSentencesAsync(parameter));
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

    public void SubscribeEvents()
    {
        UnsubscribeEvents();
        FavoritesService.Instance.Entries.CollectionChanged += OnFavoritesChanged;
        HistoryService.SearchRequested += OnSearchRequested;
    }

    public void UnsubscribeEvents()
    {
        FavoritesService.Instance.Entries.CollectionChanged -= OnFavoritesChanged;
        HistoryService.SearchRequested -= OnSearchRequested;
    }

    private void OnFavoritesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var entry in Results)
        {
            entry.IsFavorited = FavoritesService.Instance.IsFavorited(entry.Word);
        }
    }

    private async void OnSearchRequested(HistoryEntry entry)
    {
        await SearchFromHistoryAsync(entry);
    }

    private async Task SearchAsync()
    {
        string text = (InputText ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(text)) return;

        IsLoading      = true;
        StatusMessage  = string.Empty;
        Results.Clear();
        TranslatedText = string.Empty;

        try
        {
            StatusMessage = "Çevriliyor...";
            var english   = await _translationService.TranslateAsync(text);
            TranslatedText = english;

            StatusMessage = "Latince aranıyor...";
            var results   = await _dictionaryService.SearchAsync(english);

            if (results.Count > 0)
            {
                foreach (var entry in results)
                {
                    entry.IsFavorited = FavoritesService.Instance.IsFavorited(entry.Word);
                    Results.Add(entry);
                }

                HistoryService.Instance.Add(text, english);

                StatusMessage = $"{results.Count} sonuç bulundu";
            }
            else
            {
                StatusMessage = "Sonuç bulunamadı.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchFromHistoryAsync(HistoryEntry entry)
    {
        IsLoading     = true;
        StatusMessage = string.Empty;
        Results.Clear();

        InputText      = entry.InputTurkish;
        TranslatedText = entry.TranslatedWord;

        try
        {
            StatusMessage = "Latince aranıyor...";
            var results = await _dictionaryService.SearchAsync(entry.TranslatedWord);

            if (results.Count > 0)
            {
                foreach (var r in results)
                {
                    r.IsFavorited = FavoritesService.Instance.IsFavorited(r.Word);
                    Results.Add(r);
                }
                StatusMessage = $"{results.Count} sonuç bulundu";
            }
            else
            {
                StatusMessage = "Sonuç bulunamadı.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ToggleFavorite(object? parameter)
    {
        if (parameter is LatinEntry entry)
        {
            if (FavoritesService.Instance.IsFavorited(entry.Word))
            {
                FavoritesService.Instance.Remove(entry.Word);
                entry.IsFavorited = false;
            }
            else
            {
                FavoritesService.Instance.Add(entry);
                entry.IsFavorited = true;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}