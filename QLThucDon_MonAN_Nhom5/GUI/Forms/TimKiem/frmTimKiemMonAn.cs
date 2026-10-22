
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.TimKiem
{
    public partial class frmTimKiemMonAn : Form
    {
        private readonly MonAnBLL _monAnBLL = new MonAnBLL();
        private readonly DanhMucBLL _danhMucBLL = new DanhMucBLL();

        public frmTimKiemMonAn()
        {
            InitializeComponent();
            ApDungGiaoDien();

            // Gắn sự kiện
            this.Load += frmTimKiemMonAn_Load;
            btnTim.Click += btnTim_Click;
            btnLamMoi.Click += btnLamMoi_Click;
        }

        private void frmTimKiemMonAn_Load(object sender, EventArgs e)
        {
            try
            {
                // Nạp danh sách loại món
                cboLoai.DataSource =
                    _danhMucBLL.LayTatCa(LoaiDanhMuc.LoaiMon);

                cboLoai.DisplayMember = "Ten";
                cboLoai.ValueMember = "Ma";
                cboLoai.SelectedIndex = -1;

                // Nạp danh sách công dụng
                var dsCongDung =
                    _danhMucBLL.LayTatCa(LoaiDanhMuc.CongDung);

                // Thêm lựa chọn Tất cả
                dsCongDung.Insert(0, new DanhMucDTO
                {
                    Ma = "",
                    Ten = "(Tất cả)"
                });

                cboCongDung.DataSource = dsCongDung;
                cboCongDung.DisplayMember = "Ten";
                cboCongDung.ValueMember = "Ma";
                cboCongDung.SelectedIndex = 0;

                // Cấu hình bảng
                dgvMonAn.AutoGenerateColumns = true;
                dgvMonAn.ReadOnly = true;
                dgvMonAn.AllowUserToAddRows = false;
                dgvMonAn.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dgvMonAn.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                // Hiển thị toàn bộ món ăn ban đầu
                TimKiemMonAn();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            TimKiemMonAn();
        }

        private void TimKiemMonAn()
        {
            try
            {
                string maLoai = cboLoai.SelectedValue == null
                    ? ""
                    : cboLoai.SelectedValue.ToString();

                string maCongDung = cboCongDung.SelectedValue == null
                    ? ""
                    : cboCongDung.SelectedValue.ToString();

                string tenMon = txtTenMon.Text.Trim();

                List<MonAnDTO> ketQua = _monAnBLL.TimKiem(
                    maLoai,
                    maCongDung,
                    tenMon);

                dgvMonAn.DataSource = null;
                dgvMonAn.DataSource = ketQua;

                // Đặt tên cột dễ nhìn
                if (dgvMonAn.Columns["MaMonAn"] != null)
                    dgvMonAn.Columns["MaMonAn"].HeaderText = "Mã món";

                if (dgvMonAn.Columns["TenMonAn"] != null)
                    dgvMonAn.Columns["TenMonAn"].HeaderText = "Tên món";

                if (dgvMonAn.Columns["TenLoai"] != null)
                    dgvMonAn.Columns["TenLoai"].HeaderText = "Loại";

                if (dgvMonAn.Columns["TenCongDung"] != null)
                    dgvMonAn.Columns["TenCongDung"].HeaderText = "Công dụng";

                if (dgvMonAn.Columns["DonGia"] != null)
                {
                    dgvMonAn.Columns["DonGia"].HeaderText = "Đơn giá";
                    dgvMonAn.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                }

                // Ẩn các cột không cần hiển thị
                string[] cotAn = {
                    "MaLoai", "MaCongDung", "CachLam", "YeuCau"
                };

                foreach (string tenCot in cotAn)
                {
                    if (dgvMonAn.Columns[tenCot] != null)
                        dgvMonAn.Columns[tenCot].Visible = false;
                }

                lblKetQua.Text = "Tìm thấy " +
                    ketQua.Count + " món ăn";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Tìm kiếm thất bại: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            cboLoai.SelectedIndex = -1;
            cboCongDung.SelectedIndex = 0;
            txtTenMon.Clear();

            TimKiemMonAn();
        }

        private void ApDungGiaoDien()
        {
            // Giao diện chung của form
            this.BackColor = Ui.Back;
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Tìm kiếm món ăn";

            // GroupBox điều kiện tìm kiếm
            groupBox1.BackColor = Ui.Surface;
            groupBox1.ForeColor = Ui.Dark;
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Nhãn
            label1.ForeColor = Ui.Text;
            label2.ForeColor = Ui.Text;
            label3.ForeColor = Ui.Text;

            // Ô nhập tên món
            txtTenMon.Font = new Font("Segoe UI", 9F);
            txtTenMon.BackColor = Ui.Surface;
            txtTenMon.ForeColor = Ui.Text;

            // ComboBox
            cboLoai.Font = new Font("Segoe UI", 9F);
            cboLoai.BackColor = Ui.Surface;
            cboLoai.ForeColor = Ui.Text;

            cboCongDung.Font = new Font("Segoe UI", 9F);
            cboCongDung.BackColor = Ui.Surface;
            cboCongDung.ForeColor = Ui.Text;

            // Áp dụng giao diện cho nút có sẵn trong Designer
            btnTim.Text = "Tìm";
            btnTim.BackColor = Ui.Accent;
            btnTim.ForeColor = Color.White;
            btnTim.FlatStyle = FlatStyle.Flat;
            btnTim.FlatAppearance.BorderSize = 0;

            btnLamMoi.Text = "Làm mới";
            btnLamMoi.BackColor = Ui.Surface;
            btnLamMoi.ForeColor = Ui.Dark;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderColor = Ui.Border;
            btnLamMoi.FlatAppearance.BorderSize = 1;

            // Giữ nguyên vị trí và kích thước nút trên form
            btnTim.Location = new Point(723, 33);
            btnTim.Size = new Size(75, 30);
            btnTim.MinimumSize = new Size(75, 30);

            btnLamMoi.Location = new Point(820, 33);
            btnLamMoi.Size = new Size(75, 30);
            btnLamMoi.MinimumSize = new Size(75, 30);

            groupBox1.Controls.Add(btnTim);
            groupBox1.Controls.Add(btnLamMoi);

            // Bảng dữ liệu
            Ui.StyleGrid(dgvMonAn);

            dgvMonAn.Font = new Font("Segoe UI", 9F);
            dgvMonAn.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvMonAn.RowTemplate.Height = 30;
            dgvMonAn.BorderStyle = BorderStyle.None;
            dgvMonAn.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMonAn.RowHeadersVisible = false;

            // Nhãn kết quả
            lblKetQua.ForeColor = Ui.Dark;
            lblKetQua.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

    }
}
