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

### 7.2 Lösch-Logik im Click-Handler

```csharp
private void DeleteButton_Click(object sender, RoutedEventArgs e)
{
    // 1. Ausgewählten Eintrag aus der ListView holen
    Transaction? selected = TransactionListView.SelectedItem as Transaction;

    // 2. Wenn nichts ausgewählt ist: Hinweis anzeigen und abbrechen
    if (selected is null)
    {
        MessageBox.Show("Bitte zuerst einen Eintrag in der Liste auswählen.");
        return;
    }

    // 3. Eintrag aus der Liste entfernen
    _transactions.Remove(selected);

    // 4. Kontostand neu berechnen
    UpdateBalance();
}
```

**Warum hat `SelectedItem` den Typ `object?` und nicht `Transaction`?**
Eine WPF-`ListView` ist generisch und kann jeden beliebigen Typ anzeigen. Sie hat zur Compile-Zeit keine Ahnung, dass *unsere* `ItemsSource` eine `ObservableCollection<Transaction>` ist. Deshalb ist die Rückgabe `object?` – wir müssen selbst zurück in den richtigen Typ casten.

**Cast-Möglichkeiten in C# – ein Überblick:**

| Schreibweise | Verhalten bei falschem Typ | Wann nehmen? |
|--------------|----------------------------|--------------|
| `(Transaction)x` | wirft `InvalidCastException` | Wenn man absolut sicher ist, dass der Typ stimmt |
| `x as Transaction` | gibt `null` zurück | Wenn Typ-Mismatch oder `null`-Wert möglich ist |
| `x is Transaction t` | `bool` + Variable; weist nur bei Erfolg zu | Wenn man Cast und Prüfung in einem Schritt will |

Wir nehmen `as`, weil `SelectedItem` auch dann `null` ist, wenn **gar nichts** ausgewählt wurde – und mit dem anschließenden Null-Check decken wir beide Fälle (kein Item ausgewählt **oder** falscher Typ) gemeinsam ab.

**Warum `Transaction?` mit Fragezeichen?**
Das `?` signalisiert dem Compiler: *„diese Variable darf `null` sein"* (nullable reference type). Da `as` im Fehlerfall `null` zurückgibt, **muss** der Variablen-Typ das ausdrücklich erlauben – sonst bekommen wir eine Compiler-Warnung.

**Warum funktioniert `_transactions.Remove(selected)` einfach so?**
`Remove(item)` entfernt das **erste Element**, das gleich `item` ist. Bei Referenztypen wie unserer `Transaction`-Klasse heißt „gleich": dasselbe Objekt im Speicher (Referenzgleichheit). Da `selected` exakt das Objekt aus der Liste ist – die ListView reicht uns nur den Verweis weiter, sie kopiert nichts –, wird genau dieses entfernt. Property-Werte sind dabei egal: zwei `Transaction`-Objekte mit identischem Inhalt würden als verschieden gelten, weil sie unterschiedliche Speicherplätze haben.

**Warum aktualisiert sich die ListView automatisch nach `Remove`?**
Weil `_transactions` eine `ObservableCollection<T>` ist (siehe Schritt 4.5). Nach jedem `Remove` löst sie ein Change-Event aus, auf das die ListView hört und sich neu zeichnet. Wir müssen das UI nicht manuell aktualisieren.

**Warum dann doch `UpdateBalance()` von Hand?**
`ObservableCollection` informiert nur Steuerelemente, die **direkt** auf sie gebunden sind – also unsere ListView. Der `BalanceTextBlock` ist nur ein gewöhnlicher TextBlock, der über `BalanceTextBlock.Text = ...` befüllt wird. Den müssen wir selbst neu berechnen lassen, genau wie nach dem Hinzufügen.

### 7.3 Bestätigungs-Abfrage vor dem Löschen

**Erweiterung in `DeleteButton_Click` zwischen Schritt 2 (Null-Check) und Schritt 3 (Entfernen):**

```csharp
// 3. Bestätigungs-Abfrage (Ja/Nein)
MessageBoxResult result = MessageBox.Show(
    $"Eintrag wirklich löschen?\n\n{selected.Date:dd.MM.yyyy} – {selected.Category}: {selected.Amount:C}",
    "Löschen bestätigen",
    MessageBoxButton.YesNo,
    MessageBoxImage.Question);

// 4. Wenn der Nutzer NICHT auf "Ja" geklickt hat: abbrechen
if (result != MessageBoxResult.Yes)
{
    return;
}
```

**Die `MessageBox.Show`-Familie**

Bisher haben wir nur die einfache Form `MessageBox.Show("Text")` benutzt – ein OK-Button, kein Rückgabewert. Es gibt aber Überladungen mit zusätzlichen Parametern:

| Parameter | Typ | Bedeutung |
|-----------|-----|-----------|
| `messageBoxText` | `string` | Inhalt der Nachricht |
| `caption` | `string` | Titelzeile des Fensters |
| `button` | `MessageBoxButton` | Welche Buttons? (`OK`, `OKCancel`, `YesNo`, `YesNoCancel`) |
| `icon` | `MessageBoxImage` | Welches Symbol? (`None`, `Information`, `Warning`, `Question`, `Error`) |

Der **Rückgabewert** vom Typ `MessageBoxResult` zeigt, welchen Button der Nutzer geklickt hat (`Yes`, `No`, `OK`, `Cancel`, `None`).

**Warum eine Vorschau im Dialogtext?**
„Wirklich löschen?" allein ist zu abstrakt. Mit Datum, Kategorie und Betrag (`{selected.Date:dd.MM.yyyy} – {selected.Category}: {selected.Amount:C}`) sieht der Nutzer in der Bestätigung **konkret**, was gleich verschwindet. Die Format-Spezifizierer (`:dd.MM.yyyy` für das Datum, `:C` für den Betrag) kennen wir aus der ListView (`StringFormat`) und Schritt 6.

**Warum `\n\n` im Text?**
`\n` ist der Escape-Code für einen Zeilenumbruch. Zwei davon erzeugen eine Leerzeile zwischen Frage und Eintragsdetails – sieht im Dialog deutlich besser aus.

**Warum `if (result != MessageBoxResult.Yes)` und nicht `if (result == MessageBoxResult.No)`?**
Bei `MessageBoxButton.YesNo` gibt es zwar nur diese zwei Optionen – der Nutzer kann den Dialog aber auch über das **X (Schließen)** oder mit **ESC** schließen. Ergebnis dann: `MessageBoxResult.None`. Mit `!= Yes` decken wir alle „nicht-bestätigt"-Fälle ab. Das ist ein typisches **Default-Deny-Muster**: bei jedem Zweifel abbrechen, lieber einmal zu wenig als einmal zu viel löschen.

**Warum zwei verschiedene Dialog-Typen im selben Click-Handler?**
Sie haben **unterschiedliche Funktionen**:
- Der erste Dialog (kein Eintrag ausgewählt) ist ein **Hinweis** – nur OK-Button reicht.
- Der zweite (Bestätigung) ist eine **Entscheidung** – `YesNo`-Buttons + `Question`-Icon.

Konsistenz nach Funktion, nicht nach Form: das richtige Icon hilft dem Nutzer, sofort einzuordnen, was von ihm erwartet wird.

---

## Schritt 8 – Einträge bearbeiten

Schritt 8 schließt v2 ab. Wir bauen die Bearbeiten-Funktion in vier kleinen Etappen auf: erst der Button (8.1), dann das Laden der Werte ins Formular (8.2), dann das Speichern in den bestehenden Eintrag (8.3), und zum Schluss eine Abbrechen-Möglichkeit (8.4).

### 8.1 Bearbeiten-Button im UI hinzufügen

**`MainWindow.xaml` – Aktionsleiste um zweiten Button erweitern:**

```xml
<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10,0,0">
    <Button x:Name="EditButton"
            Content="Bearbeiten"
            Click="EditButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"/>
    <Button x:Name="DeleteButton"
            Content="Löschen"
            Click="DeleteButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"/>
</StackPanel>
```

**`MainWindow.xaml.cs` – leerer Click-Handler-Stub:**

```csharp
private void EditButton_Click(object sender, RoutedEventArgs e)
{
    // Logik folgt in Schritt 8.2
}
```

**Warum den Bearbeiten-Button links?**
UX-Konvention: weniger destruktive Aktionen kommen vor destruktiveren. Bearbeiten ist umkehrbar (zur Not zurückeditieren), Löschen nicht. Dadurch landet die „gefährlichste" Schaltfläche am Rand und erschwert Versehensklicks.

**Warum genügt das einfache Voranstellen im XAML?**
`StackPanel` mit `Orientation="Horizontal"` ordnet seine Kinder in der Reihenfolge an, in der sie im XAML stehen. Wer zuerst kommt, steht links – keine extra Konfiguration nötig.

**Warum wieder ein leerer Stub?**
Gleiches Muster wie in 4.2 und 7.1: sobald `Click="EditButton_Click"` im XAML steht, sucht WPF beim Kompilieren nach einer Methode dieses Namens. Fehlt sie, scheitert der Build.

### 8.2 Werte des ausgewählten Eintrags ins Formular laden

```csharp
private void EditButton_Click(object sender, RoutedEventArgs e)
{
    // 1. Ausgewählten Eintrag aus der ListView holen
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
}
```

**Rück-Konvertierung – die zentrale Hürde**

Beim Hinzufügen (Schritt 4.3) sind wir vom UI-String **zum** typisierten Wert gegangen. Beim Bearbeiten gehen wir den umgekehrten Weg:

| Property | Quelle (Transaction) | Ziel (UI-Element) | Konvertierung |
|----------|---------------------|-------------------|---------------|
| `Amount` | `decimal` | `AmountTextBox.Text` (`string`) | `.ToString()` |
| `Category` | `string` | `CategoryTextBox.Text` | direkt |
| `Description` | `string` | `DescriptionTextBox.Text` | direkt |
| `Date` | `DateTime` | `DatePicker.SelectedDate` (`DateTime?`) | impliziter Cast `DateTime` → `DateTime?` |
| `Type` | `TransactionType` | `TypeComboBox.SelectedIndex` (`int`) | `Income → 0`, `Expense → 1` |

**Warum `selected.Amount.ToString()` ohne Format-Argument?**
`decimal.ToString()` ohne Argumente verwendet die aktuelle Kultur (de-CH bei uns). Beim Speichern parsen wir den Text mit `decimal.TryParse(...)` – dieselbe Kultur, also passt der Round-Trip. Ein Format wie `"C"` wäre **schädlich**, weil dann „CHF 12.50" im Textfeld stünde und `TryParse` das nicht zurück lesen kann.

**Warum geht `DatePicker.SelectedDate = selected.Date` einfach so?**
`Date` ist `DateTime`, `SelectedDate` ist `DateTime?`. Jeder konkrete `DateTime`-Wert lässt sich **implizit** in `DateTime?` umwandeln – ein nullables Feld kann natürlich auch einen echten Wert aufnehmen. Die andere Richtung (von `DateTime?` nach `DateTime`) bräuchte den `??`-Operator – die *enge* Richtung verlangt explizite Behandlung.

**Warum `SelectedIndex` statt `SelectedItem`?**
Unsere ComboBox hat zwei feste `ComboBoxItem`s im XAML (Index 0 = Einnahme, Index 1 = Ausgabe). Mit dem ternären Operator ist die Zuordnung Enum → UI eine Zeile. `SelectedItem` würde verlangen, dass wir ein passendes `ComboBoxItem`-Objekt aus den `Items` heraussuchen – mehr Code, kein Mehrwert bei dieser fixen Zwei-Optionen-Auswahl.

**Bewusste Lücke nach 8.2:** Beim Klick auf „Hinzufügen" mit geladenen Werten entsteht ein **neuer** Eintrag, das Original bleibt unverändert. Das ist gewollt – wir bauen Schritt für Schritt und reparieren das in 8.3.

### 8.3 Speichern-Button + Bearbeitungs-Modus

**`MainWindow.xaml` – AddButton in StackPanel verpacken, SaveButton daneben:**

```xml
<StackPanel Grid.Row="5" Grid.Column="1" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,10,0,0">
    <Button x:Name="AddButton"
            Content="Hinzufügen"
            Click="AddButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"/>
    <Button x:Name="SaveButton"
            Content="Speichern"
            Click="SaveButton_Click"
            Padding="20,5"
            IsEnabled="False"/>
</StackPanel>
```

**`MainWindow.xaml.cs` – neues Feld für den Modus:**

```csharp
private Transaction? _editingTransaction = null;
```

| Wert | Bedeutung |
|------|-----------|
| `null` | Kein Eintrag wird bearbeitet → Hinzufügen-Modus |
| Konkretes `Transaction`-Objekt | Dieser Eintrag wird gerade bearbeitet → Speichern-Modus |

**`EditButton_Click` aus 8.2 am Ende ergänzen:**

```csharp
// 4. In den Bearbeitungs-Modus wechseln
_editingTransaction = selected;
SaveButton.IsEnabled = true;
```

**`AddButton_Click` am Ende ergänzen:**

```csharp
// Falls vorher der Bearbeitungs-Modus aktiv war: beenden
_editingTransaction = null;
SaveButton.IsEnabled = false;
```

**Neuer Handler `SaveButton_Click`:**

```csharp
private void SaveButton_Click(object sender, RoutedEventArgs e)
{
    // Sicherheits-Check: ohne aktiven Bearbeitungs-Modus nichts tun
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

    // 4. Typ aus der ComboBox auslesen und auf das Enum mappen
    ComboBoxItem? selectedItem = TypeComboBox.SelectedItem as ComboBoxItem;
    string? typeText = selectedItem?.Content?.ToString();
    TransactionType type = typeText == "Einnahme"
        ? TransactionType.Income
        : TransactionType.Expense;

    // 5. Werte ins bestehende Objekt schreiben (statt ein neues anzulegen)
    _editingTransaction.Amount = amount;
    _editingTransaction.Category = category;
    _editingTransaction.Description = description;
    _editingTransaction.Date = date;
    _editingTransaction.Type = type;

    // 6. ListView zwingen, ihre Anzeige neu zu zeichnen
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

    // 9. Kontostand aktualisieren
    UpdateBalance();
}
```

**Warum ein „nullable Referenz als Modus-Schalter"?**
Statt eine zusätzliche `bool _isEditing`-Variable + eine separate `Transaction _editingItem`-Referenz zu pflegen, kombinieren wir beides in einer einzigen Variable. Eine Variable, eine Bedeutung – weniger Stellen, an denen die zwei Werte auseinanderlaufen können (Inkonsistenz vermeiden). Solche **State-Maschinen mit nullable Referenzen** sind ein häufiges, leichtgewichtiges Pattern für einfache Modi.

**Warum `TransactionListView.Items.Refresh()`?**
`ObservableCollection<T>` löst Events nur bei **Strukturänderungen** der Liste aus (Add, Remove, Insert, Replace). Eine reine Property-Änderung an einem bestehenden Element (`_editingTransaction.Amount = ...`) wird **nicht** signalisiert – die ListView weiß nichts vom Update und zeigt weiterhin die alten Werte.

**Drei Wege das zu lösen:**

| Variante | Aufwand | Wann sinnvoll |
|----------|---------|---------------|
| `INotifyPropertyChanged` in `Transaction` implementieren | hoch | sauber, idiomatisch in MVVM – heben wir uns für v5 auf |
| Element in der Liste durch ein neues ersetzen | mittel | wenn Original-Referenz egal ist |
| `Items.Refresh()` aufrufen | minimal | pragmatisch für kleine Listen |

Wir nehmen `Items.Refresh()` – ein „WPF-Hammer", der die ListView zwingt, alle Bindings neu auszuwerten. Bei wenigen Einträgen kein Performance-Problem.

**Warum `IsEnabled="False"` schon im XAML?**
Beim Programmstart ist nichts ausgewählt, der Speichern-Button hätte keinen Sinn. Wir starten ihn deaktiviert; aktiviert wird er erst durch `EditButton_Click`. Disabled-Zustand = grauer Button, keine Klicks.

**Warum nicht `Visibility="Collapsed"` (verstecken) statt `IsEnabled="False"`?**
`Collapsed` würde den Button **komplett aus dem Layout** entfernen, das StackPanel würde sich verkleinern. Beim Wechsel der Modi gäbe es einen Layout-Sprung. Mit `IsEnabled` bleibt das Layout stabil, nur die Verfügbarkeit ändert sich – ruhigere UX.

**Warum der Sicherheits-Check `if (_editingTransaction is null) return;` ganz am Anfang?**
Theoretisch kann der Button nicht geklickt werden, solange `_editingTransaction == null` (er wäre dann disabled). Aber: defensives Programmieren schadet nicht, und der Compiler braucht den Check, damit er weiß, dass die Variable in den nachfolgenden `_editingTransaction.Amount = amount;`-Zuweisungen garantiert nicht-null ist. Sonst gibt es Warnungen.

**Warum der ähnliche Code-Block wie in `AddButton_Click`?**
Eingaben auslesen + validieren ist beide Male identisch. Wir lassen die Duplikation bewusst stehen – sie ist nur ~15 Zeilen lang, beide Methoden bleiben so jeweils linear lesbar. Im v5-Refactoring auf MVVM räumen wir das mit der gesamten UI-Logik gemeinsam auf. Vorzeitig zu abstrahieren würde mehr Komplexität bringen, als sie wegnimmt – Stichwort *„premature abstraction"*.

**Warum auch in `AddButton_Click` den Modus zurücksetzen?**
Stell dir vor: Nutzer klickt „Bearbeiten", ändert Werte, klickt aber **„Hinzufügen"** statt „Speichern". Dann entsteht ein neuer Eintrag (gewünscht), aber `_editingTransaction` würde noch auf das Original zeigen. Würde der Nutzer jetzt direkt auf „Speichern" klicken, hätten wir leere Werte ins Original geschrieben (Bug!). Indem wir nach jedem `AddButton_Click` den Modus beenden, ist dieser Pfad sauber.

### 8.4 Abbrechen-Button

**`MainWindow.xaml` – dritten Button neben Speichern setzen, dem SaveButton ein Margin geben:**

```xml
<StackPanel Grid.Row="5" Grid.Column="1" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,10,0,0">
    <Button x:Name="AddButton"
            Content="Hinzufügen"
            Click="AddButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"/>
    <Button x:Name="SaveButton"
            Content="Speichern"
            Click="SaveButton_Click"
            Padding="20,5"
            Margin="0,0,10,0"
            IsEnabled="False"/>
    <Button x:Name="CancelButton"
            Content="Abbrechen"
            Click="CancelButton_Click"
            Padding="20,5"
            IsEnabled="False"/>
</StackPanel>
```

**`EditButton_Click` ergänzen:**

```csharp
CancelButton.IsEnabled = true;
```

**`AddButton_Click` und `SaveButton_Click` jeweils ergänzen:**

```csharp
CancelButton.IsEnabled = false;
```

**Neuer Handler `CancelButton_Click`:**

```csharp
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
```

**Was ist die Aufgabe von Abbrechen?**
Eine reine **Notbremse**: Felder leer, Modus weg, Save/Cancel wieder grau. Das Original in der Liste bleibt unangetastet, weil wir **nichts** in `_editingTransaction` zurückschreiben. Im Gegensatz zu Speichern ist Abbrechen **schreibfrei** – die geladenen Form-Werte werden einfach verworfen.

**Warum CancelButton parallel zu SaveButton an-/ausschalten?**
Beide Buttons gehören zum Bearbeitungs-Modus und sollen nur in diesem Modus geklickt werden können. Sie laufen synchron: aktiviert mit dem Wechsel in den Modus, deaktiviert mit dem Verlassen. Bei jedem Modus-Verlassen (Save, Cancel, Add) deaktivieren wir beide Buttons gemeinsam.

**Was fällt dir an `CancelButton_Click`, `SaveButton_Click` und `AddButton_Click` auf?**
Drei Stellen, die alle dieselben drei Zeilen am Ende haben:

```csharp
_editingTransaction = null;
SaveButton.IsEnabled = false;
CancelButton.IsEnabled = false;
```

Plus das Leeren der Eingabefelder. Das ist eine klassische Stelle, an der man später mit einer kleinen Helper-Methode (z. B. `ResetForm()` oder `ExitEditMode()`) Duplikate beseitigt – aber **bewusst noch nicht**. Solche Aufräumarbeiten machen wir gebündelt im v5-Refactoring auf MVVM. Drei ähnliche Zeilen sind besser als eine voreilige Abstraktion, die später wieder auseinandergenommen werden müsste, wenn MVVM andere Anforderungen mitbringt.
