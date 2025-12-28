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
            // FormDodaj
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}