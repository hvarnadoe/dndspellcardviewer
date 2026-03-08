namespace dndspellcardgenerator
{
    partial class Form1
    {

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            default_card = new GroupBox();
            type_and_level = new TextBox();
            text_body = new RichTextBox();
            pictureBox1 = new PictureBox();
            duration = new Label();
            components = new Label();
            range = new Label();
            casting_time = new Label();
            duration_header = new Label();
            components_header = new Label();
            range_header = new Label();
            casting_time_header = new Label();
            spell_name = new Label();
            spellList = new CheckedListBox();
            classSelect = new ComboBox();
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            print = new Button();
            default_card.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // default_card
            // 
            default_card.BackColor = Color.White;
            default_card.BackgroundImageLayout = ImageLayout.None;
            default_card.Controls.Add(type_and_level);
            default_card.Controls.Add(text_body);
            default_card.Controls.Add(pictureBox1);
            default_card.Controls.Add(duration);
            default_card.Controls.Add(components);
            default_card.Controls.Add(range);
            default_card.Controls.Add(casting_time);
            default_card.Controls.Add(duration_header);
            default_card.Controls.Add(components_header);
            default_card.Controls.Add(range_header);
            default_card.Controls.Add(casting_time_header);
            default_card.Controls.Add(spell_name);
            default_card.Location = new Point(29, 26);
            default_card.Name = "default_card";
            default_card.Size = new Size(252, 352);
            default_card.TabIndex = 3;
            default_card.TabStop = false;
            // 
            // type_and_level
            // 
            type_and_level.BackColor = SystemColors.ActiveCaption;
            type_and_level.BorderStyle = BorderStyle.None;
            type_and_level.Font = new Font("Segoe UI Light", 5.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            type_and_level.Location = new Point(6, 336);
            type_and_level.Name = "type_and_level";
            type_and_level.Size = new Size(240, 10);
            type_and_level.TabIndex = 11;
            type_and_level.Text = "Transmutation Cantrip";
            // 
            // text_body
            // 
            text_body.BackColor = Color.White;
            text_body.BorderStyle = BorderStyle.None;
            text_body.Location = new Point(6, 90);
            text_body.Name = "text_body";
            text_body.Size = new Size(240, 240);
            text_body.TabIndex = 10;
            text_body.Text = "";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveCaption;
            pictureBox1.Location = new Point(6, 79);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(240, 5);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // duration
            // 
            duration.AutoSize = true;
            duration.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            duration.ForeColor = Color.Black;
            duration.Location = new Point(196, 65);
            duration.Name = "duration";
            duration.Size = new Size(50, 11);
            duration.TabIndex = 8;
            duration.Text = "Instantaneous";
            // 
            // components
            // 
            components.AutoSize = true;
            components.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            components.ForeColor = Color.Black;
            components.Location = new Point(126, 65);
            components.Name = "components";
            components.Size = new Size(23, 11);
            components.TabIndex = 7;
            components.Text = "V,S,M";
            // 
            // range
            // 
            range.AutoSize = true;
            range.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            range.ForeColor = Color.Black;
            range.Location = new Point(73, 65);
            range.Name = "range";
            range.Size = new Size(24, 11);
            range.TabIndex = 6;
            range.Text = "Touch";
            // 
            // casting_time
            // 
            casting_time.AutoSize = true;
            casting_time.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            casting_time.ForeColor = Color.Black;
            casting_time.Location = new Point(6, 65);
            casting_time.Name = "casting_time";
            casting_time.Size = new Size(32, 11);
            casting_time.TabIndex = 5;
            casting_time.Text = "1 minute";
            // 
            // duration_header
            // 
            duration_header.AutoSize = true;
            duration_header.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            duration_header.ForeColor = SystemColors.ActiveCaption;
            duration_header.Location = new Point(196, 54);
            duration_header.Name = "duration_header";
            duration_header.Size = new Size(36, 11);
            duration_header.TabIndex = 4;
            duration_header.Text = "Duration:";
            // 
            // components_header
            // 
            components_header.AutoSize = true;
            components_header.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            components_header.ForeColor = SystemColors.ActiveCaption;
            components_header.Location = new Point(126, 54);
            components_header.Name = "components_header";
            components_header.Size = new Size(46, 11);
            components_header.TabIndex = 3;
            components_header.Text = "Components:";
            // 
            // range_header
            // 
            range_header.AutoSize = true;
            range_header.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            range_header.ForeColor = SystemColors.ActiveCaption;
            range_header.Location = new Point(73, 54);
            range_header.Name = "range_header";
            range_header.Size = new Size(27, 11);
            range_header.TabIndex = 2;
            range_header.Text = "Range:";
            // 
            // casting_time_header
            // 
            casting_time_header.AutoSize = true;
            casting_time_header.Font = new Font("Sitka Banner Semibold", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            casting_time_header.ForeColor = SystemColors.ActiveCaption;
            casting_time_header.Location = new Point(6, 54);
            casting_time_header.Name = "casting_time_header";
            casting_time_header.Size = new Size(49, 11);
            casting_time_header.TabIndex = 1;
            casting_time_header.Text = "Casting Time:";
            // 
            // spell_name
            // 
            spell_name.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            spell_name.BackColor = SystemColors.ActiveCaption;
            spell_name.Font = new Font("Microsoft JhengHei", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            spell_name.Location = new Point(6, 19);
            spell_name.Name = "spell_name";
            spell_name.Size = new Size(240, 35);
            spell_name.TabIndex = 0;
            spell_name.Text = "MENDING";
            spell_name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // spellList
            // 
            spellList.FormattingEnabled = true;
            spellList.Location = new Point(300, 26);
            spellList.Name = "spellList";
            spellList.Size = new Size(250, 724);
            spellList.TabIndex = 4;
            spellList.ItemCheck += spellList_ItemCheck;
            spellList.SelectedIndexChanged += spellList_SelectedIndexChanged;
            spellList.MouseDown += spellList_MouseDown;
            // 
            // classSelect
            // 
            classSelect.FormattingEnabled = true;
            classSelect.Location = new Point(619, 45);
            classSelect.Name = "classSelect";
            classSelect.Size = new Size(121, 23);
            classSelect.TabIndex = 5;
            classSelect.SelectedIndexChanged += classSelect_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(619, 27);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 6;
            label1.Text = "Filter by class";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(620, 113);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(120, 23);
            textBox1.TabIndex = 7;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(620, 91);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 8;
            label2.Text = "Search";
            // 
            // print
            // 
            print.Location = new Point(642, 142);
            print.Name = "print";
            print.Size = new Size(75, 23);
            print.TabIndex = 9;
            print.Text = "Print";
            print.UseVisualStyleBackColor = true;
            print.Click += print_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1412, 779);
            Controls.Add(print);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(classSelect);
            Controls.Add(spellList);
            Controls.Add(default_card);
            Name = "Form1";
            Text = "Form1";
            ResizeEnd += Form1_ResizeEnd;
            default_card.ResumeLayout(false);
            default_card.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox default_card;
        private Label casting_time_header;
        private Label spell_name;
        private Label range_header;
        private Label components_header;
        private Label duration_header;
        private Label duration;
        private Label range;
        private Label casting_time;
        private Label components;
        private PictureBox pictureBox1;
        private RichTextBox text_body;
        private TextBox type_and_level;
        private CheckedListBox spellList;
        private ComboBox classSelect;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private Button print;
    }
}
