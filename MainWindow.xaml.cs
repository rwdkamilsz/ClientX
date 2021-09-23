using System.Windows;

namespace ClientX
{

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Przypisanie i inicjalizacja strony Dashboard
        /// </summary>
        public Dashboard dp = new Dashboard();

        /// <summary>
        /// Zmienna pomocnicza do sprawdzenia czy strona Dashboard zotała wcześniej zainicjalizowana
        /// </summary>
        public Dashboard dpNew;

        /// <summary>
        /// Inicjalizacja głównego okna aplikacji
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Funkcja wywoływana po załadowaniu okna, która przypisuje strone Dashboard do zawartości Frame.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event wskazuje na zdarzenie, które jest kierowane</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = this.dp;
        }

        /// <summary>
        /// Funkcja odpowiadająca za sprawdzenie, czy wcześniej strona Dashboard została zainicjalizowana oraz za załadowanie tej strony do główneg Frame aplikacji.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event wskazuje na zdarzenie, które jest kierowane</param>
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

        /// <summary>
        /// Metoda odpowiadająca za zamknięcie okna aplikacji.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event wskazuje na zdarzenie, które jest kierowane</param>
        private void exitAppHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
