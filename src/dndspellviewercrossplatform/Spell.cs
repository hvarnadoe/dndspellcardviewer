using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using VectSharp;
using VectSharp.PDF;

namespace dndspellviewercrossplatform
{
    public class Spell
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("casting_time")]
        public string? CastingTime { get; set; }
        [JsonPropertyName("range")]
        public string? Range { get; set; }
        [JsonPropertyName("components")]
        public string[]? Components { get; set; }
        [JsonPropertyName("duration")]
        public string? Duration { get; set; }
        [JsonPropertyName("concentration")]
        public bool? Concentration { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("class")]
        public string[]? Class { get; set; }

        private const int spacer = 10;
        public static void CreatePDF(List<Spell> spells, Stream stream)
        {
            Colour fillColour = Colours.Black;
            Document doc = new Document();

            int spellIndex = spells.Count - 1;

            while (spellIndex >= 0)
            {
                double width = 850;
                double height = 1100;
                Page page = new Page(width, height);
                Graphics graphics = page.Graphics;

                double rectangleX = 43;
                double rectangleY = 21;
                double rectangleWidth = 248;
                double rectangleHeight = 346;

                bool drewAnySpell = false;

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (spellIndex < 0)
                            break;

                        Console.WriteLine($"Drawing spell number {spellIndex}");
                        DrawSpell(graphics, rectangleX, rectangleY, rectangleWidth, rectangleHeight, spells[spellIndex]);
                        drewAnySpell = true;
                        spellIndex--;

                        rectangleX += rectangleWidth + spacer;
                    }
                    rectangleX = 43;
                    rectangleY += rectangleHeight + spacer;

                    if (spellIndex < 0)
                        break;
                }

                if (drewAnySpell)
                {
                    doc.Pages.Add(page);
                }
            }

            doc.SaveAsPDF(stream);
        }

        static void DrawSpell(Graphics graphics, double startX, double startY, double cardWidth, double cardHeight, Spell spell)
        {
            // Card Border
            graphics.StrokeRectangle(startX, startY, cardWidth, cardHeight, Colours.Black, lineWidth: 10);

            #region Title Underline
            GraphicsPath path = new GraphicsPath();
            Point p1 = new Point(startX + 3 * spacer, startY + 3 * spacer);
            Point p2 = new Point(startX + cardWidth - 2 * spacer, startY + 3 * spacer);
            path.MoveTo(p1).LineTo(p2);
            graphics.StrokePath(path, Colours.Black, lineWidth: spacer / 5);
            #endregion

            #region Spell Name
            Font font = new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.HelveticaOblique), 12);
            var text = spell.Name ?? "";
            graphics.FillText(p1.X, p1.Y - 1.5 * spacer, text, font, Colours.Black);
            #endregion

            #region Spell Level
            font = new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.TimesBoldItalic), 12);
            text = GetLevelNumeral(spell.Level);
            graphics.StrokeText(p2.X - 3.5 * spacer, p1.Y - 1.5 * spacer, text, font, Colours.Black);
            graphics.FillText(p2.X - 3.5 * spacer, p1.Y - 1.5 * spacer, text, font, Colours.Black);
            #endregion

            #region Spell Info
            Font titleFont = new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.HelveticaBold), 6);
            text = "CASTING TIME";
            graphics.FillText(p1.X, p1.Y + spacer, text, titleFont, Colours.Black);
            text = "RANGE";
            graphics.FillText(p1.X + 0.4 * cardWidth, p1.Y + spacer, text, titleFont, Colours.Black);
            text = "COMPONENTS";
            graphics.FillText(p1.X, p1.Y + 3 * spacer, text, titleFont, Colours.Black);
            text = "DURATION";
            graphics.FillText(p1.X + 0.4 * cardWidth, p1.Y + 3 * spacer, text, titleFont, Colours.Black);

            Font descFont = new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 6);
            text = spell?.CastingTime ?? "";
            graphics.FillText(p1.X, p1.Y + 2 * spacer, text, titleFont, Colours.Black);
            text = spell?.Range ?? "";
            graphics.FillText(p1.X + 0.4 * cardWidth, p1.Y + 2 * spacer, text, titleFont, Colours.Black);
            string material;
            (text, material) = GetSpellComponets(spell);
            graphics.FillText(p1.X, p1.Y + 4 * spacer, text, titleFont, Colours.Black);
            Point bodyStart;
            if (descFont.MeasureText(material).Width > cardWidth - 6 * spacer)
            {
                material.Insert(material.Length / 2, "\n");
                bodyStart = new Point(p1.X, startY + 12 * spacer);
            }
            else if (material != string.Empty)
                bodyStart = new Point(p1.X, startY + 11 * spacer);
            else
                bodyStart = new Point(p1.X, startY + 10 * spacer);
            graphics.FillText(p1.X, p1.Y + 6 * spacer, material, titleFont, Colours.Black);
            text = spell?.Duration ?? "";
            graphics.FillText(p1.X + 0.4 * cardWidth, p1.Y + 4 * spacer, text, titleFont, Colours.Black);
            if (spell?.Concentration ?? false)
            {
                text = "  - Concentration";
                graphics.FillText(p1.X + 0.4 * cardWidth + descFont.MeasureText(spell?.Duration ?? "").Width, p1.Y + 4 * spacer, text, titleFont, Colours.Black);
            }
            path.MoveTo(p1.X, p1.Y + 5 * spacer).LineTo(p2.X, p2.Y + 5 * spacer);
            graphics.StrokePath(path, Colours.Black, lineWidth: spacer / 5);
            #endregion

            #region Body
            var lines = FormatDescription(spell, descFont, cardWidth - 6 * spacer);
            for (int i = 0; i < lines.Length; i++)
                graphics.FillText(bodyStart.X, bodyStart.Y + i * spacer, lines[i], titleFont, Colours.Black);
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
            return lines.ToArray();
        }
    }
}
