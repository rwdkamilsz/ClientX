using System.Windows;

namespace ClientX
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        private readonly string user = "user";

        private readonly string userPassword = "user";
        public LoginWindow()
        {
            InitializeComponent();
        }

        public bool checkCredentials(string username, string userpassword)
        {
            bool validated = false;

            if (username == user && userpassword == userPassword)
            {
                validated = true;
            }

            return validated;
        }

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
