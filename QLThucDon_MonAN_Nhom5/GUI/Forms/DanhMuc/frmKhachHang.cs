using System;
using System.Windows.Forms;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System.Drawing;
using QLThucDon_MonAN_Nhom5.GUI;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmKhachHang : Form
    {
        private KhachHangDAL _khachHangDAL = new KhachHangDAL();
        private bool _isAdding = false;

        public frmKhachHang()
        {
            InitializeComponent();
            ApDungGiaoDien();
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            LoadData();
            SetControlState(false);
        }

        private void LoadData()
        {
            // Sử dụng GetAll() theo đúng DAL
            dgvKhachHang.DataSource = _khachHangDAL.GetAll();

            if (dgvKhachHang.Columns.Count > 0)
            {
                dgvKhachHang.Columns["MaKhachHang"].HeaderText = "Mã KH";
                dgvKhachHang.Columns["TenKhachHang"].HeaderText = "Tên Khách Hàng";
                dgvKhachHang.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                dgvKhachHang.Columns["DienThoai"].HeaderText = "Điện Thoại";
                dgvKhachHang.Columns["MaSoThue"].HeaderText = "Mã Số Thuế";
            }
        }

        private void SetControlState(bool editing)
        {
            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;
            btnLuu.Enabled = editing;
            btnBoQua.Enabled = editing;

            txtTenKH.Enabled = editing;
            txtDiaChi.Enabled = editing;
            txtDienThoai.Enabled = editing;
            txtMaSoThue.Enabled = editing;
            dgvKhachHang.Enabled = !editing;
        }

        private void ClearInputs()
        {
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtDiaChi.Clear();
            txtDienThoai.Clear();
            txtMaSoThue.Clear();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            _isAdding = true;
            ClearInputs();

            // Gọi hàm NextCode() để tự động điền mã KH mới (VD: KH004)
            txtMaKH.Text = _khachHangDAL.NextCode();

            SetControlState(true);
            txtTenKH.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!");
                return;
            }
            _isAdding = false;
            SetControlState(true);
            txtTenKH.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaKH.Text)) return;

            if (MessageBox.Show("Xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Sử dụng Delete()
                _khachHangDAL.Delete(txtMaKH.Text);
                LoadData();
                ClearInputs();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenKH.Text))
            {
                MessageBox.Show("Tên khách hàng không được để trống!");
                return;
            }

            KhachHangDTO kh = new KhachHangDTO
            {
                MaKhachHang = txtMaKH.Text,
                TenKhachHang = txtTenKH.Text,
                DiaChi = txtDiaChi.Text,
                DienThoai = txtDienThoai.Text,
                MaSoThue = txtMaSoThue.Text
            };

            if (_isAdding)
            {
                // Sử dụng Insert()
                _khachHangDAL.Insert(kh);
            }
            else
            {
                // Sử dụng Update()
                _khachHangDAL.Update(kh);
            }

            LoadData();
            SetControlState(false);
            _isAdding = false;
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            SetControlState(false);
            _isAdding = false;
            ClearInputs();
        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];
                txtMaKH.Text = row.Cells["MaKhachHang"].Value?.ToString();
                txtTenKH.Text = row.Cells["TenKhachHang"].Value?.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();
                txtDienThoai.Text = row.Cells["DienThoai"].Value?.ToString();
                txtMaSoThue.Text = row.Cells["MaSoThue"].Value?.ToString();
            }
        }
        private void ApDungGiaoDien()
        {
            // ==================== 1. FORM ====================
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F);

            // ==================== 2. CÁC Ô NHẬP LIỆU ====================
            TextBox[] dsTextBox =
            {
        txtMaKH,
        txtTenKH,
        txtDiaChi,
        txtDienThoai,
        txtMaSoThue
    };

            foreach (TextBox txt in dsTextBox)
            {
                txt.BackColor = Ui.Surface;
                txt.ForeColor = Ui.Text;
                txt.Font = new Font("Segoe UI", 9F);
                txt.BorderStyle = BorderStyle.FixedSingle;
            }

            // Mã khách hàng được tự động sinh, không cho sửa trực tiếp
            txtMaKH.ReadOnly = true;
            txtMaKH.BackColor = Ui.Header;

            // ==================== 3. CÁC NÚT CHỨC NĂNG ====================
            Button[] dsNut =
            {
        btnThem,
        btnSua,
        btnXoa,
        btnLuu,
        btnBoQua
    };

            foreach (Button b in dsNut)
            {
                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;

                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = Ui.Border;

                b.BackColor = Ui.Surface;
                b.ForeColor = Ui.Dark;
                b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                b.FlatAppearance.MouseOverBackColor = Ui.Hover;
                b.FlatAppearance.MouseDownBackColor = Ui.Header;

                b.Cursor = Cursors.Hand;
            }

            // Nút Lưu sử dụng màu Teal để nhấn mạnh
            btnLuu.BackColor = Ui.Accent;
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatAppearance.BorderColor = Ui.Accent;
            btnLuu.FlatAppearance.MouseOverBackColor = Ui.Dark;
            btnLuu.FlatAppearance.MouseDownBackColor = Ui.Dark;

            // ==================== 4. BẢNG KHÁCH HÀNG ====================
            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.BackgroundColor = Ui.Surface;
            dgvKhachHang.BorderStyle = BorderStyle.None;
            dgvKhachHang.GridColor = Ui.Border;

            dgvKhachHang.ReadOnly = true;
            dgvKhachHang.AllowUserToAddRows = false;
            dgvKhachHang.AllowUserToDeleteRows = false;
            dgvKhachHang.AllowUserToResizeRows = false;

            dgvKhachHang.MultiSelect = false;
            dgvKhachHang.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvKhachHang.RowHeadersVisible = false;

            // Tiêu đề cột
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Ui.Header;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Ui.Dark;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ui.Header;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionForeColor = Ui.Dark;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvKhachHang.ColumnHeadersHeight = 38;
            dgvKhachHang.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nội dung ô
            dgvKhachHang.DefaultCellStyle.BackColor = Ui.Surface;
            dgvKhachHang.DefaultCellStyle.ForeColor = Ui.Text;
            dgvKhachHang.DefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgvKhachHang.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvKhachHang.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvKhachHang.DefaultCellStyle.Padding = new Padding(5, 2, 5, 2);

            // Hàng xen kẽ
            dgvKhachHang.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;
            dgvKhachHang.AlternatingRowsDefaultCellStyle.ForeColor = Ui.Text;
            dgvKhachHang.AlternatingRowsDefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgvKhachHang.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvKhachHang.RowTemplate.Height = 32;
        }
    }
}