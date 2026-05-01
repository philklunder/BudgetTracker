# Budget Tracker (WPF, C#)

Eine einfache Desktop-App als Lernprojekt, um C# und WPF besser zu verstehen.

## Ziel des Projekts

Ich möchte klein anfangen und die App Schritt für Schritt erweitern. Das Projekt soll so aufgebaut sein, dass ich dabei wirklich lerne, wie man eine saubere C#-Anwendung strukturiert – ohne von Anfang an in eine zu komplizierte Architektur zu rutschen.

## Mein aktueller Wissensstand

- Grundlagen in C#
- Objektorientierte Programmierung
- Vererbung und Polymorphismus
- Noch am Lernen – deshalb bitte keine überkomplizierte Architektur am Anfang

## Technologien

- C#
- WPF (.NET)

## Wichtige Anforderungen an den Lernprozess

- Mit einer einfachen ersten Version starten
- WPF verwenden
- Code verständlich erklären
- Saubere Projektstruktur
- Schrittweise arbeiten
- Zuerst nur die nötigsten Funktionen bauen
- Weitere Features erst später hinzufügen
- Am Anfang noch keine Datenbank
- Daten später zuerst in JSON speichern
- Unnötig komplizierte Patterns am Anfang vermeiden
- Wenn sinnvoll, später erklären, wann MVVM nützlich wird

## Features der ersten Version

1. Einnahmen hinzufügen
2. Ausgaben hinzufügen
3. Betrag, Kategorie, Beschreibung und Datum eingeben
4. Alle Einträge in einer Liste anzeigen
5. Den aktuellen Kontostand berechnen

## Reihenfolge der Umsetzung

1. Projektstruktur planen
2. Benötigte Klassen festlegen
3. Einfaches UI in WPF entwerfen
4. Logik zum Hinzufügen von Einträgen bauen
5. Liste der Einträge anzeigen
6. Kontostand berechnen

## Arbeitsweise (wichtig)

- Nicht sofort das komplette riesige Projekt auf einmal
- In kleinen, sinnvollen Schritten arbeiten
- Zuerst nur Schritt 1 zeigen
- Wenn Schritt 1 fertig ist, mit Schritt 2 weitermachen
- Bei jedem Schritt kurz erklären, warum wir es so machen
- Auf sauberen und lesbaren Code achten
- Gute Namen für Klassen, Methoden und Variablen verwenden
- Wenn Code gegeben wird, dann vollständig und lauffähig für den jeweiligen Schritt

Wenn ich sage, dass wir mit Schritt 1, 2, 3 oder 4 anfangen, möchte ich, dass du mir ganz genau und gut erklärst, was wir jetzt machen, und mir Schritt für Schritt dabei hilfst – sodass ich so viel wie möglich selbst schreiben kann.

## Geplante Erweiterungen (in dieser Reihenfolge)

- Einträge löschen
- Einträge bearbeiten
- Daten in JSON speichern und laden (mit `System.Text.Json`)
- Filtern nach Kategorie
- Monatsübersicht
- Einfache Auswertung
- Später optional MVVM

## Versionierung

Jede in sich abgeschlossene Version wird auf einem eigenen Branch als Momentaufnahme festgehalten.
Auf `main` läuft immer die aktuellste Entwicklung – also die jeweils neueste Version inklusive der Arbeit an der nächsten.

| Version | Branch | Inhalt |
|---------|--------|--------|
| **v1**  | `v1`   | Erste lauffähige Version: Einnahmen/Ausgaben hinzufügen, Liste anzeigen, Kontostand berechnen |
| v2      | *(geplant)* | Einträge löschen und bearbeiten |
| v3      | *(geplant)* | Persistenz: Daten in JSON speichern und laden |
| v4      | *(geplant)* | Filtern nach Kategorie + Monatsübersicht |
| v5      | *(geplant)* | Refactoring nach MVVM-Pattern |

Den Stand einer bestimmten Version lokal anschauen:
```bash
git checkout v1     # in den v1-Stand wechseln
git checkout main   # zurück zur aktuellen Entwicklung
```

---

# Schritt-für-Schritt-Anleitung

Diese Anleitung beschreibt rein schriftlich, **was** in jedem Schritt zu tun ist – ohne Code-Beispiele.
Den Code und die Begründung *warum* findet man zu jedem Schritt in der `DOKUMENTATION.md`.

---

## Schritt 1 – Projektstruktur planen

### 1.1 WPF-Projekt anlegen
- In Visual Studio ein neues Projekt vom Typ **„WPF-Anwendung (.NET)"** erstellen
- Projektnamen `BudgetTracker` wählen
- Die Standard-Vorlage übernehmen (App.xaml, MainWindow.xaml, AssemblyInfo.cs werden automatisch erzeugt)

### 1.2 Ordnerstruktur vorbereiten
- Im Projekt einen neuen Ordner `Models` anlegen
- In diesen Ordner kommen später alle reinen Datenklassen

---

## Schritt 2 – Benötigte Klassen festlegen

### 2.1 Enum für den Transaktionstyp
- Im Ordner `Models` eine neue Datei `TransactionType.cs` anlegen
- Darin ein öffentliches Enum mit den beiden Werten `Income` und `Expense` definieren

### 2.2 Transaction-Klasse
- Im Ordner `Models` eine neue Datei `Transaction.cs` anlegen
- Eine öffentliche Klasse `Transaction` mit folgenden Properties:
  - `Amount` (Typ `decimal`)
  - `Category` (Typ `string`, Default-Wert: leerer String)
  - `Description` (Typ `string`, Default-Wert: leerer String)
  - `Date` (Typ `DateTime`)
  - `Type` (Typ `TransactionType`)

---

## Schritt 3 – Einfaches UI in WPF entwerfen

### 3.1 Fenster und Hauptlayout
- In `MainWindow.xaml` Titel und Größe des Fensters festlegen (z. B. 800 × 500)
- Das Wurzel-Grid in drei Zeilen aufteilen: oben `Auto`, Mitte `*`, unten `Auto`
- Einen Außenrand (Margin) von 10 setzen

### 3.2 Eingabeformular
- In Zeile 0 eine `GroupBox` mit dem Header „Neuer Eintrag" einfügen
- Innerhalb der GroupBox ein Grid mit zwei Spalten (fix 120 / flexibel `*`) und sechs Zeilen
- Pro Zeile ein Label und ein Eingabefeld:
  - Betrag → TextBox mit `x:Name="AmountTextBox"`
  - Kategorie → TextBox mit `x:Name="CategoryTextBox"`
  - Beschreibung → TextBox mit `x:Name="DescriptionTextBox"`
  - Datum → DatePicker mit `x:Name="DatePicker"`
  - Typ → ComboBox mit `x:Name="TypeComboBox"` und zwei Items: „Einnahme" und „Ausgabe"
- In der letzten Zeile einen Button mit `x:Name="AddButton"` und Click-Event `AddButton_Click`

### 3.3 ListView für die Einträge
- In Zeile 1 eine ListView mit `x:Name="TransactionListView"` einfügen
- Innerhalb der ListView eine GridView mit den Spalten definieren:
  - Datum (gebunden an `Date`, Format `dd.MM.yyyy`)
  - Typ (gebunden an `Type`)
  - Kategorie (gebunden an `Category`)
  - Beschreibung (gebunden an `Description`)
  - Betrag (gebunden an `Amount`, Format Währung)

### 3.4 Kontostand-Anzeige
- In Zeile 2 einen `TextBlock` mit `x:Name="BalanceTextBlock"` einfügen
- Initialer Text: „Kontostand: 0,00 €"
- Schrift: Größe 16, fett, rechtsbündig

---

## Schritt 4 – Logik zum Hinzufügen von Einträgen

### 4.1 Liste für Transactions anlegen
- In `MainWindow.xaml.cs` ein privates Feld vom Typ `List<Transaction>` anlegen
- Konvention: Private Feldnamen beginnen mit Unterstrich (`_transactions`)
- Direkt mit einer leeren Liste initialisieren
- Hinweis: dieser Typ wird in Schritt 4.5 zu `ObservableCollection<Transaction>` geändert

### 4.2 Button-Click-Handler verdrahten
- Eine private Methode `AddButton_Click(object sender, RoutedEventArgs e)` in `MainWindow.xaml.cs` anlegen
- Diese Methode wird automatisch aufgerufen, weil im XAML `Click="AddButton_Click"` steht

### 4.3 Eingaben auslesen, parsen, validieren
Im Click-Handler nacheinander:

1. Den Text aus `AmountTextBox` mit `decimal.TryParse` in eine Dezimalzahl umwandeln. Schlägt das fehl: Fehlermeldung anzeigen und mit `return` abbrechen.
2. `Category` und `Description` direkt als String aus den jeweiligen TextBoxen auslesen.
3. Das Datum aus `DatePicker.SelectedDate` lesen. Falls nichts ausgewählt ist: das heutige Datum als Fallback nehmen.
4. Aus `TypeComboBox` das ausgewählte Item holen, daraus den Anzeigetext lesen und auf den entsprechenden `TransactionType`-Wert (`Income` / `Expense`) abbilden.
5. (Optional zur Kontrolle:) Eine MessageBox mit den erkannten Werten zeigen – wird in 4.4 wieder entfernt.

### 4.4 Transaction erzeugen, hinzufügen, Felder leeren
1. Mit Object-Initializer-Syntax ein neues `Transaction`-Objekt erzeugen und mit den fünf ausgelesenen Werten befüllen.
2. Das Objekt mit `Add` zur `_transactions`-Liste hinzufügen.
3. Alle Eingabefelder zurücksetzen:
   - TextBoxen mit `Clear()` leeren
   - `DatePicker.SelectedDate` auf `null` setzen
   - `TypeComboBox.SelectedIndex` auf `-1` setzen
4. Die Test-MessageBox aus 4.3 entfernen.

### 4.5 ListView mit der Liste verbinden
1. Den `using`-Eintrag `System.Collections.ObjectModel` ergänzen.
2. Den Typ des `_transactions`-Feldes von `List<Transaction>` auf `ObservableCollection<Transaction>` ändern.
3. Im Konstruktor – nach `InitializeComponent()` – die `ItemsSource` der ListView auf `_transactions` setzen.
4. Nicht mehr benötigte `using`-Direktiven aus dem Code-Behind entfernen.

---

## Schritt 5 – Liste der Einträge anzeigen
In diesem Schritt ist **kein neuer Code** mehr nötig. Die Funktionalität wurde bereits in zwei früheren Schritten umgesetzt:

- **Schritt 3.3** legt mit den `GridViewColumn`s fest, **welche Spalten** die ListView hat und an welche Property von `Transaction` jede Spalte gebunden ist.
- **Schritt 4.5** verbindet mit `ItemsSource = _transactions` die ListView mit unserer Datenquelle und sorgt mit `ObservableCollection<T>` dafür, dass neue Einträge **automatisch** in der Tabelle erscheinen.

Damit ist die Anzeige der Einträge vollständig fertig. Eine ausführlichere Begründung steht in `DOKUMENTATION.md` unter Schritt 5.

---

## Schritt 6 – Kontostand berechnen

### 6.1 Berechnungsmethode `UpdateBalance` anlegen
- In `MainWindow.xaml.cs` eine neue private Methode `UpdateBalance()` ohne Rückgabewert anlegen
- Eine lokale Variable `balance` vom Typ `decimal` mit Startwert `0` deklarieren
- Mit einer `foreach`-Schleife über alle Einträge in `_transactions` iterieren
- Pro Eintrag prüfen, ob der `Type` gleich `TransactionType.Income` ist:
  - wenn ja: den Betrag zur `balance` addieren
  - wenn nein (also Ausgabe): den Betrag von der `balance` abziehen
- Nach der Schleife `BalanceTextBlock.Text` mit dem berechneten Wert setzen, formatiert als Währung

### 6.2 `UpdateBalance` an den richtigen Stellen aufrufen
- **Im Konstruktor:** nach der Zeile `TransactionListView.ItemsSource = _transactions;` einmal `UpdateBalance()` aufrufen, damit beim Programmstart der Kontostand dynamisch aus der (leeren) Liste berechnet wird
- **Am Ende von `AddButton_Click`:** nach dem Zurücksetzen der Eingabefelder ebenfalls `UpdateBalance()` aufrufen, damit der Kontostand nach jedem neuen Eintrag aktualisiert wird

### 6.3 Einheitliche Währungsformatierung im UI
- In `MainWindow.xaml` am `<Window …>`-Tag das Attribut `xml:lang="de-CH"` ergänzen
- Damit verwenden alle XAML-Bindings (z. B. die Betragsspalte mit `StringFormat=C`) dieselbe Kultur wie der C#-Code und zeigen einheitlich `CHF` statt `$`

---

## Schritt 7 – Einträge löschen

Ab hier beginnt die Arbeit an **v2**. Ziel von Schritt 7 ist es, einen ausgewählten Eintrag aus der Liste entfernen zu können.

### 7.1 Löschen-Button im UI hinzufügen
- In `MainWindow.xaml` das Wurzel-Grid um eine vierte Zeile erweitern (`<RowDefinition Height="Auto"/>` zusätzlich)
- Zwischen ListView und Kontostand in Zeile 2 ein neues `StackPanel` mit `Orientation="Horizontal"` einfügen
- In das StackPanel einen `Button` mit `x:Name="DeleteButton"`, Beschriftung „Löschen" und `Click="DeleteButton_Click"` platzieren
- Den bestehenden `BalanceTextBlock` von `Grid.Row="2"` auf `Grid.Row="3"` ändern, damit er unter den Aktions-Buttons bleibt
- In `MainWindow.xaml.cs` einen vorerst leeren Click-Handler `DeleteButton_Click(object sender, RoutedEventArgs e)` anlegen, damit das Projekt kompiliert. Die eigentliche Lösch-Logik kommt in 7.2.

---

## Status

Work in Progress – wird laufend erweitert, während ich neue Konzepte lerne.
