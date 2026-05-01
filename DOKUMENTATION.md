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

In diesem Schritt wird **kein neuer Code** geschrieben – die Funktionalität wurde bereits in Schritt 4 vollständig erledigt. Trotzdem lohnt es sich, hier nochmal zu sammeln, *was* genau das Anzeigen ausmacht und *wo* es im Projekt zusammenkommt.

**Das Anzeigen einer Liste in WPF besteht aus drei Teilen:**

1. **Spalten definieren** – „Welche Felder sollen in welcher Reihenfolge zu sehen sein und wie heißen die Überschriften?"
   → erledigt in **Schritt 3.3** über die `GridViewColumn`s mit `DisplayMemberBinding="{Binding Date}"`, `{Binding Category}` usw.

2. **Datenquelle anhängen** – „Wo holt sich die ListView ihre Einträge her?"
   → erledigt in **Schritt 4.5** mit `TransactionListView.ItemsSource = _transactions;`

3. **Automatische Aktualisierung** – „Wie merkt die ListView, dass ein neuer Eintrag dazukam?"
   → erledigt ebenfalls in **Schritt 4.5**, indem das Feld nicht als `List<T>` sondern als `ObservableCollection<T>` typisiert wurde. Diese Klasse löst bei jeder Änderung ein Event aus, auf das die ListView hört und sich neu zeichnet.

**Warum tauchen die drei Teile nicht in einem einzigen „Schritt 5"-Block auf?**
Die UI-Definition (Spalten) gehört logisch zum UI-Aufbau (Schritt 3) – sonst wäre die XAML-Datei zerfetzt. Die Datenanbindung gehört zur Hinzufügen-Logik (Schritt 4), weil die `ObservableCollection` nur dort Sinn ergibt. Schritt 5 ist also keine eigene Aufgabe, sondern das **Zusammenwirken** der vorhergehenden Schritte – und genau das wollten wir mit WPF erreichen: möglichst viel deklarativ im XAML, statt manuell Items per Code anzuhängen.

---

## Schritt 6 – Kontostand berechnen

### 6.1 Berechnungsmethode `UpdateBalance`

```csharp
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

    BalanceTextBlock.Text = $"Kontostand: {balance:C}";
}
```

**Warum eine eigene Methode statt den Code direkt einzubauen?**
- *Wiederverwendbarkeit:* Wir brauchen die Berechnung sowohl beim Programmstart als auch nach jedem neuen Eintrag. Wäre die Logik direkt in `AddButton_Click`, hätten wir entweder Duplikate oder müssten den Konstruktor künstlich den Click-Handler aufrufen lassen.
- *Saubere Trennung von Aufgaben:* `AddButton_Click` macht „ein Eintrag hinzufügen", `UpdateBalance` macht „Kontostand neu berechnen". Eine Methode = eine klar umrissene Aufgabe.

**`balance += transaction.Amount;`** ist die Kurzform für `balance = balance + transaction.Amount;`. Genauso `-=` für die Subtraktion. Diese Compound-Operatoren sind in C# Standard und sehr verbreitet.

**Warum `foreach` statt `for`?** Wir brauchen keinen Index – wir wollen einfach jeden Eintrag der Reihe nach durchgehen. `foreach` zeigt diese Absicht klar und ist weniger fehleranfällig.

**`$"Kontostand: {balance:C}"`** – ein interpolierter String:
- Das `$` davor erlaubt es, mitten im String C#-Ausdrücke in `{ }` zu schreiben.
- Der Doppelpunkt `:C` ist eine Format-Anweisung für *Currency*. Welche Währung am Ende erscheint, hängt von der Sprache des Systems ab (in der Schweiz `CHF`, in Deutschland `€`, in den USA `$`).

**Alternative mit LINQ (zur Information, nicht im Code verwendet):**
```csharp
decimal balance = _transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
```
Eine einzige Zeile statt einer ganzen Schleife. LINQ ist sehr mächtig, aber für den Anfang ist die `foreach`-Variante leichter zu lesen und im Debugger Schritt für Schritt nachvollziehbar.

### 6.2 `UpdateBalance` aufrufen

**Im Konstruktor:**
```csharp
public MainWindow()
{
    InitializeComponent();
    TransactionListView.ItemsSource = _transactions;
    UpdateBalance();
}
```
Damit beim Programmstart sofort der berechnete Wert (am Anfang `0,00 CHF`) erscheint – und nicht ein hartkodierter Text aus dem XAML.

**Am Ende von `AddButton_Click`:**
```csharp
// ... vorheriger Code (Felder zurücksetzen) ...
UpdateBalance();
```
So wird nach jedem neuen Eintrag der Kontostand neu berechnet und im UI angezeigt.

**Warum reicht das?** Solange wir nur über `_transactions.Add(...)` Einträge hinzufügen und sonst nichts an der Liste ändern, kennen wir alle Stellen, an denen sich der Kontostand ändern kann. Sobald wir später „Löschen" oder „Bearbeiten" einbauen, müssen wir `UpdateBalance()` auch dort aufrufen.

### 6.3 Einheitliche Währungsformatierung im XAML

**Was hinzuzufügen ist** – am `<Window …>`-Tag in `MainWindow.xaml`:
```xml
<Window x:Class="BudgetTracker.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xml:lang="de-CH"
        Title="Budget Tracker" Height="500" Width="800">
```

**Warum braucht es das überhaupt?**
Eine bekannte WPF-Eigenheit: `{balance:C}` im **C#-Code** verwendet automatisch die Sprache des Betriebssystems. `StringFormat=C` in einem **XAML-Binding** dagegen verwendet standardmäßig **en-US** – also Dollar. Ohne Korrektur sähe man:
- im `BalanceTextBlock` (Code): `Kontostand: CHF 100.00`
- in der ListView-Spalte (XAML): `$100.00`

Mit `xml:lang="de-CH"` am `Window` erbt jedes Binding innerhalb dieses Fensters die schweizerdeutsche Kultur. Damit sind UI und Code-Behind konsistent. Diese Lösung wirkt für **alle** zukünftigen Bindings – wir müssen das nicht für jede Spalte einzeln nachpflegen.

---

## Schritt 7 – Einträge löschen

Mit Schritt 7 startet die Arbeit an **v2**. Wir bauen Stück für Stück die Funktion „Eintrag aus der Liste entfernen" auf. 7.1 legt zunächst nur das UI-Skelett an (Button + leerer Click-Handler), die eigentliche Lösch-Logik folgt in 7.2.

### 7.1 Löschen-Button im UI hinzufügen

**`MainWindow.xaml` – vierte Zeile im Grid ergänzen:**

```xml
<Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="*"/>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="Auto"/>
</Grid.RowDefinitions>
```

**`MainWindow.xaml` – neue Aktionsleiste zwischen ListView und Kontostand:**

```xml
<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10,0,0">
    <Button x:Name="DeleteButton"
            Content="Löschen"
            Click="DeleteButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"/>
</StackPanel>
```

**`MainWindow.xaml` – Kontostand rückt eine Zeile nach unten:**

```xml
<TextBlock Grid.Row="3"
           x:Name="BalanceTextBlock"
           Text="Kontostand: 0,00 €"
           FontSize="16"
           FontWeight="Bold"
           HorizontalAlignment="Right"
           Margin="0,10,0,0"/>
```

**`MainWindow.xaml.cs` – leerer Click-Handler-Stub:**

```csharp
private void DeleteButton_Click(object sender, RoutedEventArgs e)
{
    // Logik folgt in Schritt 7.2
}
```

**Warum eine eigene Zeile für die Aktions-Buttons?**
Bisher lag der Kontostand direkt unter der ListView. Wir trennen jetzt sauber **Aktionen** (links, Buttons) von **Anzeige** (rechts, Kontostand) – jede mit eigener Grid-Zeile. So überlagert sich später nichts, auch wenn ein zweiter Button („Bearbeiten") dazukommt.

**Warum schon ein `StackPanel` für nur einen Button?**
Ein `StackPanel` reiht seine Kinder einfach hintereinander auf – horizontal oder vertikal. In 8.1 fügen wir den „Bearbeiten"-Button daneben ein, ohne irgendetwas am Layout-Container ändern zu müssen. Alternative wäre ein neues Grid mit Spalten – aber das ist Overkill für eine simple Reihe.

**Warum `Padding` und `Margin` gleichzeitig am Button?**
- `Padding="20,5"` – Abstand **innen**, also zwischen Buttonrand und Beschriftung. Macht den Button optisch nicht zu eng.
- `Margin="0,0,10,0"` – Abstand **außen**, hier 10 px nach rechts. Damit hat der nächste Button (kommt in 8.1) eine kleine Lücke und klebt nicht direkt am Lösch-Button.

| Format | Reihenfolge |
|--------|-------------|
| `"x,y"` | links/rechts, oben/unten |
| `"links,oben,rechts,unten"` | alle vier Seiten einzeln |

**Warum brauchen wir den leeren Click-Handler-Stub?**
Sobald im XAML `Click="DeleteButton_Click"` steht, sucht WPF beim Kompilieren nach einer Methode genau dieses Namens im Code-Behind. Fehlt die Methode, scheitert der Build mit einem Compiler-Fehler. Mit dem leeren Stub kompiliert das Projekt, der Button erscheint im Fenster – beim Klick passiert nur (noch) nichts. Genau das gleiche Vorgehen hatten wir in Schritt 4.2 mit dem `AddButton_Click`.

**Wofür ist `Grid.Row` an einem Element?**
`Grid.Row` ist eine sogenannte **Attached Property**: Sie gehört nicht zum Element selbst (Button, TextBlock …), sondern wird vom **Eltern-Grid** ausgewertet. Damit weiß das Grid, in welche Zeile das Kind gehört. Standard ist `Grid.Row="0"` – deshalb mussten wir die `<Grid.RowDefinitions>` für das Eingabeformular nicht extra mit `Grid.Row="0"` markieren, alle weiteren Elemente brauchen den Hinweis aber explizit.
