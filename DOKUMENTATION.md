# Dokumentation – Budget Tracker

Diese Datei begleitet die `ANLEITUNG.md`. Während die Anleitung nur beschreibt, **was** in jedem Schritt getan werden muss, zeigt die Dokumentation:

- den konkreten **Code**
- **warum** er so geschrieben ist
- welche **C#- und WPF-Konzepte** dahinterstecken

Aufgebaut ist sie in derselben Nummerierung wie die Anleitung – wer dort z. B. „4.3" liest, findet hier unter „4.3" die zugehörige Erklärung mit Code.

---

## Schritt 1 – Projektstruktur

### 1.1 WPF-Projekt anlegen
Die Visual-Studio-Vorlage erzeugt automatisch:

- `App.xaml` / `App.xaml.cs` – Einstiegspunkt der Anwendung; hier wird beim Start das erste Fenster geöffnet.
- `MainWindow.xaml` / `MainWindow.xaml.cs` – das Hauptfenster.
- `AssemblyInfo.cs` – Metadaten (Versionsnummer, Sprache usw.).

**Warum die Vorlage nehmen?** Sie bringt den ganzen Boilerplate für den App-Lifecycle und das XAML-Laden mit. Den müssten wir sonst selbst schreiben – das hat mit „Budget Tracker" nichts zu tun.

### 1.2 Models-Ordner
Wir legen einen Ordner `Models` an, in dem alle reinen Datenklassen leben.

**Warum diese Trennung?** Auch wenn wir noch kein MVVM machen, ist es eine bewährte Konvention, „Daten" (Models) von „UI" (Views) zu trennen. So bleibt das Projekt übersichtlich und ist später leicht zu erweitern.

---

## Schritt 2 – Klassen

### 2.1 TransactionType (Enum)

```csharp
namespace BudgetTracker.Models
{
    public enum TransactionType
    {
        Income,
        Expense
    }
}
```

**Warum ein Enum?** Es gibt genau zwei mögliche Werte: Einnahme oder Ausgabe. Mit einem Enum ist das im Code typsicher (`TransactionType.Income`) – Tippfehler wie `"income"` vs. `"Income"` können nicht passieren, der Compiler prüft das.

### 2.2 Transaction (Datenklasse)

```csharp
using System;

namespace BudgetTracker.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
    }
}
```

**Warum `decimal` für `Amount`?** Geldbeträge nie mit `double` oder `float` speichern – diese Fließkommatypen können Beträge wie `0.1` nicht exakt darstellen, was zu falschen Summen führt. `decimal` ist genau für diesen Anwendungsfall gemacht.

**Warum `= ""` bei den Strings?** Damit die Properties nie `null` sind. Wir ersparen uns überall im Code lästige Null-Checks und mögliche `NullReferenceException`s.

**Warum nur `{ get; set; }`?** Das nennt man **Auto-Properties**. C# erzeugt das interne Feld automatisch. Reicht hier vollkommen aus, weil wir keine spezielle Logik beim Setzen brauchen.

---

## Schritt 3 – UI in WPF

### 3.1 Hauptlayout

```xml
<Window x:Class="BudgetTracker.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Budget Tracker" Height="500" Width="800">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <!-- Inhalt der drei Zeilen -->
    </Grid>
</Window>
```

**Warum drei Zeilen mit `Auto / * / Auto`?**
- `Auto` heißt: nimm exakt so viel Platz, wie der Inhalt braucht.
- `*` heißt: nimm den gesamten restlichen Platz.

So bleibt das Eingabeformular oben und der Kontostand unten kompakt, während die ListView in der Mitte den restlichen Raum füllt – auch wenn man das Fenster vergrößert.

### 3.2 Eingabeformular

```xml
<GroupBox Grid.Row="0" Header="Neuer Eintrag" Padding="10" Margin="0,0,0,10">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="120"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <Label   Grid.Row="0" Grid.Column="0" Content="Betrag:"/>
        <TextBox Grid.Row="0" Grid.Column="1" x:Name="AmountTextBox" Margin="0,3"/>

        <Label   Grid.Row="1" Grid.Column="0" Content="Kategorie:"/>
        <TextBox Grid.Row="1" Grid.Column="1" x:Name="CategoryTextBox" Margin="0,3"/>

        <Label   Grid.Row="2" Grid.Column="0" Content="Beschreibung:"/>
        <TextBox Grid.Row="2" Grid.Column="1" x:Name="DescriptionTextBox" Margin="0,3"/>

        <Label      Grid.Row="3" Grid.Column="0" Content="Datum:"/>
        <DatePicker Grid.Row="3" Grid.Column="1" x:Name="DatePicker" Margin="0,3"/>

        <Label    Grid.Row="4" Grid.Column="0" Content="Typ:"/>
        <ComboBox Grid.Row="4" Grid.Column="1" x:Name="TypeComboBox" Margin="0,3">
            <ComboBoxItem Content="Einnahme"/>
            <ComboBoxItem Content="Ausgabe"/>
        </ComboBox>

        <Button Grid.Row="5" Grid.Column="1"
                x:Name="AddButton"
                Content="Hinzufügen"
                Click="AddButton_Click"
                Margin="0,10,0,0"
                HorizontalAlignment="Left"
                Padding="20,5"/>
    </Grid>
</GroupBox>
```

**Warum eine `GroupBox`?** Sie zeichnet einen Rahmen mit Beschriftung – optisch sieht man sofort: „Diese Felder gehören zusammen."

**Warum zwei Spalten 120 / `*`?** Linke Spalte fix für die Labels, rechte Spalte flexibel für die Eingabefelder. So sind die Labels untereinander bündig.

**Was ist `x:Name`?** Ein Name, über den wir das Element später im C#-Code ansprechen können (z. B. `AmountTextBox.Text`). Ohne `x:Name` ist das Element für den Code unsichtbar.

**Was ist `Click="AddButton_Click"`?** Damit verbinden wir den Button mit einer Methode im Code-Behind. WPF sucht beim Klick automatisch nach einer Methode dieses Namens.

### 3.3 ListView mit Spalten

```xml
<ListView Grid.Row="1" x:Name="TransactionListView">
    <ListView.View>
        <GridView>
            <GridViewColumn Header="Datum"        Width="100"
                            DisplayMemberBinding="{Binding Date, StringFormat=dd.MM.yyyy}"/>
            <GridViewColumn Header="Typ"          Width="80"
                            DisplayMemberBinding="{Binding Type}"/>
            <GridViewColumn Header="Kategorie"    Width="120"
                            DisplayMemberBinding="{Binding Category}"/>
            <GridViewColumn Header="Beschreibung" Width="250"
                            DisplayMemberBinding="{Binding Description}"/>
            <GridViewColumn Header="Betrag"       Width="100"
                            DisplayMemberBinding="{Binding Amount, StringFormat=C}"/>
        </GridView>
    </ListView.View>
</ListView>
```

**Was bedeutet `DisplayMemberBinding="{Binding Date}"`?**
Eine Anweisung an die ListView: *„Lies aus jedem Element die Property `Date` aus und zeig sie in dieser Spalte."* Das funktioniert, weil unsere `Transaction`-Klasse genau diese Property-Namen hat.

**Was ist `StringFormat=C`?** Formatiert die Zahl als Währung in der System-Sprache (z. B. `"12,50 €"`). `dd.MM.yyyy` bei der Datums-Spalte erzeugt z. B. `"24.04.2026"`.

### 3.4 Kontostand-Anzeige

```xml
<TextBlock Grid.Row="2"
           x:Name="BalanceTextBlock"
           Text="Kontostand: 0,00 €"
           FontSize="16"
           FontWeight="Bold"
           HorizontalAlignment="Right"
           Margin="0,10,0,0"/>
```

Erstmal nur ein statischer Text. Die echte Berechnung folgt in Schritt 6.

---

## Schritt 4 – Logik zum Hinzufügen von Einträgen

### 4.1 Liste anlegen

```csharp
private List<Transaction> _transactions = new List<Transaction>();
```

**Warum `private`?** Niemand außerhalb der `MainWindow`-Klasse soll die Liste direkt verändern – alle Änderungen laufen über unseren Code-Behind.

**Warum der Unterstrich?** Konvention in C#: private Felder beginnen mit `_`. So sieht man sofort, dass es kein Parameter und keine lokale Variable ist.

**Hinweis:** Dieser Typ wird in 4.5 zu `ObservableCollection<Transaction>` umgebaut.

### 4.2 Click-Handler verdrahten

```csharp
private void AddButton_Click(object sender, RoutedEventArgs e)
{
    // Logik kommt in 4.3 / 4.4
}
```

**Wie findet WPF diese Methode?** Im XAML steht `Click="AddButton_Click"`. WPF sucht im zugehörigen Code-Behind automatisch nach einer Methode mit diesem Namen und passender Signatur.

**Was sind `sender` und `e`?**
- `sender` = das Objekt, das das Event ausgelöst hat – hier unser Button.
- `e` = zusätzliche Event-Daten (für Click meistens nicht nötig).

### 4.3 Eingaben auslesen, parsen, validieren

```csharp
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
```

**`decimal.TryParse(..., out decimal amount)`**
Versucht den String umzuwandeln. Der Rückgabewert ist `true` bei Erfolg, `false` bei Misserfolg. Über das `out`-Schlüsselwort schreibt die Methode das Ergebnis direkt in unsere Variable. Mit `!` davor: *„Wenn die Umwandlung schiefgeht, dann …"* – Fehlermeldung anzeigen und mit `return` abbrechen, damit der Rest des Handlers nicht mehr läuft.

**`?? DateTime.Today`** – der Null-Coalescing-Operator
`SelectedDate` ist vom Typ `DateTime?` (kann also `null` sein). Bei `null` nehmen wir den heutigen Tag.

**`as ComboBoxItem`**
Sicherer Cast: schlägt die Umwandlung fehl, kommt `null` heraus statt einer Exception. `?.` (Null-Conditional) ruft die nächste Property nur auf, wenn das Objekt nicht `null` ist.

**`? :` – ternärer Operator**
Kompakte if/else-Schreibweise als Ausdruck: `Bedingung ? wennWahr : wennFalsch`.

### 4.4 Transaction erzeugen, hinzufügen, Felder leeren

```csharp
// 5. Neues Transaction-Objekt erzeugen
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

// 7. Eingabefelder zurücksetzen
AmountTextBox.Clear();
CategoryTextBox.Clear();
DescriptionTextBox.Clear();
DatePicker.SelectedDate = null;
TypeComboBox.SelectedIndex = -1;
```

**Object-Initializer-Syntax** (`new Transaction { ... }`)
Kompakter als zuerst `new Transaction()` und dann fünf Zeilen mit `transaction.Amount = …` zu schreiben. In C# der Standard, sobald alle Properties einen Setter haben.

**Warum verschiedene Wege zum Leeren?**
- `TextBox` hat eine bequeme `Clear()`-Methode.
- `DatePicker.SelectedDate` setzen wir auf `null` (= „nichts ausgewählt").
- `ComboBox` hat keine `Clear`-Methode; `SelectedIndex = -1` heißt: kein Item ausgewählt.

### 4.5 ListView mit der Liste verbinden

**using ergänzen:**

```csharp
using System.Collections.ObjectModel;
```

**Feld umstellen:**

```csharp
private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();
```

**Konstruktor anpassen:**

```csharp
public MainWindow()
{
    InitializeComponent();
    TransactionListView.ItemsSource = _transactions;
}
```

**Warum `ObservableCollection<T>` statt `List<T>`?**
- `List<T>` ist eine *„stille"* Datenstruktur – sie sagt niemandem Bescheid, wenn sich etwas ändert.
- `ObservableCollection<T>` löst bei jeder Änderung (Add, Remove, …) ein Event aus.
- Die ListView hört auf dieses Event und zeichnet sich automatisch neu, sobald wir `Add` aufrufen.

**Warum `ItemsSource` erst nach `InitializeComponent()`?**
`InitializeComponent` baut das XAML aus und legt erst dort die UI-Elemente im Speicher an. Davor wäre `TransactionListView` noch `null` und es käme ein `NullReferenceException`.

**Wie finden die Spalten ihre Werte?**
Die `DisplayMemberBinding`-Ausdrücke aus 3.3 ziehen sich für jedes Element der Liste die richtige Property und schreiben sie in die zugehörige Spalte.

---

## Schritt 5 – Liste der Einträge anzeigen
Bereits durch 4.5 vollständig abgedeckt. Die GridView-Spalten kennen die Property-Namen, die `ItemsSource`-Zuweisung verbindet die ListView mit der Liste, `ObservableCollection` sorgt für automatische Aktualisierung.

---

## Schritt 6 – Kontostand berechnen
*(noch offen – wird im nächsten Arbeitsschritt umgesetzt)*
