using System.Collections.ObjectModel;
using System.Windows.Input;
using lexicon_latina.Helpers;
using lexicon_latina.Models;
using lexicon_latina.Services;

namespace lexicon_latina.ViewModels;

public class HistoryViewModel
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
}
