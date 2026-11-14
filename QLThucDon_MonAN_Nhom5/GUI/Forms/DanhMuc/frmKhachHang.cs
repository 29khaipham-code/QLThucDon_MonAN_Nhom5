using QLThucDon_MonAN_Nhom5.BLL;
//using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using QLThucDon_MonAN_Nhom5.GUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmKhachHang : Form
    {
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
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
            try
            {
                dgvKhachHang.DataSource = _khachHangBLL.LayTatCa();

                if (dgvKhachHang.Columns.Count > 0)
                {
                    dgvKhachHang.Columns["MaKhachHang"].HeaderText = "Mã KH";
                    dgvKhachHang.Columns["TenKhachHang"].HeaderText = "Tên Khách Hàng";
                    dgvKhachHang.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                    dgvKhachHang.Columns["DienThoai"].HeaderText = "Điện Thoại";
                    dgvKhachHang.Columns["MaSoThue"].HeaderText = "Mã Số Thuế";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách khách hàng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
            try
            {
                // Tạo mã trước, nếu lỗi thì không chuyển form sang trạng thái thêm
                string maMoi = _khachHangBLL.TaoMaMoi();

                ClearInputs();
                txtMaKH.Text = maMoi;

                _isAdding = true;
                SetControlState(true);
                txtTenKH.Focus();
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
                    "Lỗi khi tạo khách hàng mới:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaKH.Text))
                {
                    MessageBox.Show(
                        "Vui lòng chọn khách hàng cần xóa!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa khách hàng này không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                _khachHangBLL.Xoa(txtMaKH.Text.Trim());

                MessageBox.Show(
                    "Xóa khách hàng thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
                ClearInputs();
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
                    "Lỗi khi xóa khách hàng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenKH.Text))
                {
                    MessageBox.Show("Tên khách hàng không được để trống!");
                    txtTenKH.Focus();
                    return;
                }

                var kh = new KhachHangDTO
                {
                    MaKhachHang = txtMaKH.Text.Trim(),
                    TenKhachHang = txtTenKH.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    MaSoThue = txtMaSoThue.Text.Trim()
                };

                if (_isAdding)
                    _khachHangBLL.Them(kh);
                else
                    _khachHangBLL.Sua(kh);

                MessageBox.Show(
                    "Lưu thông tin khách hàng thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
                ClearInputs();
                _isAdding = false;
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
                    "Lỗi khi lưu khách hàng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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