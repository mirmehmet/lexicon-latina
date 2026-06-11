using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lexicon_latina.Models;

public class TatoebaSentence : INotifyPropertyChanged
{
    private string _latinText = string.Empty;
    private string _translationText = string.Empty;
    private bool _isFavorited;

    public string LatinText
    {
        get => _latinText;
        set { _latinText = value; OnPropertyChanged(); }
    }

    public string TranslationText
    {
        get => _translationText;
        set { _translationText = value; OnPropertyChanged(); }
    }

    public bool IsFavorited
    {
        get => _isFavorited;
        set { _isFavorited = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
