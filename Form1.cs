using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace dndspellcardgenerator
{
    public partial class Form1 : Form
    {
        List<Spell> spells;
        int currentIndex = 0;
        public Form1()
        {
            InitializeComponent();
            string spellsJson = File.ReadAllText("spells.json");
            var spellTemp = JsonSerializer.Deserialize<List<Spell>>(spellsJson);
            if (spellTemp != null)
            {
                spells = spellTemp;
            }
            else
            {
                spells = new List<Spell>();
            }
            UpdateDisplayCard(spells[0]);
            HashSet<string> list = new HashSet<string>();
            list.Add("None");
            foreach (var spell in spells)
            {
                foreach (var c in spell?.Class ?? [])
                {
                    list.Add(c);
                }
                spellList.Items.Add(spell?.Name ?? "");
            }
            list.ToList().ForEach(w => classSelect.Items.Add(w));
        }

        void UpdateDisplayCard(Spell spell)
        {
            Control name = default_card.Controls.Find("spell_name", true)[0];
            Control castingTime = default_card.Controls.Find("casting_time", true)[0];
            Control range = default_card.Controls.Find("range", true)[0];
            Control components = default_card.Controls.Find("components", true)[0];
            Control duration = default_card.Controls.Find("duration", true)[0];
            Control description = default_card.Controls.Find("text_body", true)[0];
            Control typeAndLevel = default_card.Controls.Find("type_and_level", true)[0];
            name.Text = spell?.Name?.ToUpper();
            castingTime.Text = spell?.CastingTime;
            range.Text = spell?.Range;
            string? materialComp = null;
            components.Text = "";
            for (int i = 0; i < spell?.Components?.Length; i++)
            {
                components.Text += spell.Components[i][0];
                if (i < spell?.Components.Length - 1)
                    components.Text += ", ";
                if (spell?.Components[i].Length > 2)
                {
                    materialComp = spell.Components[i].Substring(2);
                }
            }
            if (spell?.Duration != null && spell.Duration == "0")
            {
                duration.Text = "Instantanious";
            }
            else
            {
                duration.Text = spell?.Duration;
            }
            description.Text = "";
            if (materialComp != null)
            {
                description.Text = materialComp.ToString() + "\n";
            }
            description.Text += spell?.Description;
            if (spell?.Level == 0)
            {
                typeAndLevel.Text = spell.Type + " Cantrip";
            }
            else
            {
                string prefix = "";
                switch (spell?.Level)
                {
                    case 1:
                        prefix = "1st ";
                        break;
                    case 2:
                        prefix = "2nd ";
                        break;
                    case 3:
                        prefix = "3rd ";
                        break;
                    default:
                        prefix = $"{spell?.Level}th ";
                        break;
                }
                typeAndLevel.Text = prefix + " level " + spell?.Type;
            }
            foreach (Control control in default_card.Controls)
            {
                GrowFontToFit(control);
                ShrinkFontToFit(control);
            }
        }
        void ShrinkFontToFit(Control control)
        {
            float minSize = 1f;
            float size = control.Font.Size;
            var bounds = control.ClientSize;

            var flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;

            while (size > minSize)
            {
                var testFont = new Font(control.Font.FontFamily, size, control.Font.Style);

                var textSize = TextRenderer.MeasureText(
                    control.Text,
                    testFont,
                    bounds,
                    flags
                );

                if (textSize.Width <= bounds.Width &&
                    textSize.Height <= bounds.Height)
                {
                    control.Font = testFont;
                    return;
                }

                testFont.Dispose();
                size -= 0.25f;
            }
        }

        void GrowFontToFit(Control control)
        {
            float maxSize = 9f;     // pick a sane upper bound
            float step = 0.25f;

            float size = control.Font.Size;
            var bounds = control.ClientSize;

            var flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;

            Font? lastGoodFont = null;

            while (size <= maxSize)
            {
                var testFont = new Font(control.Font.FontFamily, size, control.Font.Style);

                var textSize = TextRenderer.MeasureText(
                    control.Text,
                    testFont,
                    bounds,
                    flags
                );

                if (textSize.Width <= bounds.Width &&
                    textSize.Height <= bounds.Height)
                {
                    lastGoodFont?.Dispose();
                    lastGoodFont = testFont;
                    size += step;
                }
                else
                {
                    testFont.Dispose();
                    break;
                }
            }

            if (lastGoodFont != null)
                control.Font = lastGoodFont;
        }

        private void classSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = (ComboBox)sender;
            var temp = name.AccessibilityObject.Value;
            filterSpells(temp ?? "");
        }

        private void filterSpells(string className)
        {
            spellList.Items.Clear();
            for (int i = 0; i < checkedSpells.Count; i++)
            {
                spellList.Items.Add(checkedSpells.ElementAt(i));
                spellList.SetItemChecked(i, true);
            }
            if (className == string.Empty || className == "None")
            {
                spells.Where(w =>
                    w.Name != null && !checkedSpells.Contains(w.Name))
                    .ToList().ForEach(spell => spellList.Items.Add(spell?.Name ?? ""));
                return;
            }

            spells.Where(w =>
                w.Class != null && w.Class.Contains(className)
                && w.Name != null && !checkedSpells.Contains(w.Name))
                .ToList().ForEach(spell => spellList.Items.Add(spell?.Name ?? ""));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            filterSpells(classSelect.Text);
            var toRemove = new List<string>();
            for (int i = 0; i < spellList.Items.Count; i++)
            {
                var spell = spellList.Items[i].ToString();
                if (spell == null)
                    continue;
                if (spell.ToLower().StartsWith(textBox1.Text.ToLower()))
                    continue;
                toRemove.Add(spell);
            }
            toRemove.Where(w => !checkedSpells.Contains(w)).ToList().ForEach(spell => spellList.Items.Remove(spell));
        }

        private void spellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var l = (CheckedListBox)sender;
            Spell spell = spells.Where(s => s.Name == l?.SelectedItem?.ToString()).ToList()[0];
            UpdateDisplayCard(spell);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            spellList.Size = new Size(spellList.Width, (int)(this.Height * .8));
        }

        HashSet<string> checkedSpells = [];
        private void spellList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var spell = spellList.Items[e.Index].ToString();
            if (spell == null)
                return;
            if (e.CurrentValue == CheckState.Checked)
            {
                checkedSpells.Remove(spell);
                return;
            }

            checkedSpells.Add(spell);
        }

        private void spellList_MouseDown(object sender, MouseEventArgs e)
        {
            int index = spellList.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches) return;

            // Get the rectangle for the item
            Rectangle itemRect = spellList.GetItemRectangle(index);

            // Checkbox area is on the far left (rough estimate)
            // This width usually works well for Windows checkbox hitbox
            Rectangle checkboxRect = new Rectangle(itemRect.X, itemRect.Y, 18, itemRect.Height);

            if (checkboxRect.Contains(e.Location))
            {
                // Toggle check state immediately if checkbox area clicked
                bool currentlyChecked = spellList.GetItemChecked(index);
                spellList.SetItemChecked(index, !currentlyChecked);
            }
        }
        private Bitmap RenderControlToBitmap(Control ctrl)
        {
            Bitmap bmp = new Bitmap(ctrl.Width, ctrl.Height);
            ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, ctrl.Width, ctrl.Height));
            return bmp;
        }
        private void SaveBitmapsToPdf(List<Bitmap> images, string pdfPath)
        {
            PdfDocument doc = new PdfDocument();

            int perPage = 9;
            int cols = 3;
            int rows = 3;

            for (int i = 0; i < images.Count; i += perPage)
            {
                PdfPage page = doc.AddPage();
                page.Size = PdfSharp.PageSize.Letter; // or A4

                using XGraphics gfx = XGraphics.FromPdfPage(page);

                double pageW = page.Width;
                double pageH = page.Height;

                double margin = 20;
                double cellW = (pageW - margin * 2) / cols;
                double cellH = (pageH - margin * 2) / rows;

                for (int j = 0; j < perPage; j++)
                {
                    int imgIndex = i + j;
                    if (imgIndex >= images.Count) break;

                    int r = j / cols;
                    int c = j % cols;

                    double x = margin + c * cellW;
                    double y = margin + r * cellH;

                    // Convert Bitmap -> XImage
                    using MemoryStream ms = new MemoryStream();
                    images[imgIndex].Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    using XImage ximg = XImage.FromStream(ms);

                    // Fit inside cell while preserving aspect ratio
                    double scale = Math.Min(cellW / ximg.PixelWidth, cellH / ximg.PixelHeight);
                    double drawW = ximg.PixelWidth * scale;
                    double drawH = ximg.PixelHeight * scale;

                    double centerX = x + (cellW - drawW) / 2;
                    double centerY = y + (cellH - drawH) / 2;

                    gfx.DrawImage(ximg, centerX, centerY, drawW, drawH);
                }
            }

            doc.Save(pdfPath);
        }
        private void ExportCardsPdf()
        {
            List<Bitmap> captures = new List<Bitmap>();

            foreach (string spell in checkedSpells)
            {
                Spell current = spells.Where(s => s.Name == spell).ToList()[0];
                UpdateDisplayCard(current);
                default_card.Refresh();
                Application.DoEvents();
                captures.Add(RenderControlToBitmap(default_card));
            }

            using SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";
            sfd.FileName = "cards.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveBitmapsToPdf(captures, sfd.FileName);
            }

            // cleanup
            foreach (var bmp in captures)
                bmp.Dispose();
        }

        private void print_Click(object sender, EventArgs e)
        {
            ExportCardsPdf();
        }
    }
    public class Spell
    {
        [JsonPropertyName("name")]
        public string? Name {  get; set; }
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
    }
}
