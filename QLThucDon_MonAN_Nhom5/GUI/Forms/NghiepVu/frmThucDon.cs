using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;


namespace QLThucDon_MonAN_Nhom5.GUI.Forms.NghiepVu
{
    public partial class frmThucDon : Form
    {
        // ================= DỮ LIỆU VÀ BLL / DAL =================

        private readonly ThucDonBLL thucDonBLL = new ThucDonBLL();
        private readonly KhachHangDAL khachHangDAL = new KhachHangDAL();
        private readonly DauBepDAL dauBepDAL = new DauBepDAL();
        private readonly MonAnDAL monAnDAL = new MonAnDAL();

        // Bảng tạm lưu các món thuộc thực đơn đang lập
        private readonly DataTable dtChiTiet = new DataTable();

        // Tránh xử lý sự kiện khi đang tải dữ liệu ban đầu
        private bool dangTai = true;

        // Ánh xạ trực tiếp đến control đang có trong Designer gốc.
        // KHÔNG đổi tên hay chỉnh sửa frmThucDon.Designer.cs.
        private TextBox txtSoThucDon => textBox1;
        private ComboBox cboKhachHang => comboBox2;
        private TextBox txtMaSoThue => textBox2;
        private TextBox txtVAT => textBox3;
        private TextBox txtGiamGia => textBox4;
        private TextBox txtTongTien => textBox5;
        private ComboBox cboLoaiMon => comboBox3;
        private ComboBox cboDauBep => comboBox4;
        private ComboBox cboMonAn => comboBox5;
        private TextBox txtSoLuong => textBox6;
        private TextBox txtDonGia => textBox7;
        private TextBox txtThanhTien => textBox8;
        private DataGridView dgvChiTiet => dataGridView1;
        private Button btnThemMon => button1;
        private Button btnXoaDong => button2;
        private Button btnThemMoi => button3;
        private Button btnLuu => button4;
        private Button btnXoa => button5;
        private Button btnInThucDon => button6;

        // Ô Ngày dùng đã được thay bằng DateTimePicker tên dateTimePicker1.
        private void KhoiTaoNgayDung()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.Value = DateTime.Today;
        }

        public frmThucDon()
        {
            InitializeComponent();
            KhoiTaoNgayDung();

            // Áp dụng giao diện dùng chung từ Ui.cs
            ApDungGiaoDien();

            // Khởi tạo bảng chi tiết và các cột
            SetupGrid();

            // ================= ĐĂNG KÝ SỰ KIỆN =================

            Load += frmThucDon_Load;

            cboKhachHang.SelectedIndexChanged +=
                (s, e) => CapNhatMaSoThue();

            cboLoaiMon.SelectedIndexChanged +=
                (s, e) => LoadMonAn();

            cboDauBep.SelectedIndexChanged +=
(s, e) => LoadMonAn();

            cboMonAn.SelectedIndexChanged +=
                (s, e) => CapNhatDonGia();

            txtSoLuong.TextChanged +=
                (s, e) => TinhThanhTienMon();

            txtSoLuong.KeyPress += SoNguyen_KeyPress;

            txtVAT.TextChanged +=
                (s, e) => TinhTongTien();

            txtGiamGia.TextChanged +=
                (s, e) => TinhTongTien();

            btnThemMon.Click += btnThemMon_Click;
            btnXoaDong.Click += btnXoaDong_Click;
            btnThemMoi.Click += btnThemMoi_Click;
            btnLuu.Click += btnLuu_Click;
            btnXoa.Click += btnXoa_Click;
            btnInThucDon.Click += btnInThucDon_Click;
        }

        // =====================================================
        // GIAO DIỆN - dùng màu và hàm StyleGrid của Ui.cs gốc
        // =====================================================
        private void ApDungGiaoDien()
        {
            this.BackColor = Ui.Back;
            this.ForeColor = Ui.Text;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "Quản lý thực đơn";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Áp dụng cho control do Designer tạo, kể cả control nằm trong GroupBox/Panel.
            DinhDangControl(this);

            // Các nút quan trọng / nút phụ
            DinhDangNut(btnThemMon, true);
            DinhDangNut(btnThemMoi, true);
            DinhDangNut(btnLuu, true);
            DinhDangNut(btnXoaDong, false);
            DinhDangNut(btnXoa, false);
            DinhDangNut(btnInThucDon, false);

            // Các ô tự tính, không cho sửa trực tiếp
            txtMaSoThue.ReadOnly = true;
            txtDonGia.ReadOnly = true;
            txtThanhTien.ReadOnly = true;
            txtTongTien.ReadOnly = true;

            foreach (ComboBox cb in new[] { cboKhachHang, cboLoaiMon, cboDauBep, cboMonAn })
                cb.DropDownStyle = ComboBoxStyle.DropDownList;

            Ui.StyleGrid(dgvChiTiet);
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.ReadOnly = true;
            dgvChiTiet.MultiSelect = false;
            dgvChiTiet.RowHeadersVisible = false;
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvChiTiet.ColumnHeadersHeight = 34;
            dgvChiTiet.RowTemplate.Height = 28;
        }

        private void DinhDangControl(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is GroupBox group)
                {
                    group.ForeColor = Ui.Dark;
                    group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                else if (c is Label label)
                {
                    label.ForeColor = Ui.Text;
                    label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                else if (c is TextBox txt)
                {
                    txt.BackColor = Ui.Surface;
                    txt.ForeColor = Ui.Text;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ComboBox combo)
                {
                    combo.BackColor = Ui.Surface;
                    combo.ForeColor = Ui.Text;
                    combo.FlatStyle = FlatStyle.Flat;
                }
                else if (c is DateTimePicker date)
                {
                    date.CalendarForeColor = Ui.Text;
                    date.CalendarMonthBackground = Ui.Surface;
                }

                if (c.HasChildren)
                    DinhDangControl(c);
            }
        }

        private void DinhDangNut(Button b, bool primary)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;
            b.BackColor = primary ? Ui.Accent : Ui.Surface;
            b.ForeColor = primary ? Color.White : Ui.Dark;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = primary ? Ui.Accent : Ui.Border;
            b.FlatAppearance.MouseOverBackColor = primary ? Ui.Dark : Ui.Hover;
            b.FlatAppearance.MouseDownBackColor = Ui.Header;
        }

        // =====================================================
        // KHỞI TẠO DATATABLE VÀ DATAGRIDVIEW
        // =====================================================

        private void SetupGrid()
        {
            // Khai báo cấu trúc dữ liệu chi tiết
            dtChiTiet.Columns.Add("MaMon", typeof(string));
            dtChiTiet.Columns.Add("TenMon", typeof(string));
            dtChiTiet.Columns.Add("MaDauBep", typeof(string));
            dtChiTiet.Columns.Add("TenDauBep", typeof(string));
            dtChiTiet.Columns.Add("SoLuong", typeof(int));
            dtChiTiet.Columns.Add("DonGia", typeof(decimal));
            dtChiTiet.Columns.Add("ThanhTien", typeof(decimal));

            // Chỉ khai báo cột một lần, không tự sinh cột từ DataTable
            dgvChiTiet.Columns.Clear();
            dgvChiTiet.AutoGenerateColumns = false;

            ThemCot("MaMon", "Mã món", 12, null);
            ThemCot("TenMon", "Tên món", 32, null);
            ThemCot("TenDauBep", "Đầu bếp", 22, null);
            ThemCot("SoLuong", "SL", 8, null);
            ThemCot("DonGia", "Đơn giá", 13, "N0");
            ThemCot("ThanhTien", "Thành tiền", 13, "N0");

            // Liên kết bảng dữ liệu với lưới
            dgvChiTiet.DataSource = dtChiTiet;
        }

        private void ThemCot(
            string prop,
            string header,
float weight,
            string format)
        {
            var col = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Name = "col" + prop,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            if (format != null)
            {
                col.DefaultCellStyle.Format = format;
            }

            dgvChiTiet.Columns.Add(col);
        }

        // =====================================================
        // LOAD FORM
        // =====================================================

        private void frmThucDon_Load(object sender, EventArgs e)
        {
            txtVAT.Text = "8";
            txtGiamGia.Text = "0";
            txtTongTien.Text = "0";
            dateTimePicker1.Value = DateTime.Today;

            try
            {
                dangTai = true;

                // Tải danh sách khách hàng
                cboKhachHang.DataSource = khachHangDAL.GetAll();
                cboKhachHang.DisplayMember = "TenKhachHang";
                cboKhachHang.ValueMember = "MaKhachHang";

                // Tải danh sách loại món
                cboLoaiMon.DataSource = DbHelper.ExecuteDataTable(
                    "SELECT MaLoai, TenLoai FROM LoaiMon ORDER BY TenLoai");

                cboLoaiMon.DisplayMember = "TenLoai";
                cboLoaiMon.ValueMember = "MaLoai";

                // Tải danh sách đầu bếp
                cboDauBep.DataSource = dauBepDAL.GetAll();
                cboDauBep.DisplayMember = "TenDauBep";
                cboDauBep.ValueMember = "MaDauBep";

                dangTai = false;

                CapNhatMaSoThue();
                LoadMonAn();

                // Tạo số thực đơn mới
                txtSoThucDon.Text = thucDonBLL.TaoSoMoi();
            }
            catch (Exception ex)
            {
                dangTai = false;

                MessageBox.Show(
                    "Lỗi load dữ liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CẬP NHẬT THÔNG TIN KHÁCH HÀNG
        // =====================================================

        private void CapNhatMaSoThue()
        {
            var kh = cboKhachHang.SelectedItem as KhachHangDTO;

            txtMaSoThue.Text = kh != null
                ? kh.MaSoThue
                : "";
        }

        // =====================================================
        // TẢI MÓN ĂN THEO LOẠI MÓN VÀ ĐẦU BẾP
        // =====================================================

        private void LoadMonAn()
        {
            if (dangTai)
                return;
            string maLoai = cboLoaiMon.SelectedValue as string;
            string maBep = cboDauBep.SelectedValue as string;

            if (maLoai == null || maBep == null)
            {
                cboMonAn.DataSource = null;
            }
            else
            {
                try
                {
                    cboMonAn.DataSource =
                        monAnDAL.GetByLoaiVaDauBep(maLoai, maBep);

                    cboMonAn.DisplayMember = "TenMonAn";
                    cboMonAn.ValueMember = "MaMonAn";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Lỗi lấy danh sách món:\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            CapNhatDonGia();
        }

        // =====================================================
        // CẬP NHẬT ĐƠN GIÁ VÀ THÀNH TIỀN MÓN ĐANG CHỌN
        // =====================================================

        private void CapNhatDonGia()
        {
            var mon = cboMonAn.SelectedItem as MonAnDTO;

            txtDonGia.Text = mon != null
                ? mon.DonGia.ToString("N0")
                : "";

            TinhThanhTienMon();
        }

        private void TinhThanhTienMon()
        {
            var mon = cboMonAn.SelectedItem as MonAnDTO;

            int sl;

            if (mon != null &&
                int.TryParse(txtSoLuong.Text, out sl) &&
                sl > 0)
            {
                txtThanhTien.Text =
                    thucDonBLL.TinhThanhTien(sl, mon.DonGia)
                        .ToString("N0");
            }
            else
            {
                txtThanhTien.Text = "";
            }
        }

        // Chỉ cho phép nhập số nguyên vào ô số lượng
        private void SoNguyen_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // =====================================================
        // ĐỌC DỮ LIỆU TIỀN VÀ PHẦN TRĂM
        // =====================================================

        private static decimal DocTien(string s)
        {
            string chuSo = new string(
                (s ?? "").Where(char.IsDigit).ToArray());

            return decimal.TryParse(chuSo, out decimal d)
                ? d
                : 0m;
        }

        private static decimal DocPhanTram(string s)
        {
            string x = (s ?? "").Trim().Replace(',', '.');

            return decimal.TryParse(
                x,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal d)
? d
                : 0m;
        }

        // =====================================================
        // TÍNH TỔNG TIỀN THỰC ĐƠN
        // =====================================================

        private void TinhTongTien()
        {
            var dsChiTiet = new List<ChiTietThucDonDTO>();

            foreach (DataRow r in dtChiTiet.Rows)
            {
                dsChiTiet.Add(new ChiTietThucDonDTO
                {
                    ThanhTien = Convert.ToDecimal(r["ThanhTien"])
                });
            }

            decimal vat = DocPhanTram(txtVAT.Text);
            decimal giam = DocTien(txtGiamGia.Text);

            decimal tong = thucDonBLL.TinhTongTien(
                dsChiTiet,
                vat,
                giam);

            txtTongTien.Text = tong.ToString("N0");
        }

        // =====================================================
        // THÊM MÓN VÀO CHI TIẾT THỰC ĐƠN
        // =====================================================

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            var mon = cboMonAn.SelectedItem as MonAnDTO;
            var bep = cboDauBep.SelectedItem as DauBepDTO;

            if (mon == null || bep == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn loại món, đầu bếp và món ăn!\n" +
                    "(Đầu bếp phải làm được món thuộc loại đã chọn.)",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int sl;

            if (!int.TryParse(txtSoLuong.Text, out sl) || sl <= 0)
            {
                MessageBox.Show(
                    "Số lượng phải là số nguyên lớn hơn 0!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtSoLuong.Focus();
                return;
            }

            // Kiểm tra món và đầu bếp đã tồn tại trong chi tiết chưa
            DataRow dong = null;

            foreach (DataRow r in dtChiTiet.Rows)
            {
                if ((string)r["MaMon"] == mon.MaMonAn &&
                    (string)r["MaDauBep"] == bep.MaDauBep)
                {
                    dong = r;
                    break;
                }
            }

            if (dong != null)
            {
                // Nếu đã có thì cộng dồn số lượng
                int slMoi = (int)dong["SoLuong"] + sl;

                dong["SoLuong"] = slMoi;

                dong["ThanhTien"] = thucDonBLL.TinhThanhTien(
                    slMoi,
                    (decimal)dong["DonGia"]);
            }
            else
            {
                // Nếu chưa có thì thêm dòng mới
                dtChiTiet.Rows.Add(
                    mon.MaMonAn,
                    mon.TenMonAn,
bep.MaDauBep,
                    bep.TenDauBep,
                    sl,
                    mon.DonGia,
                    thucDonBLL.TinhThanhTien(sl, mon.DonGia));
            }

            txtSoLuong.Clear();
            TinhTongTien();
        }

        // =====================================================
        // XÓA MỘT DÒNG CHI TIẾT
        // =====================================================

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            var drv = dgvChiTiet.CurrentRow != null
                ? dgvChiTiet.CurrentRow.DataBoundItem as DataRowView
                : null;

            if (drv == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn dòng cần xoá trong danh sách!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            dtChiTiet.Rows.Remove(drv.Row);

            TinhTongTien();
        }

        // =====================================================
        // TẠO THỰC ĐƠN MỚI / RESET FORM
        // =====================================================

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            dtChiTiet.Clear();

            txtVAT.Text = "8";
            txtGiamGia.Text = "0";
            txtSoLuong.Clear();

            dateTimePicker1.Value = DateTime.Today;

            try
            {
                txtSoThucDon.Text = thucDonBLL.TaoSoMoi();
            }
            catch
            {
                txtSoThucDon.Clear();
            }

            TinhTongTien();

            txtSoThucDon.Focus();
        }

        // =====================================================
        // LƯU THỰC ĐƠN
        // =====================================================

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string soTD = txtSoThucDon.Text.Trim();

            if (soTD.Length == 0)
            {
                MessageBox.Show(
                    "Vui lòng nhập số thực đơn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (cboKhachHang.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khách hàng!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (dtChiTiet.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Thực đơn chưa có món nào. Hãy thêm món trước khi lưu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
MessageBoxIcon.Warning);

                return;
            }

            // Lấy ngày đã chọn trực tiếp từ DateTimePicker.
            DateTime ngayDung = dateTimePicker1.Value.Date;

            // Đóng gói thông tin thực đơn
            var td = new ThucDonDTO
            {
                SoThucDon = soTD,
                MaKhachHang = cboKhachHang.SelectedValue.ToString(),
                NgayDung = ngayDung.Date,
                ThueVAT = DocPhanTram(txtVAT.Text),
                GiamGia = DocTien(txtGiamGia.Text)
            };

            // Đóng gói danh sách chi tiết thực đơn
            var dsChiTiet = new List<ChiTietThucDonDTO>();

            foreach (DataRow r in dtChiTiet.Rows)
            {
                dsChiTiet.Add(new ChiTietThucDonDTO
                {
                    SoThucDon = soTD,
                    MaMonAn = (string)r["MaMon"],
                    MaDauBep = (string)r["MaDauBep"],
                    SoLuong = (int)r["SoLuong"]
                });
            }

            try
            {
                thucDonBLL.Them(td, dsChiTiet);

                MessageBox.Show(
                    "Lưu thực đơn " + soTD + " thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ResetForm();
            }
            catch (SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show(
                    "Số thực đơn \"" + soTD +
                    "\" đã tồn tại. Hãy nhập số khác!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi nghiệp vụ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // XÓA THỰC ĐƠN
        // =====================================================

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string soTD = txtSoThucDon.Text.Trim();

            if (soTD.Length == 0)
            {
                MessageBox.Show(
                    "Vui lòng nhập số thực đơn cần xoá!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Xoá thực đơn " + soTD + " (kèm chi tiết)?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                thucDonBLL.Xoa(soTD);

                MessageBox.Show(
                    "Đã xoá thực đơn " + soTD + "!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi xoá:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // IN THỰC ĐƠN
        // =====================================================

        private void btnInThucDon_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Chức năng in thực đơn đang được cập nhật!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}