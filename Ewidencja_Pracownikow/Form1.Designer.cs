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
            ((System.ComponentModel.ISupportInitialize)dgvPracownicy).BeginInit();
            SuspendLayout();
            // 
            // dgvPracownicy
            // 
            dgvPracownicy.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPracownicy.Location = new Point(201, 98);
            dgvPracownicy.Name = "dgvPracownicy";
            dgvPracownicy.Size = new Size(240, 150);
            dgvPracownicy.TabIndex = 0;
            dgvPracownicy.CellContentClick += dataGridView1_CellContentClick;
            // 
            // btnOdswiez
            // 
            btnOdswiez.Location = new Point(123, 330);
            btnOdswiez.Name = "btnOdswiez";
            btnOdswiez.Size = new Size(146, 23);
            btnOdswiez.TabIndex = 1;
            btnOdswiez.Text = "Pobierz dane z bazy";
            btnOdswiez.UseVisualStyleBackColor = true;
            btnOdswiez.Click += btnOdswiez_Click;
            // 
            // btnDodaj
            // 
            btnDodaj.Location = new Point(366, 330);
            btnDodaj.Name = "btnDodaj";
            btnDodaj.Size = new Size(165, 23);
            btnDodaj.TabIndex = 2;
            btnDodaj.Text = "Dodaj pracownika";
            btnDodaj.UseVisualStyleBackColor = true;
            btnDodaj.Click += btnDodaj_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnDodaj);
            Controls.Add(btnOdswiez);
            Controls.Add(dgvPracownicy);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvPracownicy).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvPracownicy;
        private Button btnOdswiez;
        private Button btnDodaj;
    }
}
