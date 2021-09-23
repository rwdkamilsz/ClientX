using System.Windows;

namespace ClientX
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    /// 
    public partial class LoginWindow : Window
    {
        /** 
        * a public variable.
        * Details.
        */
        private readonly string user = "user";
        private readonly string userPassword = "user";

        /// <summary>
        /// Inicjalizacja komponentu 
        /// </summary>
        /// <code>InitializeComponent();</code> Inicjalizacja
        public LoginWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Walidacja poprawności danych logowania
        /// </summary>
        /// <param name="username">Login podany przez użytkownika</param>
        /// <param name="userpassword">Hasło podane przez użytkownika</param>
        /// <returns>bool validated -  Zwraca True jeśli dane się zgadzają</returns> 
        public bool checkCredentials(string username, string userpassword)
        {
            bool validated = false;

            if (username == user && userpassword == userPassword)
            {
                validated = true;
            }

            return validated;
        }
        /// <summary>
        /// Funkcja sprawdza czy oba pola logowania są wypełnione i zwraca komunikat do użytkownika, gdy ich nie wypełnił.
        /// Następnie wywołuje funkcje która waliduje poprawność wpisanych danych logowania, i jeśli są poprawne to inicjalizuje główne okno aplikacji lub jeśli nie są, wyświetla komunikat o błędzie.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event Executed wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void tryLogin(object sender, RoutedEventArgs e)
        {
            if (LoginInput.Text.Length == 0)
            {
                MessageBox.Show("Please enter the username.");
                LoginInput.Focus();
            }
            if (PasswordInput.Password.Length == 0)
            {
                MessageBox.Show("Please enter the password.");
                PasswordInput.Focus();
            }
            bool validated = checkCredentials(LoginInput.Text, PasswordInput.Password);

            if (validated)
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Your credentials are incorrect! Try again");
            }
        }

    }
}
