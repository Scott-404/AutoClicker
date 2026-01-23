using System.Windows;

namespace SeroAutoClicker
{
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Ensure view model resources are cleaned up on app shutdown
            if (MainWindow?.DataContext is ViewModels.MainViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}
