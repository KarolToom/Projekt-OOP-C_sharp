namespace Ewidencja_Pracownikow
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            dgvPracownicy = new DataGridView();
            btnOdswiez = new Button();
            btnDodaj = new Button();
            btnUsun = new Button();
            btnImport = new Button();
            btnEksport = new Button();
            cmbSortowanie = new ComboBox();
            btnEdytuj = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvPracownicy).BeginInit();
            SuspendLayout();
            // 
            // dgvPracownicy
            // 
            dgvPracownicy.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPracownicy.Location = new Point(22, 41);
            dgvPracownicy.Name = "dgvPracownicy";
            dgvPracownicy.Size = new Size(766, 235);
            dgvPracownicy.TabIndex = 0;
            dgvPracownicy.CellContentClick += dgvPracownicy_CellContentClick;
            dgvPracownicy.SelectionChanged += dgvPracownicy_SelectionChanged;
            // 
            // btnOdswiez
            // 
            btnOdswiez.Location = new Point(22, 321);
            btnOdswiez.Name = "btnOdswiez";
            btnOdswiez.Size = new Size(146, 23);
            btnOdswiez.TabIndex = 1;
            btnOdswiez.Text = "Pobierz dane z bazy";
            btnOdswiez.UseVisualStyleBackColor = true;
            btnOdswiez.Click += btnOdswiez_Click;
            // 
            // btnDodaj
            // 
            btnDodaj.Location = new Point(22, 350);
            btnDodaj.Name = "btnDodaj";
            btnDodaj.Size = new Size(146, 23);
            btnDodaj.TabIndex = 2;
            btnDodaj.Text = "Dodaj pracownika";
            btnDodaj.UseVisualStyleBackColor = true;
            btnDodaj.Click += btnDodaj_Click;
            // 
            // btnUsun
            // 
            btnUsun.Location = new Point(356, 321);
            btnUsun.Name = "btnUsun";
            btnUsun.Size = new Size(108, 23);
            btnUsun.TabIndex = 3;
            btnUsun.Text = "Usuń";
            btnUsun.UseVisualStyleBackColor = true;
            btnUsun.Click += btnUsun_Click;
            // 
            // btnImport
            // 
            btnImport.Location = new Point(202, 350);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(126, 23);
            btnImport.TabIndex = 5;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // btnEksport
            // 
            btnEksport.Location = new Point(202, 321);
            btnEksport.Name = "btnEksport";
            btnEksport.Size = new Size(126, 23);
            btnEksport.TabIndex = 6;
            btnEksport.Text = "Eksport";
            btnEksport.UseVisualStyleBackColor = true;
            btnEksport.Click += btnEksport_Click;
            // 
            // cmbSortowanie
            // 
            cmbSortowanie.FormattingEnabled = true;
            cmbSortowanie.Items.AddRange(new object[] { "Nazwisko", "Pensja", "Premia", "Suma" });
            cmbSortowanie.Location = new Point(654, 322);
            cmbSortowanie.Name = "cmbSortowanie";
            cmbSortowanie.Size = new Size(121, 23);
            cmbSortowanie.TabIndex = 7;
            cmbSortowanie.Text = "Sortowanie po";
            cmbSortowanie.SelectedIndexChanged += cmbSortowanie_SelectedIndexChanged;
            // 
            // btnEdytuj
            // 
            btnEdytuj.Location = new Point(356, 350);
            btnEdytuj.Name = "btnEdytuj";
            btnEdytuj.Size = new Size(108, 23);
            btnEdytuj.TabIndex = 8;
            btnEdytuj.Text = "Edytuj";
            btnEdytuj.UseVisualStyleBackColor = true;
            btnEdytuj.Click += btnEdytuj_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnEdytuj);
            Controls.Add(cmbSortowanie);
            Controls.Add(btnEksport);
            Controls.Add(btnImport);
            Controls.Add(btnDodaj);
            Controls.Add(btnOdswiez);
            Controls.Add(dgvPracownicy);
            Controls.Add(btnUsun);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvPracownicy).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvPracownicy;
        private Button btnOdswiez;
        private Button btnDodaj;
        private Button btnUsun;
        private Button btnImport;
        private Button btnEksport;
        private ComboBox cmbSortowanie;
        private Button btnEdytuj;
    }
}
