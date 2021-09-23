using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
namespace ClientX
{
    /// <summary>
    /// Logika interakcji dla klasy Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page

    {
       
        DatabaseEntities context = new DatabaseEntities();
        CollectionViewSource customerViewSource;
        CollectionViewSource ordViewSource;

        /// <summary>
        /// Załadowanie strony panelu głównego oraz przypisanie zasobów do zmiennych.
        /// </summary>
        /// <code>InitializeComponent();</code> Inicjalizacja głównego okna
        public Dashboard()
        {
            InitializeComponent();
            customerViewSource = (CollectionViewSource)FindResource("customersViewSource");
            ordViewSource = (CollectionViewSource)FindResource("customersOrdersViewSource");
            DataContext = this;
        }

        /// <summary>
        /// Odnalezienie źródła danych i wyświetlenie go.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event wskazuje na zdarzenie, które jest kierowane</param>
        /// <code>context.Products.Load()</code> Załadowanie bazy produktów
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            context.Customers.Load();
            customerViewSource.Source = context.Customers.Local;
        }

        /// <summary>
        /// Funkcja pomocnicza, pokazująca monit użytkownikowi o anulowaniu wykonywanej operacji
        /// </summary>
        public static System.Windows.Forms.DialogResult ShowAbortMessage => MessageBox.Show("Operation aborted.");

        /// <summary>
        /// Funkcja pomocnicza, pokazująca monit użytkownikowi z możliwością potwierdzenia lub anulowania operacji usuwania.
        /// </summary>
        public static System.Windows.Forms.DialogResult ShowDeleteDialog(string message = "Are you sure to delete row?") => MessageBox.Show(message, "Confirmation", buttons: System.Windows.Forms.MessageBoxButtons.YesNo);

        /// <summary>
        /// Funkcja pomocnicza, która pozwala na wyświetlenie monitu użytkownikowi.
        /// </summary>
        /// <param name="message">Wiadomośc, która ma się wyświetlić w monicie</param>
        public System.Windows.Forms.DialogResult ShowDialogBox(string message)
        {
            return MessageBox.Show(message);
        }
        /// <summary>
        /// Metoda przenosząca nas do ostatniego elementu bazy danych.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToLast();
        }

        /// <summary>
        /// Metoda przenosząca nas do poprzedniego elementu bazy danych po naciśnięciu przycisku
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        /// <summary>
        /// Metoda przenosząca nas do następnego elementu bazy danych po wciśnięciu przycisku.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();

        }

        /// <summary>
        /// Metoda przenosząca nas do pierwszego elementu bazy danych po naciśnięciu przycisku.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

            customerViewSource.View.MoveCurrentToFirst();
        }

        /// <summary>
        /// Metoda przenosząca nas do elementu wyszukanego po danym przez użytkownika CustomerID. Metoda wyciąga tekst, który użytkownik wpisał w wyszukiwarke
        /// a następnie przenosi nas do rekordu bazy, w którym pole CustomerID jest takie samo
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param> 
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        /// <code>
        /// context.Customers.Where(x => x.CustomerID.Contains(Search)).FirstOrDefault()
        /// </code> Znalazienie rekordu o odpowiadającym CustomerID
        private void Search(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox txtb = (TextBox)search_id;
            string Search = search_id.Text;
            customerViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(Search)).FirstOrDefault());
        }

        /// <summary>
        /// Funkcja odpowiadająca za walidację pola Freight oraz konwersję na liczbę dziesiętną
        /// </summary>
        /// <param name="freight">Pole odpowiadające za wartość opłaty za zamowienie</param>
        /// <returns>
        /// Funkcja zwraca liczbę dziesiętną, zwalidowaną i przekonwertowaną z typu string
        /// </returns> 
        public decimal? ValidateFreight(string freight)
        {
            decimal validatedFreight;
            decimal freightNew = Convert.ToDecimal(freight);
            if (freightNew > 1)
            {
                validatedFreight = freightNew;
            }
            else
            {
                validatedFreight = 1001;
            }

            return validatedFreight;
        }
        /// <summary>
        /// Metoda ta odpowiada za dwa zdarzenia. Dodanie nowego klienta oraz dodanie nowego zamówienia w zależności od widoczności danego formularza.
        /// Przy dodawaniu nowego klienta, metoda sprawdza czy pole CustomerID jest wypełnione, jeśli tak to dodaje rekord na ostatnie miejsce w bazie.
        /// W przypadku dodawania nowego zamówienia, metoda waliduje pole Freight i EmployeeID a następnie dodaje rekord do bazy.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param> 
        /// <param name="e">ExecutedRouted Event wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (newCustomerGrid.IsVisible)
            {
                Customers newCustomer = new Customers
                {
                    Address = add_addressTextBox.Text,
                    City = add_cityTextBox.Text,
                    CompanyName = add_companyNameTextBox.Text,
                    ContactName = add_contactNameTextBox.Text,
                    ContactTitle = add_contactTitleTextBox.Text,
                    Country = add_countryTextBox.Text,
                    CustomerID = add_customerIDTextBox.Text,
                    Phone = add_phoneTextBox.Text,
                    PostalCode = add_postalCodeTextBox.Text,
                    Region = add_regionTextBox.Text
                };
                if (newCustomer.CustomerID.Length != 0)
                {
                    int len = context.Customers.Local.Count();
                    int pos = len;
                    for (int i = 0; i < len; ++i)
                    {
                        if (String.CompareOrdinal(newCustomer.CustomerID, context.Customers.Local[i].CustomerID) < 0)
                        {
                            pos = i;
                            break;
                        }
                    }
                    context.Customers.Local.Insert(pos, newCustomer);
                    customerViewSource.View.Refresh();
                    customerViewSource.View.MoveCurrentTo(newCustomer);
                }
                else
                {
                    MessageBox.Show("CustomerID cannot be null");
                }

                newCustomerGrid.Visibility = Visibility.Collapsed;
                existingCustomerGrid.Visibility = Visibility.Visible;
            }
            else if (newOrderGrid.IsVisible)
            {
                Customers currentCustomer = (Customers)customerViewSource.View.CurrentItem;

                Orders newOrder = new Orders()
                {
                    OrderDate = add_orderDatePicker.SelectedDate,
                    CustomerID = currentCustomer.CustomerID,
                    ShipAddress = currentCustomer.Address,
                    ShipCity = currentCustomer.City,
                    ShipCountry = currentCustomer.Country,
                    ShipName = currentCustomer.CompanyName,
                    ShipPostalCode = currentCustomer.PostalCode,
                    ShipRegion = currentCustomer.Region
                };

                try
                {
                    newOrder.EmployeeID = Int32.Parse(add_employeeIDTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("EmployeeID must be a valid integer value.");
                    return;
                }

                try
                {
                    newOrder.Freight = ValidateFreight(add_freightTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Freight must be convertible to decimal and greater than 1.");
                    return;
                }

                context.Orders.Add(newOrder);
                ordViewSource.View.Refresh();
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Metoda odwołująca się do przycisku Add, która wywołuje Grid dla dodania nowego klienta, czyści jego pola oraz ukrywa inny aktualnie otworzony formularz.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event  wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            existingCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.Visibility = Visibility.Collapsed;
            newCustomerGrid.Visibility = Visibility.Visible;

            foreach (var child in newCustomerGrid.Children)
            {
                var tb = child as TextBox;
                if (tb != null)
                {
                    tb.Text = "";
                }
            }
        }
        /// <summary>
        /// Metoda odwołująca się do przycisku Add, która sprawdza czy wybrany jest klient do którego ma być dopisane zamówienie a następnie ukrywa inne formularze
        /// i wyświetla formularz dodania nowego zamówienia.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">Routed Event wskazuje na zdarzenie, które jest kierowane</param>
        private void NewOrder_click(object sender, RoutedEventArgs e)
        {
            var cust = customerViewSource.View.CurrentItem as Customers;
            if (cust == null)
            {
                MessageBox.Show("No customer selected.");
                return;
            }

            existingCustomerGrid.Visibility = Visibility.Collapsed;
            newCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.UpdateLayout();
            newOrderGrid.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Metoda, która odpowiada za usuwanie zamówień z bazy. Wywoływana jest przy usuwaniu pojedynczego zamówienia
        /// oraz podczas usuwania klienta z bazy - wtedy usuwa wszystkie zamówienia przypisane do danego klienta.
        /// </summary>
        /// <param name="order">Obiekt "Zamówienia" - może być to pojedyncze zamówienie lub zbiór zamówień</param>
        /// <param name="type">Typ operacji jaka ma być wykonana - pojedyncze usunięcie lub usunięcie wszystkich zamówień klienta. Domyślnie usuwa pojedyncze zamówienie</param>
        public void Delete_Order(Orders order, string type = "single")
        {
            var ord = (from o in context.Orders.Local
                       where o.OrderID == order.OrderID
                       select o).FirstOrDefault();

            if (type == "single")
            {
                System.Windows.Forms.DialogResult confirmation = ShowDeleteDialog();

                if (confirmation == System.Windows.Forms.DialogResult.Yes)
                {

                    try
                    {
                        foreach (var detail in ord.Order_Details.ToList())
                        {
                            context.Order_Details.Remove(detail);
                        }

                        context.Orders.Remove(ord);
                        context.SaveChanges();

                        ordViewSource.View.Refresh();

                        ShowDialogBox("Order deleted succesfully.");
                    }
                    catch (Exception error)
                    {
                    
                        ShowDialogBox("Something went wrong. Error: " + error);
                    
                    }
                }
                else
                {
                    _ = ShowAbortMessage;
                }
            }

            if (type == "all")
            {
                try
                {

                    foreach (var detail in ord.Order_Details.ToList())
                    {
                        context.Order_Details.Remove(detail);
                    }

                    context.Orders.Remove(ord);
                    context.SaveChanges();

                    ordViewSource.View.Refresh();

                }
                catch (Exception error)
                {
                
                    ShowDialogBox("Something went wrong. Error: " + error);
                }

            }

        }

        /// <summary>
        /// Metoda odpowiadająca za usunięcie użytkownik z bazy oraz usunięcie wszystkich jego zamówień.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event  wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var cur = customerViewSource.View.CurrentItem as Customers;

            var cust = (from c in context.Customers
                        where c.CustomerID == cur.CustomerID
                        select c).FirstOrDefault();
            if(cust != null)
            {

            }

            if (ShowDeleteDialog() == System.Windows.Forms.DialogResult.Yes 
                && ShowDeleteDialog("Are you willing to delete all orders associated with this customer?") == System.Windows.Forms.DialogResult.Yes)
            {
              if (cust != null)

                {
                  try
                    {
                        foreach (var ord in cust.Orders.ToList())
                        {
                            Delete_Order(ord, "all");
                        }

                    }
                    catch (Exception error)
                    {
                         throw error;
                    }
                    ShowDialogBox("Customer and all his orders deleted succesfully.");
                    context.Customers.Remove(cust);

                }
                context.SaveChanges();
                customerViewSource.View.Refresh();
            }

            else
            {
                _ = ShowAbortMessage;
            }
        }

        /// <summary>
        /// Metoda odpowiadająca za anulowanie wprowadzania nowego klienta.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event  wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void CancelCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            add_addressTextBox.Text = "";
            add_cityTextBox.Text = "";
            add_companyNameTextBox.Text = "";
            add_contactNameTextBox.Text = "";
            add_contactTitleTextBox.Text = "";
            add_countryTextBox.Text = "";
            add_customerIDTextBox.Text = "";
            add_phoneTextBox.Text = "";
            add_postalCodeTextBox.Text = "";
            add_regionTextBox.Text = "";

            newOrderGrid.Visibility = Visibility.Collapsed;
            newCustomerGrid.Visibility = Visibility.Collapsed;

            existingCustomerGrid.Visibility = Visibility.Visible;

       
        }

        /// <summary>
        /// Metoda, przypisana do kontrolki, która wywołuję funkcję usuwająca zamówienia z bazy.
        /// </summary>
        /// <param name="sender">Parametr sender zawiera odniesienie do kontrolki/obiektu, który wywołał zdarzenie</param>
        /// <param name="e">ExecutedRouted Event  wskazuje na zdarzenie, które jest kierowane i wykonane</param>
        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Orders obj = e.Parameter as Orders;

            Delete_Order(obj);

        }

 
    }
}
