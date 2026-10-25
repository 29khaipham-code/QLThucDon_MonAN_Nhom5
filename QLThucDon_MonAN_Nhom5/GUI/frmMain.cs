using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc;
using QLThucDon_MonAN_Nhom5.GUI.Forms.TimKiem;
using QLThucDon_MonAN_Nhom5.GUI.Forms.NghiepVu;
using QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao;


namespace QLThucDon_MonAN_Nhom5.GUI
{
    public partial class frmMain : Form
    {

        //an thanh cuon trong flowmenu
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        private const int SB_VERT = 1;


        private const bool KieuTrang = true;
        private const bool ChiMoMotNhom = true;
        

        private class NhomMenu
        {
            public Button Nut;  // nút nhóm (btnNhomXxx)
            public string Ten;  // tên nhóm, không kèm mũi tên
            public List<Button> Con = new List<Button>();      // các nút con thuộc nhóm
            public bool Mo;    // trạng thái đích: mở hay thu gọn
            public float TienDo;  // 0 = thu gọn hẳn, 1 = mở hết (dùng cho hoạt ảnh)
        }

        private readonly List<NhomMenu> _nhom = new List<NhomMenu>();
        private readonly Dictionary<Button, int> _chieuCaoGoc = new Dictionary<Button, int>();
        private readonly Dictionary<Button, Color> _mauNenGoc = new Dictionary<Button, Color>();
        private readonly Dictionary<Button, Color> _mauChuGoc = new Dictionary<Button, Color>();
        private readonly Dictionary<string, Button> _nutTheoKhoa = new Dictionary<string, Button>();
        private readonly Dictionary<string, Func<Form>> _dsForm = new Dictionary<string, Func<Form>>();
        private readonly System.Windows.Forms.Timer _timerAnim = new System.Windows.Forms.Timer { Interval = 15 };
        private Button _nutDangChon;
        public frmMain()
        {
            InitializeComponent();
            ApDungGiaoDien();
            DoubleBuffer(flowMenu);   // giảm giật khi chạy hoạt ảnhh

            flowMenu.AutoScroll = true;
            ShowScrollBar(flowMenu.Handle, SB_VERT, 0);

            Load += frmMain_Load;
            FormClosing += frmMain_FormClosing;
            MdiChildActivate += frmMain_MdiChildActivate;
            KeyPreview = true;
            KeyDown += frmMain_KeyDown;

            Shown += (s, e) =>
            {
                BeginInvoke(new Action(AnThanhCuon));
            };

            timer1.Tick += (s, e) => CapNhatGio();
            _timerAnim.Tick += TimerAnim_Tick;

            btnThoat.Click += (s, e) => Close();
            btnThoat.MouseEnter += Nut_MouseEnter;
            btnThoat.MouseLeave += Nut_MouseLeave;
        }

        private void DangKyForm()
        {
            _dsForm["btnLoaiMon"] = () => new frmDanhMuc(LoaiDanhMuc.LoaiMon);
            _dsForm["btnCongDung"] = () => new frmDanhMuc(LoaiDanhMuc.CongDung);
            _dsForm["btnDonViTinh"] = () => new frmDanhMuc(LoaiDanhMuc.DonViTinh);
            _dsForm["btnNoiHoc"] = () => new frmDanhMuc(LoaiDanhMuc.NoiHoc);
            _dsForm["btnTrinhDo"] = () => new frmDanhMuc(LoaiDanhMuc.TrinhDo);
            _dsForm["btnXepLoai"] = () => new frmDanhMuc(LoaiDanhMuc.XepLoai);

             _dsForm["btnNguyenLieu"]     = () => new frmNguyenLieu();
            _dsForm["btnMonAn"] = () => new frmMonAn();
            _dsForm["btnDauBep"]         = () => new frmDauBep();
             _dsForm["btnKhachHang"]      = () => new frmKhachHang();
            _dsForm["btnLapThucDon"] = () => new frmThucDon();
            _dsForm["btnTimKiemMonAn"]       = () => new frmTimKiemMonAn();
             _dsForm["btnTimKiemDauBep"]      = () => new frmTimKiemDauBep();
            _dsForm["btnBcDoanhThu"] = () => new frmBaoCaoDoanhThu();
            _dsForm["btnBcMonAn"] = () => new frmBaoCaoMonAn();
            _dsForm["btnBcMonTheoBep"] = () => new frmBaoCaoMonTheoDauBep();
            _dsForm["btnBcThucDon"] = () => new frmBaoCaoThucDon();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            DangKyForm();
            PhanTichMenu();

            // Ban đầu: chỉ mở nhóm đầu tiên (Danh mục)
            for (int i = 0; i < _nhom.Count; i++)
            {
                _nhom[i].Mo = (i == 0);
                _nhom[i].TienDo = _nhom[i].Mo ? 1f : 0f;
                CapNhatMuiTen(_nhom[i]);
                ApDungTienDo(_nhom[i]);
            }

            lblTieuDe.Text = "Trang chủ";
            timer1.Interval = 1000;
            timer1.Start();
            CapNhatGio();

            // Báo lỗi kết nối sau khi form đã hiện ra
            BeginInvoke(new Action(KiemTraKetNoi));
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !Ui.Confirm("Bạn có chắc muốn thoát chương trình?"))
                e.Cancel = true;
        }
        private void frmMain_MdiChildActivate(object sender, EventArgs e)
        {
            var f = ActiveMdiChild;
            lblTieuDe.Text = f != null ? f.Text : "Trang chủ";

            Button nut = null;
            if (f?.Tag is string khoa) _nutTheoKhoa.TryGetValue(khoa, out nut);
            DatNutDangChon(nut);

            // Tự mở nhóm chứa mục đang chọn
            if (nut != null)
            {
                var nhom = _nhom.FirstOrDefault(n => n.Con.Contains(nut));
                if (nhom != null && !nhom.Mo) DoiTrangThai(nhom, true);
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + W: đóng form con đang mở
            if (e.Control && e.KeyCode == Keys.W)
            {
                ActiveMdiChild?.Close();
                e.Handled = true;
            }
        }

        private void CapNhatGio() =>
           lblGio.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy  HH:mm:ss", new CultureInfo("vi-VN"));

        private void KiemTraKetNoi()
        {
            try { new DanhMucBLL().LayTatCa(LoaiDanhMuc.LoaiMon); }
            catch (Exception ex)
            {
                Ui.Error("Không kết nối được CSDL QuanLyDauBep.\n" +
                         "Hãy kiểm tra chuỗi kết nối trong App.config.\n\nChi tiết: " + ex.Message);
            }
        }

        private void PhanTichMenu()
        {
            NhomMenu hienTai = null; // luu nut nhom dang dc xu ly  (nut cha)

            foreach (Control c in flowMenu.Controls)
            {

                // Chỉ xử lý các Button ( as Button de ep kieu sang button , neu c ko phai btn thi b = null)
                Button b = c as Button;
                if (b == null)
                    continue;

                // Lưu trạng thái ban đầu của nút
                _chieuCaoGoc[b] = b.Height;
                _mauNenGoc[b] = b.BackColor;
                _mauChuGoc[b] = b.ForeColor;

                // Gắn sự kiện rê chuột
                b.MouseEnter -= Nut_MouseEnter;///-= dùng để hủy đăng ký hàm xử lý đó.
                b.MouseEnter += Nut_MouseEnter;///Ký hiệu += dùng để đăng ký một hàm xử lý sự kiện

                b.MouseLeave -= Nut_MouseLeave;
                b.MouseLeave += Nut_MouseLeave;

                // Xác định nút nhóm menu
                if (b.Name.StartsWith(
                    "btnNhom",
                    StringComparison.OrdinalIgnoreCase))
                {
                    hienTai = new NhomMenu
                    {
                        Nut = b,
                        Ten = b.Text
                            .TrimStart('▾', '▸', ' ')
                            .Trim()
                    };

                    _nhom.Add(hienTai);


                    b.Click -= NutNhom_Click;
                    b.Click += NutNhom_Click;
                }
                // Các nút con thuộc nhóm hiện tại
                else if (hienTai != null)
                {
                    hienTai.Con.Add(b);
                    _nutTheoKhoa[b.Name] = b;

                    b.Click -= NutCon_Click;
                    b.Click += NutCon_Click;
                }
            }

            // Lưu màu ban đầu của nút Thoát
            _mauNenGoc[btnThoat] = btnThoat.BackColor;
            _mauChuGoc[btnThoat] = btnThoat.ForeColor;
        }


        private void NutNhom_Click(object sender, EventArgs e)
        {
            var nhom = _nhom.FirstOrDefault(n => n.Nut == sender);
            if (nhom == null) return;
            DoiTrangThai(nhom, !nhom.Mo);
        }

        private void NutCon_Click(object sender, EventArgs e) => MoForm(((Button)sender).Name);

        // ===================== MỞ FORM CON =====================
        private void MoForm(string khoa)
        {
            // Đang mở rồi thì đưa lên trước, không mở thêm
            var daMo = MdiChildren.FirstOrDefault(f => Equals(f.Tag, khoa));
            if (daMo != null) { daMo.Activate(); return; }

            if (!_dsForm.TryGetValue(khoa, out var tao))
            {
                Ui.Info($"Chức năng \"{_nutTheoKhoa[khoa].Text.Trim()}\" chưa được làm.");
                return;
            }

            if (KieuTrang)
                foreach (var f in MdiChildren.ToArray()) f.Close();

            Form frm = tao();
            frm.Tag = khoa;            // dùng để nhận biết form và tô sáng đúng mục menu
            frm.MdiParent = this;
            if (KieuTrang)
            {
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
            }
            frm.Show();
        }

        // ===================== HOẠT ẢNH THU GỌN / MỞ NHÓM =====================
        private void DoiTrangThai(NhomMenu nhom, bool mo)
        {
            if (mo && ChiMoMotNhom)
                foreach (var n in _nhom.Where(n => n != nhom && n.Mo))
                {
                    n.Mo = false;
                    CapNhatMuiTen(n);
                }

            nhom.Mo = mo;
            CapNhatMuiTen(nhom);
            _timerAnim.Start();
        }

        //Mỗi tick: đưa TienDo của từng nhóm tiến dần tới đích (chậm dần về cuối), rồi chỉnh chiều cao các nút con
        private void TimerAnim_Tick(object sender, EventArgs e)
        {
            bool conChay = false;
            flowMenu.SuspendLayout();

            foreach (var n in _nhom)
            {
                float dich = n.Mo ? 1f : 0f;
                if (n.TienDo == dich) continue;

                float buoc = (dich - n.TienDo) * 0.28f;
                if (Math.Abs(buoc) < 0.03f) buoc = Math.Sign(dich - n.TienDo) * 0.03f;   // bước tối thiểu để không kéo dài
                n.TienDo += buoc;
                if ((buoc > 0 && n.TienDo >= dich) || (buoc < 0 && n.TienDo <= dich)) n.TienDo = dich;

                ApDungTienDo(n);
                if (n.TienDo != dich) conChay = true;
            }

            flowMenu.ResumeLayout(true);
            AnThanhCuon();
            if (!conChay) _timerAnim.Stop();
        }

        private void ApDungTienDo(NhomMenu nhom)
        {
            foreach (var b in nhom.Con)
            {
                if (!_chieuCaoGoc.TryGetValue(b, out int chieuCaoGoc))
                    continue;
                int h = (int)Math.Round(_chieuCaoGoc[b] * nhom.TienDo);
                b.Height = h;
                b.Visible = h > 0;
            }
        }

        private void CapNhatMuiTen(NhomMenu nhom) =>
            nhom.Nut.Text = (nhom.Mo ? "▾  " : "▸  ") + nhom.Ten;

        // HIỆU ỨNG MÀU: HOVER & MỤC ĐANG CHỌN 
        private void Nut_MouseEnter(object sender, EventArgs e)
        {
            var b = (Button)sender;
            if (b != _nutDangChon && _mauNenGoc.ContainsKey(b))
                b.BackColor = ControlPaint.Light(_mauNenGoc[b], 0.15f);
        }

        private void Nut_MouseLeave(object sender, EventArgs e)
        {
            var b = (Button)sender;
            if (!_mauNenGoc.ContainsKey(b)) return;
            b.BackColor = b == _nutDangChon ? Ui.Accent : _mauNenGoc[b];
        }

        private void DatNutDangChon(Button nut)
        {
            if (_nutDangChon != null)
            {
                _nutDangChon.BackColor = _mauNenGoc[_nutDangChon];
                _nutDangChon.ForeColor = _mauChuGoc[_nutDangChon];
            }
            _nutDangChon = nut;
            if (nut != null)
            {
                nut.BackColor = Ui.Accent;
                nut.ForeColor = Color.White;
            }
        }
        private void AnThanhCuon()
        {
            if (flowMenu.IsHandleCreated)
            {
                ShowScrollBar(flowMenu.Handle, SB_VERT, 0);
            }
        }

        public void ApDungGiaoDien()
        {
            BackColor = Ui.Back;
            ForeColor = Ui.Text;

            flowMenu.BackColor = Ui.Dark;

            foreach (Control c in flowMenu.Controls)
            {
                if (!(c is Button b))
                    continue;

                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;

                bool laNhom = b.Name.StartsWith(
                    "btnNhom",
                    StringComparison.OrdinalIgnoreCase);

                b.BackColor = laNhom ? Ui.Dark : Ui.Header;
                b.ForeColor = laNhom ? Color.White : Ui.Text;
            }

            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.BackColor = Ui.Accent;
            btnThoat.ForeColor = Color.White;

            lblThuongHieu.BackColor = Ui.Back;
            lblThuongHieu.ForeColor = Ui.Dark;

            lblTieuDe.BackColor = Ui.Back;
            lblTieuDe.ForeColor = Ui.Text;
        }

        // TIỆN ÍCH 
        private static void DoubleBuffer(Control c) =>
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(c, true);
    }
}