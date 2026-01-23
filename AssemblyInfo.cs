using System.Windows;

namespace SeroAutoClicker
{
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Clean shutdown
            if (MainWindow?.DataContext is ViewModels.MainViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}
