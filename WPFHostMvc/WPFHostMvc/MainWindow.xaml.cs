using System.Reflection;
using System.Windows;

namespace WPFHostMvc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic activeX = Browser.GetType().InvokeMember("ActiveXInstance", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, Browser, new object[] { });
            activeX.Silent = true;
            Browser.Navigate("http://127.0.0.1:9388/myview");
        }
    }
}
