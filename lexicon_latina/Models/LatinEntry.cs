
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lexicon_latina.Models
{

    public class LatinEntry : INotifyPropertyChanged
    {
        private string _word = string.Empty;
        private string _partOfSpeech = string.Empty;
        private string _shortMeaning = string.Empty;
        private string? _source;
        private bool _isFavorited;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}