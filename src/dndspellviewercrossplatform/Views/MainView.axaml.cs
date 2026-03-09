using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using VectSharp;
using VectSharp.Canvas;
using Point = VectSharp.Point;

namespace dndspellviewercrossplatform.Views;

public partial class MainView : UserControl
{
    Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
    Spell selectedSpell = new Spell();
    Grid mainGrid;
    double woffset;
    double hoffset;
    double offset;

    double wscaler;
    double hscaler;
    double scaler;
    double cardWidth;
    double cardHeight;
    double scaledWidth;
    double scaledHeight;

    string levelFilterName = "levelfilter";
    string classFilterName = "classfilter";

    public MainView()
    {
        InitializeComponent();
        try
        {
            var vb = this.FindControl<Grid>("MainGrid");
            if (vb != null)
            {
                mainGrid = vb;
            }
            else
            {
                throw new Exception("Unable to load viewbox");
            }
        }
        catch
        {
            Console.WriteLine("Unable to load");
            Environment.Exit(1);
        }
        var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        var uri = new Uri($"avares://{assemblyName}/Assets/spells.json");
        var file = AssetLoader.Open(uri);
        string spellsJson = new StreamReader(file).ReadToEnd();
        var spellTemp = JsonSerializer.Deserialize<List<Spell>>(spellsJson);
        if (spellTemp != null)
        {
            foreach (var spell in spellTemp)
            {
                if (spell.Name != null && !spells.ContainsKey(spell.Name))
                    spells.Add(spell.Name, spell);
            }
        }
        else
        {
            Console.WriteLine("Unable to load spells, exiting");
            Environment.Exit(1);
        }
        selectedSpell = spells.First().Value;
        this.Loaded += (_, __) => Init();
        this.Background = Brushes.DarkSlateGray;
    }

    void Init()
    {
        #region Variable Setup
        woffset = Bounds.Width * 0.01;
        hoffset = Bounds.Height * 0.05;
        offset = Math.Abs(Math.Min(woffset, hoffset));

        wscaler = 0.2 * (Bounds.Width - 100) / 248;
        hscaler = 0.8 * (Bounds.Height - 100) / 346;
        scaler = Math.Abs(Math.Min(wscaler, hscaler));
        cardWidth = 248;
        cardHeight = 346;
        scaledWidth = cardWidth * scaler;
        scaledHeight = cardHeight * scaler;
        #endregion

        (_, _, _, Document doc, _) = UpdateSpell();
        var vect = doc.Pages.Last().PaintToCanvas();

        var sv = CreateSpellList();

        var printButton = new Button();
        printButton.Content = "Save Selected as PDF";
        printButton.Click += PrintButton_Click;

        var classFilter = CreateFilter("Filter by Class", classFilterName, ["Bard", "Cleric", "Druid", "Paladin", "Ranger", "Sorcerer", "Warlock", "Wizard", "All"]);
        var levelFilter = CreateFilter("Filter by Level", levelFilterName, ["Cantrip", "1", "2", "3", "4", "5", "6", "7", "8", "9", "All"]);
        var filterPanel = new StackPanel();
        filterPanel.Orientation = Orientation.Vertical;

        filterPanel.Children.Add(classFilter);
        filterPanel.Children.Add(levelFilter);

        var searchInput = new StackPanel();
        var label = new Label();
        label.Content = "Search";
        var textBox = new TextBox();
        textBox.Width = 200;
        textBox.KeyUp += SearchBox_KeyUp;
        searchInput.Children.Add(label);
        searchInput.Children.Add(textBox);

        #region Grid Layout
        Grid.SetColumn(vect, 0);
        Grid.SetRow(vect, 0);
        Grid.SetColumn(printButton, 0);
        Grid.SetRow(printButton, 1);

        Grid.SetColumn(sv, 1);

        Grid.SetColumn(filterPanel, 2);

        Grid.SetColumn(searchInput, 3);

        mainGrid.Children.Add(vect);
        mainGrid.Children.Add(printButton);
        mainGrid.Children.Add(sv);
        mainGrid.Children.Add(filterPanel);
        mainGrid.Children.Add(searchInput);
        #endregion

    }

    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        ApplyFilters();
    }

    ScrollViewer CreateSpellList()
    {
        ScrollViewer sv = new ScrollViewer();
        sv.Background = Brushes.DimGray;
        sv.Height = 0.9 * (Bounds.Height - 100);
        sv.Margin = new Thickness(25 * scaler);
        StackPanel sp = new StackPanel();
        sv.Content = sp;
        sp.Spacing = 1 * scaler;
        foreach (Spell spell in spells.Values)
        {
            sp.Children.Add(CreateSpellEntry(spell, sv.Width));
        }

        return sv;
    }

    CheckBox CreateSpellEntry(Spell spell, double width)
    {
        var checkBox = new CheckBox();

        checkBox.Width = width * .8;
        checkBox.Height = 15 * hscaler;
        checkBox.FontSize = 15 * scaler;
        checkBox.Foreground = Brushes.Black;
        checkBox.BorderBrush = Brushes.Black;
        checkBox.Content = spell.Name ?? "missing name";

        checkBox.PointerEntered += CheckBox_PointerEntered;
        checkBox.PointerExited += CheckBox_PointerExited;

        return checkBox;
    }

    StackPanel CreateFilter(string title, string cbName, string[] options)
    {
        StackPanel sp = new StackPanel();
        var name = new Label();
        name.Content = title;


        var cb = new ComboBox();
        cb.Name = cbName;

        foreach (var c in options)
        {
            var cbi = new ComboBoxItem();
            cbi.Content = c;
            cb.Items.Add(cbi);
        }
        cb.SelectionChanged += Filter_SelectionChanged;

        sp.Children.Add(name);
        sp.Children.Add(cb);

        sp.Height = name.Height + cb.Height;
        return sp;
    }

    private void Filter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }
    void ApplyFilters()
    {
        string selectedLevel = string.Empty;
        string selectedClass = "All";
        int level = -1;

        var filterPanel = mainGrid.Children[3] as StackPanel;

        var levelSp = filterPanel?.Children[1] as StackPanel;
        var levelSelector = levelSp?.Children.Last() as ComboBox;
        var levelItem = levelSelector?.SelectedItem as ComboBoxItem;
        if (levelSelector != null)
            selectedLevel = levelItem?.Content?.ToString() ?? "All";

        if (selectedLevel == "Cantrip")
            level = 0;
        else if (selectedLevel != string.Empty && selectedLevel != "All")
            level = Int32.Parse(selectedLevel);


        var classSp = filterPanel?.Children[0] as StackPanel;
        var classSelector = classSp?.Children.Last() as ComboBox;
        var classItem = classSelector?.SelectedItem as ComboBoxItem;
        if (classSelector != null)
            selectedClass = classItem?.Content?.ToString() ?? "All";

        var sv = mainGrid.Children[2] as ScrollViewer;
        if (sv == null) return;
        sv.MinWidth = sv.Bounds.Width;
        var sp = sv.Content as StackPanel;
        if (sp == null) return;

        var checkedSpells = GetCheckedSpells(sp);

        sp.Children.Clear();

        var searchContainer = mainGrid.Children[4] as StackPanel;
        var searchBox = searchContainer?.Children.Last() as TextBox;
        string? searchQuery = searchBox?.Text;

        foreach (var spell in spells)
        {
            if (!checkedSpells.Contains(spell.Key))
            {
                if (level != -1 && spell.Value.Level != level) continue;
                if (selectedClass != "All" && (!spell.Value.Class?.Contains(selectedClass) ?? true)) continue;
                if(!string.IsNullOrEmpty(searchQuery) && !spell.Key.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase)) continue;
            }

            var cb = CreateSpellEntry(spell.Value, sv.Width);
            if (checkedSpells.Contains(spell.Key))
                cb.IsChecked = true;

            sp.Children.Add(cb);
        }
    }

    private List<string> GetCheckedSpells(StackPanel sp)
    {
        var checkedSpells = new List<string>();
        foreach (CheckBox? c in sp.Children)
        {
            string name;
            if (GetNameFromCheckBox(c, out name))
                checkedSpells.Add(name);
        }
        return checkedSpells;
    }

    bool GetNameFromCheckBox(CheckBox? checkBox, out string name)
    {
        name = "";
        if(checkBox == null) return false;

        if(checkBox.IsChecked == false) return false;

        if(checkBox.Content == null) return false;

        var tryget = checkBox.Content.ToString();

        if(tryget == null) return false;

        name = tryget;
        return true;
    }

    private async void PrintButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ScrollViewer? sv = null;
        foreach (var control in mainGrid.Children)
        {
            sv = control as ScrollViewer;
            if (sv != null) break;
        }
        if (sv == null) return;

        StackPanel? sp = sv.Content as StackPanel;
        if (sp == null) return;

        Controls spellList = sp.Children;
        if (spellList.Count == 0) return;

        var selectedSpellNames = GetCheckedSpells(sp);
        if (selectedSpellNames.Count == 0)
        {
            DisplayErrorPopup("Please select spells");
            return;
        }
        var selectedSpells = new List<Spell>();
        foreach (var spellName in selectedSpellNames)
        {
            var temp = spells.Where(s => s.Key.Equals(spellName, StringComparison.OrdinalIgnoreCase));
            if (!temp.Any())
                continue;
            var spell = temp.First();
            selectedSpells.Add(spell.Value);
        }

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions { Title = "Select a Location", SuggestedFileName = "spelllist.pdf" });

        if (file is not null)
        {
            using var ms = new MemoryStream();

            Spell.CreatePDF(selectedSpells, ms);

            ms.Position = 0;
            await using var fileStream = await file.OpenWriteAsync();
            await ms.CopyToAsync(fileStream);
        }
    }

    void DisplayErrorPopup(string errorMsg)
    {
        var window = new Window();
        if (window is null) return;

        var sp = new StackPanel();
        var label = new Label();
        label.Content = errorMsg + "\nPress enter to close";
        label.Foreground = Brushes.Black;

        sp.Children.Add(label);
        sp.HorizontalAlignment = HorizontalAlignment.Center;
        sp.VerticalAlignment = VerticalAlignment.Center;


        window.Content = sp;
        window.Background = Brushes.Coral;
        window.Width = 200;
        window.Height = 200;
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.KeyUp += ErrorWindow_KeyUp;

        window.Activate();

        window.Show();
    }

    private void ErrorWindow_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        var window = sender as Window;
        if (window is null) return;

        if (e.Key == Avalonia.Input.Key.Enter)
        {
            window.Close();
        }
    }

    private void CheckBox_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        CheckBox? checkBox = sender as CheckBox;
        if (checkBox == null)
            return;

        checkBox.Foreground = Brushes.Black;
    }

    private void CheckBox_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        CheckBox? checkBox = sender as CheckBox;
        if (checkBox == null)
            return;

        checkBox.Foreground = Brushes.DeepSkyBlue;

        var spellName = checkBox?.Content?.ToString();
        if (spellName == null) return;
        Spell? selected;
        if (spells.TryGetValue(spellName, out selected))
            selectedSpell = selected;

        UpdateDisplay();
    }

    (double, double, double, Document, bool) UpdateSpell()
    {


        var doc = new Document();
        doc.Pages.Add(new Page(scaledWidth, scaledHeight));

        var gpr = doc.Pages.Last().Graphics;


        GenerateSpell(gpr, 0, 0, cardWidth, cardHeight, scaler, selectedSpell);
        gpr.Save();
        gpr.Restore();

        return (wscaler, hscaler, scaler, doc, true);
    }
    void UpdateDisplay()
    {
        if (mainGrid.Children.Count <= 0) return;

        double wscaler, hscaler, scaler;
        (wscaler, hscaler, scaler, Document doc, bool success) = UpdateSpell();
        if (!success) return;

        var vect = doc.Pages.Last().PaintToCanvas();

        mainGrid.Children[0] = vect;
        Grid.SetColumn(vect, 0);
    }

    int spacer = 10;
    void GenerateSpell(Graphics graphics, double startX, double startY, double cardWidth, double cardHeight, double scaleMultiplyer, Spell spell)
    {
        double scaledWidth = cardWidth * scaleMultiplyer;
        double scaledHeight = cardHeight * scaleMultiplyer;
        double s = spacer * scaleMultiplyer;

        // Card Border
        graphics.FillRectangle(startX, startY, scaledWidth, scaledHeight, Colours.White);
        graphics.StrokeRectangle(startX, startY, scaledWidth, scaledHeight, Colours.Black, lineWidth: 10 * scaleMultiplyer);

        #region Title Underline
        GraphicsPath path = new GraphicsPath();
        Point p1 = new Point(startX + 3 * s, startY + 3 * s);
        Point p2 = new Point(startX + scaledWidth - 2 * s, startY + 3 * s);
        path.MoveTo(p1).LineTo(p2);
        graphics.StrokePath(path, Colours.Black, lineWidth: (s / 5));
        #endregion

        #region Spell Name
        Font font = new Font(VectSharp.FontFamily.ResolveFontFamily(VectSharp.FontFamily.StandardFontFamilies.HelveticaOblique), 12 * scaleMultiplyer);
        var text = spell.Name ?? "";
        graphics.FillText(p1.X, p1.Y - 1.5 * s, text, font, Colours.Black);
        #endregion

        #region Spell Level
        font = new Font(VectSharp.FontFamily.ResolveFontFamily(VectSharp.FontFamily.StandardFontFamilies.TimesBoldItalic), 12 * scaleMultiplyer);

        text = GetLevelNumeral(spell.Level);
        graphics.StrokeText(p2.X - 3.5 * s, p1.Y - 1.5 * s, text, font, Colours.Black);
        graphics.FillText(p2.X - 3.5 * s, p1.Y - 1.5 * s, text, font, Colours.Black);
        #endregion

        #region Spell Info
        Font titleFont = new Font(VectSharp.FontFamily.ResolveFontFamily(VectSharp.FontFamily.StandardFontFamilies.HelveticaBold), 6 * scaleMultiplyer);

        text = "CASTING TIME";
        graphics.FillText(p1.X, p1.Y + s, text, titleFont, Colours.Black);

        text = "RANGE";
        graphics.FillText(p1.X + 0.4 * scaledWidth, p1.Y + s, text, titleFont, Colours.Black);

        text = "COMPONENTS";
        graphics.FillText(p1.X, p1.Y + 3 * s, text, titleFont, Colours.Black);

        text = "DURATION";
        graphics.FillText(p1.X + 0.4 * scaledWidth, p1.Y + 3 * s, text, titleFont, Colours.Black);

        Font descFont = new Font(
            VectSharp.FontFamily.ResolveFontFamily(VectSharp.FontFamily.StandardFontFamilies.Helvetica),
            6 * scaleMultiplyer);

        text = spell?.CastingTime ?? "";
        graphics.FillText(p1.X, p1.Y + 2 * s, text, descFont, Colours.Black);

        text = spell?.Range ?? "";
        graphics.FillText(p1.X + 0.4 * scaledWidth, p1.Y + 2 * s, text, descFont, Colours.Black);

        string material;
        (text, material) = GetSpellComponets(spell);

        graphics.FillText(p1.X, p1.Y + 4 * s, text, descFont, Colours.Black);

        Point bodyStart;
        if (descFont.MeasureText(material).Width > scaledWidth - 6 * s)
        {
            material = material.Insert(material.Length / 2, "\n");
            bodyStart = new Point(p1.X, startY + 12 * s);
        }
        else if (!string.IsNullOrEmpty(material))
            bodyStart = new Point(p1.X, startY + 11 * s);
        else
            bodyStart = new Point(p1.X, startY + 10 * s);

        graphics.FillText(p1.X, p1.Y + 6 * s, material, descFont, Colours.Black);

        text = spell?.Duration ?? "";
        graphics.FillText(p1.X + 0.4 * scaledWidth, p1.Y + 4 * s, text, descFont, Colours.Black);

        if (spell?.Concentration ?? false)
        {
            text = "  - Concentration";
            graphics.FillText(
                p1.X + 0.4 * scaledWidth + descFont.MeasureText(spell?.Duration ?? "").Width,
                p1.Y + 4 * s,
                text,
                descFont,
                Colours.Black);
        }

        path = new GraphicsPath();
        path.MoveTo(p1.X, p1.Y + 5 * s).LineTo(p2.X, p2.Y + 5 * s);
        graphics.StrokePath(path, Colours.Black, lineWidth: s / 5);
        #endregion

        #region Body
        descFont = new Font(VectSharp.FontFamily.ResolveFontFamily(VectSharp.FontFamily.StandardFontFamilies.Helvetica), 10 * scaleMultiplyer);
        var lines = FormatDescription(spell, descFont, scaledWidth - 6 * s);
        for (int i = 0; i < lines.Length; i++)
        {
            graphics.FillText(bodyStart.X, bodyStart.Y + i * s, lines[i], descFont, Colours.Black);
        }
        #endregion
    }

    static string GetLevelNumeral(int level)
    {
        if (level == 0) return "Cantrip";

        var levelNumeral = "";
        if (level < 4)
        {
            for (int i = 0; i < level; i++) levelNumeral += "I";
            return levelNumeral;
        }

        if (level == 4)
            return "IV";
        if (level == 9)
            return "IX";

        levelNumeral = "V";
        for (int i = 5; i < level; i++)
        {
            levelNumeral += "I";
        }

        return levelNumeral;
    }

    static (string, string) GetSpellComponets(Spell? spell)
    {
        if (spell?.Components == null)
            return ("", string.Empty);
        string components = "";
        string material = string.Empty;
        for (int i = 0; i < spell.Components.Length; i++)
        {
            if (spell.Components[i] == null)
                continue;

            components += spell.Components[i][0];

            if (spell.Components[i].Length > 1)
                material += spell.Components[i].Substring(2);

            if (i != spell.Components.Length - 1)
                components += ", ";
        }

        return (components, material);
    }

    static string[] FormatDescription(Spell? spell, Font font, double lineLen)
    {
        if (spell == null || spell.Description == null)
            return [];
        List<string> lines = new List<string>();
        string line = "";
        for (int i = 0; i < spell.Description.Length; i++)
        {
            line += spell.Description[i];
            if (font.MeasureText(line).Width > lineLen)
            {
                for (int a = line.Length - 1; a > 0; a--)
                {
                    if (line[a] == ' ')
                    {
                        int diff = line.Length - 1 - a;
                        i -= diff;
                        line = line.Substring(0, a);
                        break;
                    }
                }
                lines.Add(line);
                line = "";
            }
        }
        if (!string.IsNullOrWhiteSpace(line))
        {
            lines.Add(line.Trim());
        }
        return lines.ToArray();
    }
}