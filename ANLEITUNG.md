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

## Fortschritt – was bisher gemacht wurde

### Schritt 1–3 (erledigt)
- **Schritt 1 – Projektstruktur:** WPF-Projekt angelegt, Ordner `Models/` für Datenklassen erstellt.
- **Schritt 2 – Klassen:** `Transaction` (Datenklasse mit `Amount`, `Category`, `Description`, `Date`, `Type`) und das Enum `TransactionType` (`Income`, `Expense`).
- **Schritt 3 – UI:** `MainWindow.xaml` mit Eingabeformular (Betrag, Kategorie, Beschreibung, Datum, Typ), `ListView` für die Einträge und `TextBlock` für den Kontostand.

### Schritt 4 – Logik zum Hinzufügen von Einträgen

Schritt 4 wurde in kleine Unter-Schritte zerlegt, damit man jede Idee für sich verstehen kann.

#### 4.1 – Liste für Transactions in MainWindow anlegen
In `MainWindow.xaml.cs` wird ein privates Feld angelegt:

```csharp
private List<Transaction> _transactions = new List<Transaction>();
```

**Warum?** Die App braucht einen Ort, an dem alle eingegebenen Einträge im Speicher gehalten werden, solange das Fenster offen ist. `List<Transaction>` ist die einfachste Wahl: dynamisch erweiterbar, leicht zu durchlaufen. Der Unterstrich (`_transactions`) ist eine verbreitete Konvention für private Felder.

#### 4.2 – Button-Click-Handler verdrahten
Im XAML hat der Button `Click="AddButton_Click"`. Dadurch sucht WPF beim Klick nach einer Methode dieses Namens im Code-Behind:

```csharp
private void AddButton_Click(object sender, RoutedEventArgs e)
{
    // Hier passiert gleich die eigentliche Logik
}
```

**Warum?** WPF arbeitet event-basiert – auf Aktionen des Nutzers (Klick, Tastatur, …) reagieren wir mit Methoden, sogenannten *Event-Handlern*. `sender` ist das Element, das das Event ausgelöst hat (hier der Button), `e` enthält Zusatzinfos zum Event.

#### 4.3 – Eingaben aus dem Formular auslesen, parsen und validieren
Im Click-Handler holen wir die Werte aus den UI-Elementen und wandeln sie in die richtigen Typen um.

**Was muss umgewandelt werden?**

| UI-Element | Liefert | Brauchen wir als |
|---|---|---|
| `AmountTextBox.Text` | `string` | `decimal` |
| `CategoryTextBox.Text` | `string` | `string` |
| `DescriptionTextBox.Text` | `string` | `string` |
| `DatePicker.SelectedDate` | `DateTime?` (nullable) | `DateTime` |
| `TypeComboBox.SelectedItem` | `object` (ein `ComboBoxItem`) | `TransactionType` (Enum) |

**Die kniffligen Stellen:**

1. **`decimal.TryParse`** – Der Nutzer könnte `"abc"` eingeben. `TryParse` versucht die Umwandlung, gibt `true`/`false` zurück und schreibt das Ergebnis über das `out`-Schlüsselwort in unsere Variable. Klappt es nicht, zeigen wir eine Fehlermeldung und brechen mit `return;` ab.

2. **`?? DateTime.Today`** – Der `??`-Operator (Null-Coalescing) bedeutet: *„Wenn der Wert links `null` ist, nimm den rechten."* `DatePicker.SelectedDate` kann `null` sein, wenn der Nutzer kein Datum gewählt hat – dann nehmen wir einfach das heutige Datum.

3. **ComboBox auslesen** – Aus `SelectedItem` kommt ein `ComboBoxItem`-Objekt zurück, kein String und schon gar kein Enum. Wir holen den angezeigten Text (`"Einnahme"` / `"Ausgabe"`) und mappen ihn mit dem ternären Operator (`? :`) auf `TransactionType.Income` bzw. `Expense`.

Am Ende von 4.3 gibt es eine `MessageBox` als Test-Ausgabe – damit man sofort sieht, dass alle Werte korrekt erkannt werden, **bevor** wir sie in 4.4 wirklich zur Liste hinzufügen.

**Lerneffekt von 4.3:**
- Sicheres Umwandeln von Strings in andere Typen (`TryParse` statt `Parse`)
- Umgang mit nullable Typen (`?`, `??`)
- Sichere Casts mit `as` und Null-sichere Aufrufe mit `?.`
- Trennung von „Werte einsammeln" und „Werte verwenden" – das macht den Code übersichtlicher und macht Schritt 4.4 einfacher.

#### 4.4 – Transaction zur Liste hinzufügen + Felder leeren *(noch offen)*
#### 4.5 – ListView mit der Liste verbinden *(noch offen)*

## Status

Work in Progress – wird laufend erweitert, während ich neue Konzepte lerne.
