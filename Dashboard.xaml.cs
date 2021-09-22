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
        public Dashboard()
        {
            InitializeComponent();
            customerViewSource = (CollectionViewSource)FindResource("customersViewSource");
            ordViewSource = (CollectionViewSource)FindResource("customersOrdersViewSource");
            DataContext = this;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            context.Customers.Load();
            customerViewSource.Source = context.Customers.Local;
        }

        public static System.Windows.Forms.DialogResult ShowAbortMessage => MessageBox.Show("Operation aborted.");

        public static System.Windows.Forms.DialogResult ShowDeleteDialog(string message = "Are you sure to delete row?") => MessageBox.Show(message, "Confirmation", buttons: System.Windows.Forms.MessageBoxButtons.YesNo);


        public System.Windows.Forms.DialogResult ShowDialogBox(string message)
        {
            return MessageBox.Show(message);
        }

        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToLast();
        }

        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();

        }

        public void MoveToSpecific(object sender)
        {
            TextBox txtBox = sender as TextBox;
            string txt = txtBox.Text;
            customerViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(txt)).FirstOrDefault());
        }


        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

            customerViewSource.View.MoveCurrentToFirst();
        }

        private void Search(object sender, ExecutedRoutedEventArgs e)
        {
            string Search = search_id.Text;
            customerViewSource.View.MoveCurrentTo(context.Customers.Where(x => x.CustomerID.Contains(Search)).FirstOrDefault());
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

        public bool Delete_Order(Orders order, string type = "single")
        {
            bool errorWhenDeleting = false;
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
                        errorWhenDeleting = true;
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
                    errorWhenDeleting = true;
                    ShowDialogBox("Something went wrong. Error: " + error);
                }

            }

            return errorWhenDeleting;

        }

        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  
            var cur = customerViewSource.View.CurrentItem as Customers;

            var cust = (from c in context.Customers
                        where c.CustomerID == cur.CustomerID
                        select c).FirstOrDefault();


            if (ShowDeleteDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                if (ShowDeleteDialog("Are you willing to delete all orders associated with this customer?") == System.Windows.Forms.DialogResult.Yes)
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
            else
            {
                _ = ShowAbortMessage;
            }
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
        }

        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Orders obj = e.Parameter as Orders;

            Delete_Order(obj);

        }

    }
}
