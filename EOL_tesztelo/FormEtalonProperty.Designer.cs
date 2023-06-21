namespace EOL_tesztelo
{
    partial class FormEtalonProperty
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnEtalonProperty_Adds = new System.Windows.Forms.Button();
            this.btnEtalonProperty_Delete = new System.Windows.Forms.Button();
            this.dGVEtalonMenu = new System.Windows.Forms.DataGridView();
            this.S_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desciptrion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type_1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.type_2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.type_3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Use = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnEtalonProperty_Save = new System.Windows.Forms.Button();
            this.btnEtalonProperty_Cancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnEtalonProperty_RowsDiagnosticUpdate = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dGVEtalonMenu)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEtalonProperty_Adds
            // 
            this.btnEtalonProperty_Adds.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEtalonProperty_Adds.Location = new System.Drawing.Point(3, 3);
            this.btnEtalonProperty_Adds.Name = "btnEtalonProperty_Adds";
            this.btnEtalonProperty_Adds.Size = new System.Drawing.Size(123, 33);
            this.btnEtalonProperty_Adds.TabIndex = 1;
            this.btnEtalonProperty_Adds.Text = "Hozzáad";
            this.btnEtalonProperty_Adds.UseVisualStyleBackColor = true;
            this.btnEtalonProperty_Adds.Click += new System.EventHandler(this.btnEtalonProperty_Adds_Click);
            // 
            // btnEtalonProperty_Delete
            // 
            this.btnEtalonProperty_Delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEtalonProperty_Delete.Location = new System.Drawing.Point(3, 69);
            this.btnEtalonProperty_Delete.Name = "btnEtalonProperty_Delete";
            this.btnEtalonProperty_Delete.Size = new System.Drawing.Size(123, 33);
            this.btnEtalonProperty_Delete.TabIndex = 2;
            this.btnEtalonProperty_Delete.Text = "Törlés";
            this.btnEtalonProperty_Delete.UseVisualStyleBackColor = true;
            this.btnEtalonProperty_Delete.Click += new System.EventHandler(this.btnEtalonProperty_Delete_Click);
            // 
            // dGVEtalonMenu
            // 
            this.dGVEtalonMenu.AllowUserToAddRows = false;
            this.dGVEtalonMenu.AllowUserToDeleteRows = false;
            this.dGVEtalonMenu.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dGVEtalonMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dGVEtalonMenu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dGVEtalonMenu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVEtalonMenu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.S_ID,
            this.Column2,
            this.Desciptrion,
            this.number,
            this.type_1,
            this.type_2,
            this.type_3,
            this.Use,
            this.Column5,
            this.Column10});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dGVEtalonMenu.DefaultCellStyle = dataGridViewCellStyle2;
            this.dGVEtalonMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGVEtalonMenu.Location = new System.Drawing.Point(2, 2);
            this.dGVEtalonMenu.Margin = new System.Windows.Forms.Padding(2);
            this.dGVEtalonMenu.Name = "dGVEtalonMenu";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dGVEtalonMenu.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dGVEtalonMenu.RowHeadersWidth = 51;
            this.dGVEtalonMenu.RowTemplate.Height = 24;
            this.dGVEtalonMenu.Size = new System.Drawing.Size(1090, 537);
            this.dGVEtalonMenu.TabIndex = 107;
            this.dGVEtalonMenu.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGVEtalonMenu_CellEndEdit);
            // 
            // S_ID
            // 
            this.S_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.S_ID.HeaderText = "S_ID";
            this.S_ID.MinimumWidth = 6;
            this.S_ID.Name = "S_ID";
            this.S_ID.Width = 65;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column2.HeaderText = "PC_ID";
            this.Column2.Name = "Column2";
            this.Column2.Width = 76;
            // 
            // Desciptrion
            // 
            this.Desciptrion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Desciptrion.HeaderText = "Leírás";
            this.Desciptrion.Name = "Desciptrion";
            this.Desciptrion.Width = 73;
            // 
            // number
            // 
            this.number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.number.HeaderText = "Sorszám";
            this.number.Name = "number";
            this.number.Width = 94;
            // 
            // type_1
            // 
            this.type_1.HeaderText = "Típus 1";
            this.type_1.Name = "type_1";
            // 
            // type_2
            // 
            this.type_2.HeaderText = "Típus 2";
            this.type_2.Name = "type_2";
            // 
            // type_3
            // 
            this.type_3.HeaderText = "Típus 3";
            this.type_3.Name = "type_3";
            // 
            // Use
            // 
            this.Use.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Use.HeaderText = "Használat";
            this.Use.Items.AddRange(new object[] {
            "Használ",
            "Nem használ"});
            this.Use.Name = "Use";
            this.Use.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Use.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Use.Width = 99;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column5.HeaderText = "Felhasználó";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 113;
            // 
            // Column10
            // 
            this.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column10.HeaderText = "Módosítás dátuma";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 142;
            // 
            // btnEtalonProperty_Save
            // 
            this.btnEtalonProperty_Save.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEtalonProperty_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEtalonProperty_Save.Location = new System.Drawing.Point(961, 3);
            this.btnEtalonProperty_Save.Name = "btnEtalonProperty_Save";
            this.btnEtalonProperty_Save.Size = new System.Drawing.Size(124, 33);
            this.btnEtalonProperty_Save.TabIndex = 108;
            this.btnEtalonProperty_Save.Text = "Mentés";
            this.btnEtalonProperty_Save.UseVisualStyleBackColor = true;
            this.btnEtalonProperty_Save.Click += new System.EventHandler(this.btnEtalonProperty_Save_Click);
            // 
            // btnEtalonProperty_Cancel
            // 
            this.btnEtalonProperty_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEtalonProperty_Cancel.Location = new System.Drawing.Point(3, 3);
            this.btnEtalonProperty_Cancel.Name = "btnEtalonProperty_Cancel";
            this.btnEtalonProperty_Cancel.Size = new System.Drawing.Size(123, 33);
            this.btnEtalonProperty_Cancel.TabIndex = 109;
            this.btnEtalonProperty_Cancel.Text = "Mégse";
            this.btnEtalonProperty_Cancel.UseVisualStyleBackColor = true;
            this.btnEtalonProperty_Cancel.Click += new System.EventHandler(this.btnEtalonProperty_Cancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel1.Controls.Add(this.dGVEtalonMenu, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1229, 587);
            this.tableLayoutPanel1.TabIndex = 110;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnEtalonProperty_RowsDiagnosticUpdate, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.btnEtalonProperty_Adds, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnEtalonProperty_Delete, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1097, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(129, 334);
            this.tableLayoutPanel2.TabIndex = 110;
            // 
            // btnEtalonProperty_RowsDiagnosticUpdate
            // 
            this.btnEtalonProperty_RowsDiagnosticUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEtalonProperty_RowsDiagnosticUpdate.Location = new System.Drawing.Point(3, 267);
            this.btnEtalonProperty_RowsDiagnosticUpdate.Name = "btnEtalonProperty_RowsDiagnosticUpdate";
            this.btnEtalonProperty_RowsDiagnosticUpdate.Size = new System.Drawing.Size(123, 64);
            this.btnEtalonProperty_RowsDiagnosticUpdate.TabIndex = 6;
            this.btnEtalonProperty_RowsDiagnosticUpdate.Text = "Tesztesetek frissitése";
            this.btnEtalonProperty_RowsDiagnosticUpdate.UseVisualStyleBackColor = true;
           // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel3.Controls.Add(this.btnEtalonProperty_Save, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 544);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1088, 40);
            this.tableLayoutPanel3.TabIndex = 111;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.btnEtalonProperty_Cancel, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1097, 544);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(129, 40);
            this.tableLayoutPanel4.TabIndex = 112;
            // 
            // FormEtalonProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 587);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FormEtalonProperty";
            this.Text = "Etalon menű szerkesztő";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormEtalonProperty_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dGVEtalonMenu)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEtalonProperty_Adds;
        private System.Windows.Forms.Button btnEtalonProperty_Delete;
        private System.Windows.Forms.DataGridView dGVEtalonMenu;
        private System.Windows.Forms.Button btnEtalonProperty_Save;
        private System.Windows.Forms.Button btnEtalonProperty_Cancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnEtalonProperty_RowsDiagnosticUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn S_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desciptrion;
        private System.Windows.Forms.DataGridViewTextBoxColumn number;
        private System.Windows.Forms.DataGridViewCheckBoxColumn type_1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn type_2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn type_3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Use;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
    }
}