using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lexicon_latina.Models;

public class LatinEntry : INotifyPropertyChanged
{
    private string _word = string.Empty;
    private string _partOfSpeech = string.Empty;
    private string _shortMeaning = string.Empty;
    private string? _source;
    private bool _isFavorited;
    private bool _isExpanded;
    private bool _isLoadingSentences;
    private string _sentencesStatusMessage = string.Empty;
    private ObservableCollection<TatoebaSentence> _sentences = new();

    public string Word
    {
        get => _word;
        set { _word = value; OnPropertyChanged(); }
    }

    public string PartOfSpeech
    {
        get => _partOfSpeech;
        set { _partOfSpeech = value; OnPropertyChanged(); }
    }

    public string ShortMeaning
    {
        get => _shortMeaning;
        set { _shortMeaning = value; OnPropertyChanged(); }
    }

    public string? Source
    {
        get => _source;
        set { _source = value; OnPropertyChanged(); }
    }

    public bool IsFavorited
    {
        get => _isFavorited;
        set { _isFavorited = value; OnPropertyChanged(); }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set { _isExpanded = value; OnPropertyChanged(); }
    }

    public bool IsLoadingSentences
    {
        get => _isLoadingSentences;
        set { _isLoadingSentences = value; OnPropertyChanged(); }
    }

    public string SentencesStatusMessage
    {
        get => _sentencesStatusMessage;
        set { _sentencesStatusMessage = value; OnPropertyChanged(); }
    }

    public ObservableCollection<TatoebaSentence> Sentences
    {
        get => _sentences;
        set { _sentences = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}