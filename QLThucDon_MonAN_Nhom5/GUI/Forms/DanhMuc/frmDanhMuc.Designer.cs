namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    partial class frmDanhMuc
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
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMa = new System.Windows.Forms.Label();
            this.txtMa = new System.Windows.Forms.TextBox();
            this.lblTen = new System.Windows.Forms.Label();
            this.txtTen = new System.Windows.Forms.TextBox();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnThem = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnMoi = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.flowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayout
            // 
            this.flowLayout.Controls.Add(this.lblMa);
            this.flowLayout.Controls.Add(this.txtMa);
            this.flowLayout.Controls.Add(this.lblTen);
            this.flowLayout.Controls.Add(this.txtTen);
            this.flowLayout.Controls.Add(this.btnThem);
            this.flowLayout.Controls.Add(this.btnXoa);
            this.flowLayout.Controls.Add(this.btnSua);
            this.flowLayout.Controls.Add(this.btnMoi);
            this.flowLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayout.Location = new System.Drawing.Point(0, 0);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Padding = new System.Windows.Forms.Padding(14, 12, 14, 0);
            this.flowLayout.Size = new System.Drawing.Size(800, 104);
            this.flowLayout.TabIndex = 0;
            // 
            // lblMa
            // 
            this.lblMa.AutoSize = true;
            this.lblMa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMa.Location = new System.Drawing.Point(14, 21);
            this.lblMa.Margin = new System.Windows.Forms.Padding(0, 9, 6, 0);
            this.lblMa.Name = "lblMa";
            this.lblMa.Size = new System.Drawing.Size(31, 18);
            this.lblMa.TabIndex = 0;
            this.lblMa.Text = "Mã";
            // 
            // txtMa
            // 
            this.txtMa.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtMa.Location = new System.Drawing.Point(54, 15);
            this.txtMa.Name = "txtMa";
            this.txtMa.Size = new System.Drawing.Size(110, 22);
            this.txtMa.TabIndex = 1;
            // 
            // lblTen
            // 
            this.lblTen.AutoSize = true;
            this.lblTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTen.Location = new System.Drawing.Point(207, 21);
            this.lblTen.Margin = new System.Windows.Forms.Padding(40, 9, 6, 0);
            this.lblTen.Name = "lblTen";
            this.lblTen.Size = new System.Drawing.Size(111, 18);
            this.lblTen.TabIndex = 2;
            this.lblTen.Text = "Tên Loại Món";
            // 
            // txtTen
            // 
            this.txtTen.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flowLayout.SetFlowBreak(this.txtTen, true);
            this.txtTen.Location = new System.Drawing.Point(327, 15);
            this.txtTen.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.txtTen.MaxLength = 100;
            this.txtTen.Name = "txtTen";
            this.txtTen.Size = new System.Drawing.Size(320, 22);
            this.txtTen.TabIndex = 3;
            // 
            // btnXoa
            // 
            this.btnXoa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoa.Location = new System.Drawing.Point(115, 60);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(75, 23);
            this.btnXoa.TabIndex = 6;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.UseVisualStyleBackColor = true;
            // 
            // btnThem
            // 
            this.btnThem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThem.Location = new System.Drawing.Point(17, 60);
            this.btnThem.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(75, 23);
            this.btnThem.TabIndex = 4;
            this.btnThem.Text = "Thêm";
            this.btnThem.UseVisualStyleBackColor = true;
            // 
            // btnSua
            // 
            this.btnSua.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSua.Location = new System.Drawing.Point(213, 60);
            this.btnSua.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.btnSua.Name = "btnSua";
            this.btnSua.Size = new System.Drawing.Size(75, 23);
            this.btnSua.TabIndex = 5;
            this.btnSua.Text = "Sửa";
            this.btnSua.UseVisualStyleBackColor = true;
            // 
            // btnMoi
            // 
            this.btnMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoi.Location = new System.Drawing.Point(311, 60);
            this.btnMoi.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.btnMoi.Name = "btnMoi";
            this.btnMoi.Size = new System.Drawing.Size(75, 23);
            this.btnMoi.TabIndex = 7;
            this.btnMoi.Text = "Làm mới";
            this.btnMoi.UseVisualStyleBackColor = true;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 104);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(800, 416);
            this.dgv.TabIndex = 1;
            // 
            // frmDanhMuc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.flowLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(640, 520);
            this.Name = "frmDanhMuc";
            this.Text = "frmDanhMuc";
            this.flowLayout.ResumeLayout(false);
            this.flowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.Label lblMa;
        private System.Windows.Forms.TextBox txtMa;
        private System.Windows.Forms.Label lblTen;
        private System.Windows.Forms.TextBox txtTen;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnMoi;
        private System.Windows.Forms.DataGridView dgv;
    }
}