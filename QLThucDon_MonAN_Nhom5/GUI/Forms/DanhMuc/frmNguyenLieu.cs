using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmNguyenLieu : Form
    {
        private NguyenLieuDAL _nguyenLieuDAL = new NguyenLieuDAL();
        private bool _isAdding = false;

        public frmNguyenLieu()
        {
            InitializeComponent();

            // Tự động gắn sự kiện bằng code (thay thế cho việc nhấp đúp trên giao diện)
            this.Load += frmNguyenLieu_Load;
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnBoQua.Click += btnBoQua_Click;
            dgvNguyenLieu.CellClick += dgvNguyenLieu_CellClick;
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
        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
            LoadData();
            SetControlState(false);
        }

        private void LoadComboBoxes()
        {
            // Tạm thời comment nếu bạn chưa có DonViTinhDAL và CongDungDAL
            /*
            cmbDonViTinh.DataSource = new DonViTinhDAL().GetAll();
            cmbDonViTinh.DisplayMember = "TenDonViTinh";
            cmbDonViTinh.ValueMember = "MaDonViTinh";
            
            cmbCongDung.DataSource = new CongDungDAL().GetAll();
            cmbCongDung.DisplayMember = "TenCongDung";
            cmbCongDung.ValueMember = "MaCongDung";
            */
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

        private void btnLuu_Click(object sender, EventArgs e)
        {

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
            cmbDonViTinh.SelectedIndex = -1; cmbCongDung.SelectedIndex = -1;
        }

       
    }
}