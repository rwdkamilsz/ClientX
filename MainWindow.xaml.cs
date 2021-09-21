using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace ClientX
{
    

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string V = "ALFKI";
        DatabaseEntities context = new DatabaseEntities();
        CollectionViewSource custViewSource;
        CollectionViewSource ordViewSource; 
        public MainWindow()
        {
            InitializeComponent();
            custViewSource = (CollectionViewSource)FindResource("customersViewSource");
            ordViewSource = (CollectionViewSource)FindResource("customersOrdersViewSource");
            DataContext = this;
          
        }

        // Handle the KeyUp event to print the type of character entered into the control.
        private void TextBox1_KeyUp(object sender)
        {

            TextBox txtbox = (TextBox)sender;
            string txt = txtbox.Text;
          
                custViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(txt)).FirstOrDefault());
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            context.Customers.Load();
            custViewSource.Source = context.Customers.Local;
        }

        public DialogResult ShowAbortMessage => MessageBox.Show("Operation aborted.");

        public DialogResult ShowDeleteDialog => MessageBox.Show("Are you sure to delete row?", "Confirmation", buttons: MessageBoxButtons.YesNo);


        public DialogResult ShowDialogBox(string message)
        {
            return MessageBox.Show(message);
        }

        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToLast();
        }

        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToPrevious();
        }

        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToNext();
           
        }

        public void MoveToSpecific(object sender)
        {
            TextBox txtBox = sender as TextBox;
            string txt = txtBox.Text;
            custViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(txt)).FirstOrDefault());
        }


        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
           
            custViewSource.View.MoveCurrentToFirst();
        }

        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  
            var cur = custViewSource.View.CurrentItem as Customers;

            var cust = (from c in context.Customers
                        where c.CustomerID == cur.CustomerID
                        select c).FirstOrDefault();


            if (ShowDeleteDialog == System.Windows.Forms.DialogResult.Yes)
            {
                if (cust != null)
                {
                    foreach (var ord in cust.Orders.ToList())
                    {
                        Delete_Order(ord);
                    }
                    context.Customers.Remove(cust);
                }
                context.SaveChanges();
                custViewSource.View.Refresh();
            }
            else
            {
                _ = ShowAbortMessage;
            }
        }


        private void search(object sender, ExecutedRoutedEventArgs e)
        {
            string Search = search_id.Text;
            custViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(Search)).FirstOrDefault());
        }

        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (newCustomerGrid.IsVisible)
            {
                // Create a new object because the old one  
                // is being tracked by EF now.  
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

                // Perform very basic validation  
                if (newCustomer.CustomerID.Length != 5)
                {
                    // Insert the new customer at correct position:  
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
                    custViewSource.View.Refresh();
                    custViewSource.View.MoveCurrentTo(newCustomer);
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
     

                Customers currentCustomer = (Customers)custViewSource.View.CurrentItem;

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
                    newOrder.Freight = Convert.ToDecimal(add_freightTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Freight must be convertible to decimal.");
                    return;
                }

                context.Orders.Add(newOrder);
                ordViewSource.View.Refresh();
            }

            context.SaveChanges();
        }


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

        private void NewOrder_click(object sender, RoutedEventArgs e)
        {
            var cust = custViewSource.View.CurrentItem as Customers;
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

        // Cancels any input into the new customer form  
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

            existingCustomerGrid.Visibility = Visibility.Visible;
            newCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.Visibility = Visibility.Collapsed;
        }

        private void Delete_Order(Orders order)
        { 
            var ord = (from o in context.Orders.Local
                       where o.OrderID == order.OrderID
                       select o).FirstOrDefault();

            if (ShowDeleteDialog == System.Windows.Forms.DialogResult.Yes)
            {

                foreach (var detail in ord.Order_Details.ToList())
                {
                    context.Order_Details.Remove(detail);
                }

                context.Orders.Remove(ord);
                context.SaveChanges();

                ordViewSource.View.Refresh();
            }
            else
            {
                _ = ShowAbortMessage;

            }

        }

        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Orders obj = e.Parameter as Orders;
            Delete_Order(obj);
        }

    }
}
