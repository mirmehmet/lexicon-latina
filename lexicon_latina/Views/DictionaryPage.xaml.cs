using System.Windows.Controls;
using lexicon_latina.ViewModels;

namespace lexicon_latina.Views;

public partial class DictionaryPage : UserControl
{
    public DictionaryPage()
    {
        InitializeComponent();

        this.Loaded += (s, e) =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SubscribeEvents();
            }
        };

        this.Unloaded += (s, e) =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.UnsubscribeEvents();
            }
        };
    }
}
