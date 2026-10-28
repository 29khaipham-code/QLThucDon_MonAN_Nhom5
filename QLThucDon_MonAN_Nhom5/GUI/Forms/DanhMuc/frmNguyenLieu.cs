using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmNguyenLieu : Form
    {
        private NguyenLieuDAL _nguyenLieuDAL = new NguyenLieuDAL();
        private DanhMucBLL _danhMucBLL = new DanhMucBLL();
        private bool _isAdding = false;

        public frmNguyenLieu()
        {
            InitializeComponent();

            // Gọi hàm đổi giao diện ngay khi khởi tạo form
            ApDungGiaoDien();

            this.Load += frmNguyenLieu_Load;
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnBoQua.Click += btnBoQua_Click;
            dgvNguyenLieu.CellClick += dgvNguyenLieu_CellClick;
        }

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
            LoadData();
            SetControlState(false);
        }

        private void LoadComboBoxes()
        {
            cmbDonViTinh.DataSource = _danhMucBLL.LayTatCa(LoaiDanhMuc.DonViTinh);
            cmbDonViTinh.DisplayMember = "Ten";
            cmbDonViTinh.ValueMember = "Ma";

            cmbCongDung.DataSource = _danhMucBLL.LayTatCa(LoaiDanhMuc.CongDung);
            cmbCongDung.DisplayMember = "Ten";
            cmbCongDung.ValueMember = "Ma";
        }

        private void LoadData()
        {
            try
            {
                dgvNguyenLieu.DataSource = _nguyenLieuDAL.GetAll();

                if (dgvNguyenLieu.Columns.Count > 0)
                {
                    dgvNguyenLieu.Columns["MaNguyenLieu"].HeaderText = "Mã NL";
                    dgvNguyenLieu.Columns["TenNguyenLieu"].HeaderText = "Tên Nguyên Liệu";
                    dgvNguyenLieu.Columns["TenDonViTinh"].HeaderText = "Đơn Vị Tính";
                    dgvNguyenLieu.Columns["TenCongDung"].HeaderText = "Công Dụng";
                    dgvNguyenLieu.Columns["DonGia"].HeaderText = "Đơn Giá";
                    dgvNguyenLieu.Columns["DinhDuong"].HeaderText = "Dinh Dưỡng";

                    dgvNguyenLieu.Columns["MaDonViTinh"].Visible = false;
                    dgvNguyenLieu.Columns["MaCongDung"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void dgvNguyenLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !_isAdding)
            {
                DataGridViewRow row = dgvNguyenLieu.Rows[e.RowIndex];
                txtMaNL.Text = row.Cells["MaNguyenLieu"].Value?.ToString();
                txtTenNL.Text = row.Cells["TenNguyenLieu"].Value?.ToString();
                txtDonGia.Text = row.Cells["DonGia"].Value?.ToString();
                txtDinhDuong.Text = row.Cells["DinhDuong"].Value?.ToString();

                cmbDonViTinh.SelectedValue = row.Cells["MaDonViTinh"].Value?.ToString();
                cmbCongDung.SelectedValue = row.Cells["MaCongDung"].Value?.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            _isAdding = true;
            SetControlState(true);
            ClearInputs();
            txtMaNL.Text = _nguyenLieuDAL.NextCode();
            txtTenNL.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNL.Text))
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu cần sửa!");
                return;
            }
            _isAdding = false;
            SetControlState(true);
            txtMaNL.Enabled = false;
            txtTenNL.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNL.Text)) return;

            if (MessageBox.Show("Xóa nguyên liệu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _nguyenLieuDAL.Delete(txtMaNL.Text);
                    LoadData();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNL.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nguyên liệu!");
                txtTenNL.Focus();
                return;
            }

            if (cmbDonViTinh.SelectedValue == null || cmbCongDung.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Công dụng và Đơn vị tính!");
                return;
            }

            decimal donGia = 0;
            decimal.TryParse(txtDonGia.Text, out donGia);

            NguyenLieuDTO nl = new NguyenLieuDTO
            {
                MaNguyenLieu = txtMaNL.Text,
                TenNguyenLieu = txtTenNL.Text,
                MaDonViTinh = cmbDonViTinh.SelectedValue.ToString(),
                MaCongDung = cmbCongDung.SelectedValue.ToString(),
                DonGia = donGia,
                DinhDuong = txtDinhDuong.Text
            };

            try
            {
                if (_isAdding) _nguyenLieuDAL.Insert(nl);
                else _nguyenLieuDAL.Update(nl);

                LoadData();
                SetControlState(false);
                _isAdding = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            _isAdding = false;
            SetControlState(false);
            ClearInputs();
        }

        private void SetControlState(bool isEditing)
        {
            txtTenNL.Enabled = cmbDonViTinh.Enabled = cmbCongDung.Enabled = txtDonGia.Enabled = txtDinhDuong.Enabled = isEditing;
            txtMaNL.Enabled = false;
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = dgvNguyenLieu.Enabled = !isEditing;
            btnLuu.Enabled = btnBoQua.Enabled = isEditing;
        }

        private void ClearInputs()
        {
            txtMaNL.Clear(); txtTenNL.Clear(); txtDonGia.Clear(); txtDinhDuong.Clear();
            if (cmbDonViTinh.Items.Count > 0) cmbDonViTinh.SelectedIndex = 0;
            if (cmbCongDung.Items.Count > 0) cmbCongDung.SelectedIndex = 0;
        }

        // ================= PHẦN GIAO DIỆN =================
        private void ApDungGiaoDien()
        {
            this.BackColor = Ui.Back;
            this.ForeColor = Ui.Text;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            foreach (Control c in this.Controls)
            {
                DinhDangControl(c);
            }

            StyleButton(btnThem, false);
            StyleButton(btnSua, false);
            StyleButton(btnXoa, false);
            StyleButton(btnBoQua, false);
            StyleButton(btnLuu, true);

            Ui.StyleGrid(dgvNguyenLieu);
            StyleGridRows(dgvNguyenLieu);
        }

        private void DinhDangControl(Control c)
        {
            if (c is GroupBox gb)
            {
                gb.BackColor = Ui.Surface;
                gb.ForeColor = Ui.Dark;
                gb.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                foreach (Control child in gb.Controls) DinhDangControl(child);
            }
            else if (c is Label lbl)
            {
                lbl.ForeColor = Ui.Text;
                lbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (c is TextBox txt)
            {
                txt.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                txt.BackColor = Ui.Surface;
                txt.ForeColor = Ui.Text;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is ComboBox cmb)
            {
                cmb.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                cmb.BackColor = Ui.Surface;
                cmb.ForeColor = Ui.Text;
                cmb.FlatStyle = FlatStyle.Flat;
            }
        }

        private void StyleButton(Button button, bool primary)
        {
            if (button == null) return;
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.BackColor = primary ? Ui.Accent : Ui.Surface;
            button.ForeColor = primary ? Color.White : Ui.Dark;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = primary ? Ui.Accent : Ui.Border;
            button.FlatAppearance.MouseOverBackColor = Ui.Hover;
            button.FlatAppearance.MouseDownBackColor = Ui.Header;
            button.Height = Math.Max(button.Height, 32);
        }

        private void StyleGridRows(DataGridView grid)
        {
            if (grid == null) return;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grid.RowTemplate.Height = 28;
            grid.ColumnHeadersHeight = 34;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }
    }
}