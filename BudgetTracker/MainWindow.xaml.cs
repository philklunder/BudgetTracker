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

        private Transaction? _editingTransaction = null;

        public MainWindow()
        {
            InitializeComponent();
            TransactionListView.ItemsSource = _transactions;
            UpdateBalance();
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
            UpdateBalance();
            // Falls vorher der Bearbeitungs-Modus aktiv war: beenden
            _editingTransaction = null;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            //1. Ausgewählte Transaktion aus der ListView holen
            Transaction? selected = TransactionListView.SelectedItem as Transaction;

            // 2. Wenn nichts ausgewählt ist: Hinweis anzeigen und abbrechen 
            if (selected is null)
            {
                MessageBox.Show("Bitte zuerst einen Eintrag in der Liste auswählen.");
                return;
            }

            // 3. Werte des Eintrags in die Formularfelder laden 
            AmountTextBox.Text = selected.Amount.ToString();
            CategoryTextBox.Text = selected.Category;
            DescriptionTextBox.Text = selected.Description;
            DatePicker.SelectedDate = selected.Date;
            TypeComboBox.SelectedIndex = selected.Type == TransactionType.Income ? 0 : 1;

            // 4. In den Bearbeitungs-Modus wechseln
            _editingTransaction = selected;
            SaveButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //1. Ausgewählte Transaktion aus der ListView holen
            Transaction? selected = TransactionListView.SelectedItem as Transaction; 

            // 2. Wenn nichts ausgewählt ist: Hinweis anzeigen und abbrechen 
            if (selected is null)
            {
                MessageBox.Show("Bitte zuerst einen Eintrag in der Liste auswählen.");
                return; 
            }

            // 3. Bestätigungsdialog anzeigen (JA/NEIN)
            MessageBoxResult result = MessageBox.Show(
                $"Eintrag wirklich löschen?\n\n{selected.Date:dd.MM.yyyy} - {selected.Category}: {selected.Amount:C}",
                "Löschen bestätigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // 4. Wenn der Nutzer NICHT auf "JA" geklickt hat: abbrechen 
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            // 5. Eintrag aus der Liste entfernen
            _transactions.Remove(selected);

            // 6. Kontostand aktualisieren
            UpdateBalance();
        }


        private void UpdateBalance()
        {
            decimal balance = 0;

            foreach (Transaction transaction in _transactions)
            {
                if (transaction.Type == TransactionType.Income)
                {
                    balance += transaction.Amount;
                }
                else
                {
                    balance -= transaction.Amount;
                }
            }

            BalanceTextBlock.Text = $"Kontostand: {balance:C}" ;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Sicherheits-Check: ohne aktiven Bearbitungs-Modus nichts tun 
            if (_editingTransaction is null)
            {
                return; 
            }

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

            // 4. Typ aus der ComboBox auslesen uns auf das Enum mappen
            ComboBoxItem? selectedItem = TypeComboBox.SelectedItem as ComboBoxItem;
            string? typeText = selectedItem?.Content?.ToString();
            TransactionType type = typeText == "Einnahme"
                ? TransactionType.Income
                : TransactionType.Expense;

            // 5. Werte ins bestehende Object schreiben (statt ein neues anzulegen)
            _editingTransaction.Amount = amount;
            _editingTransaction.Category = category;
            _editingTransaction.Description = description;
            _editingTransaction.Date = date;
            _editingTransaction.Type = type;

            // 6. ListView zwingen, Ihre Anzeige neu zu zeichnen 
            TransactionListView.Items.Refresh();

            // 7. Eingabefelder zurücksetzen 
            AmountTextBox.Clear();
            CategoryTextBox.Clear();
            DescriptionTextBox.Clear();
            DatePicker.SelectedDate = null;
            TypeComboBox.SelectedIndex = -1;

            // 8. Bearbeitungs-Modus beenden
            _editingTransaction = null;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;

            // 9. Kontostand aktualisieren
            UpdateBalance();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Eingabefelder zurücksetzen (verworfene Änderungen verwerfen)
            AmountTextBox.Clear();
            CategoryTextBox.Clear();
            DescriptionTextBox.Clear();
            DatePicker.SelectedDate = null;
            TypeComboBox.SelectedIndex = -1;

            // 2. Bearbeitungs-Modus beenden
            _editingTransaction = null;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }
    }
}