using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Lớp cha của các form báo cáo dạng bảng (Template Method).
    /// Lớp con chỉ cần: gọi KhoiTaoBaoCao(...) trong constructor và ghi đè các hàm "hook":
    ///   CacCot, LayDuLieu, TinhTieuDe (bắt buộc), TenFileXuat, TinhThongKe, NapBoLoc... (tuỳ chọn).
    /// Các việc chung: định dạng giao diện, nạp lưới, Xem, Xuất Excel, In.
    /// </summary>
    public class BaseReportForm : BaseForm
    {
        private DataGridView _dgv;
        private Label _lblTieuDe;
        private Label _lblSoDong;

        protected DataTable DuLieu { get; private set; }
        protected string TieuDeBaoCao { get; private set; } = "";

        // ----- Hook: lớp con ghi đè -----
        protected virtual CotBaoCao[] CacCot => new CotBaoCao[0];
        protected virtual string TenFileXuat => "BaoCao";
        protected virtual bool LuoiXuongDong => false;
        protected virtual DataTable LayDuLieu() => null;
        protected virtual string TinhTieuDe() => "BÁO CÁO";
        protected virtual string TinhThongKe(DataTable dt) => "Số dòng: " + dt.Rows.Count;
        protected virtual void NapBoLoc() { }          // chạy trước lần Xem đầu tiên (nạp ComboBox lọc)
        protected virtual void SauKhiNapXong() { }     // chạy sau lần Xem đầu tiên (gắn sự kiện lọc)

        // Crystal Report: lớp con khai báo file .rpt thì nút In mở báo cáo Crystal; không khai báo thì in bằng GDI+ như cũ
        protected virtual string TenFileRpt => null;                    // vd "rptMonAn.rpt"
        protected virtual string TenBangRpt => "Bang";                  // tên bảng trong dsBaoCao.xsd
        protected virtual IDictionary<string, object> ThamSoRpt() => null;   // tham số bổ sung (tuỳ chọn)

        /// <summary>Gọi một lần trong constructor của lớp con, ngay sau InitializeComponent().</summary>
        protected void KhoiTaoBaoCao(DataGridView dgv, Button btnXem, Button btnXuat, Button btnIn,
                                     Label lblTieuDe, Label lblSoDong)
        {
            _dgv = dgv;
            _lblTieuDe = lblTieuDe;
            _lblSoDong = lblSoDong;

            // Giao diện chung
            DinhDangForm();
            DinhDangODuLieu(btnXem.Parent);                  // nhóm chứa bộ lọc và nút
            DinhDangNutBam(btnXem, true);
            DinhDangNutBam(btnXuat, false);
            DinhDangNutBam(btnIn, false);
            DinhDangLuoi(dgv, 34, LuoiXuongDong);
            lblTieuDe.ForeColor = Ui.Dark;
            lblTieuDe.Font = FontTieuDe;
            lblSoDong.ForeColor = Ui.Accent;
            lblSoDong.Font = FontSo;

            // Sự kiện chung
            btnXem.Click += (s, e) => Xem();
            btnXuat.Click += (s, e) => BaoCaoHelper.XuatExcel(this, DuLieu, TieuDeBaoCao, CacCot, TenFileXuat);
            btnIn.Click += (s, e) => InBaoCao();

            Load += (s, e) =>
            {
                BaoCaoHelper.CauHinhLuoi(_dgv, CacCot);
                NapBoLoc();
                Xem();            // mở form là có báo cáo ngay
                SauKhiNapXong();
            };
        }

        /// <summary>Lấy dữ liệu qua BLL (lỗi nghiệp vụ hiện thông báo), rồi đổ lên lưới.</summary>
        protected void Xem()
        {
            DataTable dt = null;
            if (!Ui.Run(() => dt = LayDuLieu()) || dt == null) return;

            DuLieu = dt;
            TieuDeBaoCao = TinhTieuDe();

            _dgv.DataSource = dt;
            _lblTieuDe.Text = TieuDeBaoCao;
            _lblSoDong.Text = TinhThongKe(dt);
        }
        protected virtual void InBaoCao()
        {
            BaoCaoHelper.In(
                this,
                DuLieu,
                TieuDeBaoCao,
                CacCot
            );
        }

    }
}
