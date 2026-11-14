
using QLThucDon_MonAN_Nhom5.BLL;
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
        private NguyenLieuBLL _nguyenLieuBLL = new NguyenLieuBLL();
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
            try
            {
                LoadComboBoxes();
                LoadData();
                SetControlState(false);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể khởi tạo dữ liệu nguyên liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxes()
        {
            var donViTinh = _danhMucBLL.LayTatCa(LoaiDanhMuc.DonViTinh);
            var congDung = _danhMucBLL.LayTatCa(LoaiDanhMuc.CongDung);

            cmbDonViTinh.DataSource = donViTinh;
            cmbDonViTinh.DisplayMember = "Ten";
            cmbDonViTinh.ValueMember = "Ma";
            cmbDonViTinh.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbCongDung.DataSource = congDung;
            cmbCongDung.DisplayMember = "Ten";
            cmbCongDung.ValueMember = "Ma";
            cmbCongDung.DropDownStyle = ComboBoxStyle.DropDownList;

            if (cmbDonViTinh.Items.Count == 0 || cmbCongDung.Items.Count == 0)
            {
                MessageBox.Show(
                    "Danh mục Đơn vị tính hoặc Công dụng chưa có dữ liệu!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void LoadData()
        {
            try
            {
                dgvNguyenLieu.DataSource = _nguyenLieuBLL.LayTatCa();

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
            catch (BusinessException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dữ liệu nguyên liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvNguyenLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _isAdding)
                return;

            DataGridViewRow row = dgvNguyenLieu.Rows[e.RowIndex];

            txtMaNL.Text = Convert.ToString(row.Cells["MaNguyenLieu"].Value);
            txtTenNL.Text = Convert.ToString(row.Cells["TenNguyenLieu"].Value);
            txtDonGia.Text = Convert.ToString(row.Cells["DonGia"].Value);
            txtDinhDuong.Text = Convert.ToString(row.Cells["DinhDuong"].Value);

            object maDVT = row.Cells["MaDonViTinh"].Value;
            object maCD = row.Cells["MaCongDung"].Value;

            if (maDVT != null && maDVT != DBNull.Value)
                cmbDonViTinh.SelectedValue = maDVT;
            else
                cmbDonViTinh.SelectedIndex = -1;

            if (maCD != null && maCD != DBNull.Value)
                cmbCongDung.SelectedValue = maCD;
            else
                cmbCongDung.SelectedIndex = -1;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                _isAdding = true;
                SetControlState(true);
                ClearInputs();

                txtMaNL.Text = _nguyenLieuBLL.TaoMaMoi();
                txtTenNL.Focus();
            }
            catch (BusinessException ex)
            {
                _isAdding = false;
                SetControlState(false);

                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _isAdding = false;
                SetControlState(false);

                MessageBox.Show(
                    "Không thể tạo mã nguyên liệu mới:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNguyenLieu.CurrentRow == null ||
                dgvNguyenLieu.CurrentRow.IsNewRow ||
                dgvNguyenLieu.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn nguyên liệu trong bảng trước khi sửa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _isAdding = false;
            SetControlState(true);
            txtMaNL.Enabled = false;
            txtTenNL.Focus();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNL.Text))
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu cần xóa!");
                return;
            }

            if (MessageBox.Show(
                "Xóa nguyên liệu này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _nguyenLieuBLL.Xoa(txtMaNL.Text.Trim());

                MessageBox.Show(
                    "Xóa nguyên liệu thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
                ClearInputs();
                SetControlState(false);
                _isAdding = false;
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa dữ liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

            if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ và không được âm!");
                txtDonGia.Focus();
                return;
            }

            NguyenLieuDTO nl = new NguyenLieuDTO
            {
                MaNguyenLieu = txtMaNL.Text.Trim(),
                TenNguyenLieu = txtTenNL.Text.Trim(),
                MaDonViTinh = cmbDonViTinh.SelectedValue.ToString(),
                MaCongDung = cmbCongDung.SelectedValue.ToString(),
                DonGia = donGia,
                DinhDuong = txtDinhDuong.Text.Trim()
            };

            try
            {
                if (_isAdding)
                    _nguyenLieuBLL.Them(nl);
                else
                    _nguyenLieuBLL.Sua(nl);

                MessageBox.Show("Lưu nguyên liệu thành công!");

                LoadData();
                ClearInputs();
                SetControlState(false);
                _isAdding = false;
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi lưu nguyên liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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