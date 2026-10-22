
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.TimKiem
{
    public partial class frmTimKiemDauBep : Form
    {
        private readonly DauBepBLL _dauBepBLL = new DauBepBLL();
        private readonly MonAnBLL _monAnBLL = new MonAnBLL();
        private readonly DanhMucBLL _danhMucBLL = new DanhMucBLL();

        public frmTimKiemDauBep()
        {
            InitializeComponent();

            

            this.Load += frmTimKiemDauBep_Load;
            btnTim.Click += btnTim_Click;
            btnLamMoi.Click += btnLamMoi_Click;
        }

        private void frmTimKiemDauBep_Load(object sender, EventArgs e)
        {
            
            try
            {
                // Nạp danh sách nơi học
                List<DanhMucDTO> dsNoiHoc =
    _danhMucBLL.LayTatCa(LoaiDanhMuc.NoiHoc);

                if (dsNoiHoc == null)
                {
                    MessageBox.Show("Danh sách nơi học bị null!");
                    return;
                }

                dsNoiHoc.Insert(0, new DanhMucDTO
                {
                    Ma = "",
                    Ten = "(Tất cả)"
                });

                cboNoiHoc.DataSource = null;
                cboNoiHoc.DisplayMember = "Ten";
                cboNoiHoc.ValueMember = "Ma";
                cboNoiHoc.DataSource = dsNoiHoc;
                cboNoiHoc.SelectedIndex = 0;

                

                // Nạp danh sách món ăn
                List<MonAnDTO> dsMonAn = _monAnBLL.LayTatCa();
                

                dsMonAn.Insert(0, new MonAnDTO
                {
                    MaMonAn = "",
                    TenMonAn = "(Tất cả)"
                });

                cboMonAn.DataSource = dsMonAn;
                cboMonAn.DisplayMember = "TenMonAn";
                cboMonAn.ValueMember = "MaMonAn";
                cboMonAn.SelectedIndex = 0;

                // Cấu hình bảng kết quả
                dgvDauBep.AutoGenerateColumns = true;
                dgvDauBep.ReadOnly = true;
                dgvDauBep.AllowUserToAddRows = false;
                dgvDauBep.AllowUserToDeleteRows = false;
                dgvDauBep.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dgvDauBep.MultiSelect = false;
                dgvDauBep.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                // Hiển thị danh sách ban đầu
                TimKiemDauBep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi nạp dữ liệu: " + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void TimKiemDauBep()
        {
            try
            {
                string ten = txtTen.Text.Trim();

                string maNoiHoc = "";
                if (cboNoiHoc.SelectedValue != null)
                    maNoiHoc = cboNoiHoc.SelectedValue.ToString();

                string maMonAn = "";
                if (cboMonAn.SelectedValue != null)
                    maMonAn = cboMonAn.SelectedValue.ToString();

                List<DauBepDTO> ds = _dauBepBLL.TimKiem(
                    ten, maNoiHoc, maMonAn);

                dgvDauBep.DataSource = null;
                dgvDauBep.DataSource = ds;

                // Đặt tiêu đề cột
                if (dgvDauBep.Columns["MaDauBep"] != null)
                    dgvDauBep.Columns["MaDauBep"].HeaderText = "Mã đầu bếp";

                if (dgvDauBep.Columns["TenDauBep"] != null)
                    dgvDauBep.Columns["TenDauBep"].HeaderText = "Tên đầu bếp";

                if (dgvDauBep.Columns["GioiTinh"] != null)
                    dgvDauBep.Columns["GioiTinh"].HeaderText = "Giới tính";

                if (dgvDauBep.Columns["DiaChi"] != null)
                    dgvDauBep.Columns["DiaChi"].HeaderText = "Địa chỉ";

                if (dgvDauBep.Columns["DienThoai"] != null)
                    dgvDauBep.Columns["DienThoai"].HeaderText = "Điện thoại";

                if (dgvDauBep.Columns["TenTrinhDo"] != null)
                    dgvDauBep.Columns["TenTrinhDo"].HeaderText = "Trình độ";

                if (dgvDauBep.Columns["TenNoiHoc"] != null)
                    dgvDauBep.Columns["TenNoiHoc"].HeaderText = "Nơi học";

                // Ẩn các cột mã phụ
                if (dgvDauBep.Columns["MaTrinhDo"] != null)
                    dgvDauBep.Columns["MaTrinhDo"].Visible = false;

                if (dgvDauBep.Columns["MaNoiHoc"] != null)
                    dgvDauBep.Columns["MaNoiHoc"].Visible = false;

                lblKetQua.Text = "Tìm thấy " + ds.Count + " đầu bếp";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm: " + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            TimKiemDauBep();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTen.Clear();

            if (cboNoiHoc.Items.Count > 0)
                cboNoiHoc.SelectedIndex = 0;

            if (cboMonAn.Items.Count > 0)
                cboMonAn.SelectedIndex = 0;

            TimKiemDauBep();
        }
    }
}
