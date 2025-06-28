using Microsoft.UI.Xaml;

namespace WinUIpad4
{
    public partial class App : Application
    {
        private Window? _window;
        
        // Added per https://learn.microsoft.com/en-us/windows/apps/design/controls/tab-view
        public static Window Window { get { return m_window; } }
        // Update this to make it static.
        private static Window m_window;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
