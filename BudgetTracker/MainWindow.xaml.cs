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

namespace BudgetTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<Transaction> _transactions = new List<Transaction>(); 

        public MainWindow()
        {
            InitializeComponent();
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

            // 5. Test-Ausgabe - damit wir sehen, dass alles richtig ausgelesen wurde 
            MessageBox.Show($"Erkannt: {amount} CHF - {category} ({type}) am {date:dd.MM.yyyy}");


        }
    }
}