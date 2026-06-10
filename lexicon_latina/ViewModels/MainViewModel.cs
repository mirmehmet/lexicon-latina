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

    public MainViewModel()
    {
        _translationService = new TranslationService();
        _dictionaryService     = new GoogleTranslateDictionaryService();

        SearchCommand = new RelayCommand(async _ => await SearchAsync());

        ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);

        CopyCommand = new RelayCommand(CopyWord);
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