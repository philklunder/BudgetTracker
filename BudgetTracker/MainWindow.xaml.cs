using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BudgetTracker.Models;
using System.Collections.ObjectModel; 

namespace BudgetTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>(); 

        public MainWindow()
        {
            InitializeComponent();
            TransactionListView.ItemsSource = _transactions;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Betrag auslesen und in decimal umwandeln 
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Bitte einen gültigen Betrag eingeben.");
                return;
            }

            // 2. Kategorie und Beschreibung als String auslesen 
            string category = CategoryTextBox.Text;
            string description = DescriptionTextBox.Text;

            // 3. Datum auslesen (falls nichts gewählt: heute) 
            DateTime date = DatePicker.SelectedDate ?? DateTime.Today;

            // 4. Typ aus der ComboBox auslesen und auf das Enum mappen 
            ComboBoxItem? selectedItem = TypeComboBox.SelectedItem as ComboBoxItem;
            string? typeText = selectedItem?.Content?.ToString();
            TransactionType type = typeText == "Einnahme"
                ? TransactionType.Income
                : TransactionType.Expense;

            // 5.Neues Transaction-Objekt erzeugen 
            Transaction transaction = new Transaction
            {
                Amount = amount,
                Category = category,
                Description = description,
                Date = date,
                Type = type
            };

            // 6. Zur Liste hinzufügen 
            _transactions.Add(transaction);

            // 7. Eingabefelder zurücksetzten
            AmountTextBox.Clear();
            CategoryTextBox.Clear();
            DescriptionTextBox.Clear();
            DatePicker.SelectedDate = null;
            TypeComboBox.SelectedIndex = -1; 
        }
    }
}