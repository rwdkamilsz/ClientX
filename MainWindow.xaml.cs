using System.Windows;

namespace ClientX
{

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dashboard dp = new Dashboard();
#pragma warning disable CS0246 // Nie można znaleźć nazwy typu lub przestrzeni nazw „ClientList” (brak dyrektywy using lub odwołania do zestawu?)

#pragma warning restore CS0246 // Nie można znaleźć nazwy typu lub przestrzeni nazw „ClientList” (brak dyrektywy using lub odwołania do zestawu?)

#pragma warning disable CS0246 // Nie można znaleźć nazwy typu lub przestrzeni nazw „ClientList” (brak dyrektywy using lub odwołania do zestawu?)

#pragma warning restore CS0246 // Nie można znaleźć nazwy typu lub przestrzeni nazw „ClientList” (brak dyrektywy using lub odwołania do zestawu?)

        public Dashboard dpNew;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dashboard dp = new Dashboard();
            MainFrame.Content = this.dp;
        }
        private void showDashboardHandler(object sender, RoutedEventArgs e)
        {
            if (dp != null)
            {
                MainFrame.Content = dp;
            }
            else
            {
                if (this.dpNew != null)
                {
                    MainFrame.Content = this.dpNew;
                }
                else
                {

                    this.dpNew = new Dashboard();
                }
            }
        }

        private void exitAppHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
