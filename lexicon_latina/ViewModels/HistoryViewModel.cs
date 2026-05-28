
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using lexicon_latina.Helpers;
using lexicon_latina.Models;
using lexicon_latina.Services;

namespace lexicon_latina.ViewModels
{

    public class HistoryViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<HistoryEntry> Entries => HistoryService.Instance.Entries;

        public ICommand SearchEntryCommand { get; }

        public HistoryViewModel()
        {
            SearchEntryCommand = new RelayCommand(param =>
            {

                if (param is HistoryEntry entry)
                    HistoryService.RequestSearch(entry);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
