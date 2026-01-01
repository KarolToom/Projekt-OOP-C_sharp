namespace Ewidencja_Pracownikow
{
    partial class FormDodaj
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtImie = new TextBox();
            txtNazwisko = new TextBox();
            txtPESEL = new TextBox();
            txtPensja = new TextBox();
            cbTypPracownika = new ComboBox();
            btnZapisz = new Button();
            lblParametr1 = new Label();
            lblParametr2 = new Label();
            txtParametr1 = new TextBox();
            txtParametr2 = new TextBox();
            SuspendLayout();
            // 
            // txtImie
            // 
            txtImie.Location = new Point(85, 101);
            txtImie.Name = "txtImie";
            txtImie.Size = new Size(223, 23);
            txtImie.TabIndex = 0;
            txtImie.Text = "Imie";
            // 
            // txtNazwisko
            // 
            txtNazwisko.Location = new Point(85, 139);
            txtNazwisko.Name = "txtNazwisko";
            txtNazwisko.Size = new Size(223, 23);
            txtNazwisko.TabIndex = 1;
            txtNazwisko.Text = "Nazwisko";
            // 
            // txtPESEL
            // 
            txtPESEL.Location = new Point(85, 183);
            txtPESEL.Name = "txtPESEL";
            txtPESEL.Size = new Size(223, 23);
            txtPESEL.TabIndex = 2;
            txtPESEL.Text = "PESEL";
            txtPESEL.TextChanged += txtPESEL_TextChanged;
            // 
            // txtPensja
            // 
            txtPensja.Location = new Point(85, 225);
            txtPensja.Name = "txtPensja";
            txtPensja.Size = new Size(223, 23);
            txtPensja.TabIndex = 3;
            txtPensja.Text = "Pensja";
            // 
            // cbTypPracownika
            // 
            cbTypPracownika.FormattingEnabled = true;
            cbTypPracownika.Items.AddRange(new object[] { "Kierowca", "Handlowiec", "Pracownik biurowy", "Manager" });
            cbTypPracownika.Location = new Point(85, 51);
            cbTypPracownika.Name = "cbTypPracownika";
            cbTypPracownika.Size = new Size(512, 23);
            cbTypPracownika.TabIndex = 4;
            cbTypPracownika.SelectedIndexChanged += cbTypPracownika_SelectedIndexChanged;
            // 
            // btnZapisz
            // 
            btnZapisz.Location = new Point(630, 51);
            btnZapisz.Name = "btnZapisz";
            btnZapisz.Size = new Size(133, 23);
            btnZapisz.TabIndex = 5;
            btnZapisz.Text = "Zapisz";
            btnZapisz.UseVisualStyleBackColor = true;
            btnZapisz.Click += btnZapisz_Click;
            // 
            // lblParametr1
            // 
            lblParametr1.AutoSize = true;
            lblParametr1.Location = new Point(398, 104);
            lblParametr1.Name = "lblParametr1";
            lblParametr1.Size = new Size(61, 15);
            lblParametr1.TabIndex = 6;
            lblParametr1.Text = "Parametr1";
            lblParametr1.Click += label1_Click;
            // 
            // lblParametr2
            // 
            lblParametr2.AutoSize = true;
            lblParametr2.Location = new Point(398, 142);
            lblParametr2.Name = "lblParametr2";
            lblParametr2.Size = new Size(61, 15);
            lblParametr2.TabIndex = 7;
            lblParametr2.Text = "Parametr2";
            // 
            // txtParametr1
            // 
            txtParametr1.Location = new Point(516, 101);
            txtParametr1.Name = "txtParametr1";
            txtParametr1.Size = new Size(100, 23);
            txtParametr1.TabIndex = 8;
            txtParametr1.Visible = false;
            // 
            // txtParametr2
            // 
            txtParametr2.Location = new Point(516, 139);
            txtParametr2.Name = "txtParametr2";
            txtParametr2.Size = new Size(100, 23);
            txtParametr2.TabIndex = 9;
            txtParametr2.Visible = false;
            txtParametr2.TextChanged += txtParametr2_TextChanged;
            // 
            // FormDodaj
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtParametr2);
            Controls.Add(txtParametr1);
            Controls.Add(lblParametr2);
            Controls.Add(lblParametr1);
            Controls.Add(btnZapisz);
            Controls.Add(cbTypPracownika);
            Controls.Add(txtPensja);
            Controls.Add(txtPESEL);
            Controls.Add(txtNazwisko);
            Controls.Add(txtImie);
            Name = "FormDodaj";
            Text = "FormDodaj";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtImie;
        private TextBox txtNazwisko;
        private TextBox txtPESEL;
        private TextBox txtPensja;
        private ComboBox cbTypPracownika;
        private Button btnZapisz;
        private Label lblParametr1;
        private Label lblParametr2;
        private TextBox txtParametr1;
        private TextBox txtParametr2;
    }
}