using System;
using System.Windows;

namespace lexicon_latina.Views
{

    public partial class ShellWindow : Window
    {

        public ShellWindow()
        {

            InitializeComponent();
            this.StateChanged += ShellWindow_StateChanged;
        }

        private void ShellWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeIcon.Text = "❐";
            }
            else
            {
                MaximizeIcon.Text = "▢";
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
