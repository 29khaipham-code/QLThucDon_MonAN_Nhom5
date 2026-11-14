
using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    public partial class frmBaoCaoDoanhThu : Form
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();

        private DataTable _dt;
        private decimal _tong;
        private string _tieuDe = "";
        private int _dongDaIn;

        public frmBaoCaoDoanhThu()
        {
            InitializeComponent();
            ApDungGiaoDien();

            Text = "Báo cáo doanh thu đầu bếp";

            cmbKy.DropDownStyle = ComboBoxStyle.DropDownList;
            nudNam.Minimum = 2000;
            nudNam.Maximum = 2100;

            Load += frmBaoCaoDoanhThu_Load;
            rdoThang.CheckedChanged += Kieu_CheckedChanged;
            rdoQuy.CheckedChanged += Kieu_CheckedChanged;
            rdoNam.CheckedChanged += Kieu_CheckedChanged;
            btnXem.Click += btnXem_Click;
            btnXuat.Click += btnXuat_Click;
            btnIn.Click += btnIn_Click;
        }

        private void frmBaoCaoDoanhThu_Load(object sender, EventArgs e)
        {
            try
            {
                CauHinhLuoi();

                nudNam.Value = Math.Min(
                    nudNam.Maximum,
                    Math.Max(nudNam.Minimum, DateTime.Now.Year));

                rdoQuy.Checked = true;
                CapNhatKy();

                XemBaoCao(baoKhiTrong: false);
            }
            catch (BusinessException ex)
            {
                Ui.Info(ex.Message);
            }
            catch (Exception ex)
            {
                Ui.Error("Không thể khởi tạo báo cáo doanh thu: " + ex.Message);
            }
        }

        private void CauHinhLuoi()
        {
            dgv.AutoGenerateColumns = false;
            dgv.ReadOnly = true;

            if (dgv.Columns.Count == 0)
            {
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "MaDauBep",
                    Width = 120
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TenDauBep",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SoPhan",
                    Width = 130
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DoanhThu",
                    Width = 190
                });
            }

            string[] prop =
            {
                "MaDauBep",
                "TenDauBep",
                "SoPhan",
                "DoanhThu"
            };

            string[] tieuDe =
            {
                "Mã đầu bếp",
                "Tên đầu bếp",
                "Số phần",
                "Doanh thu"
            };

            for (int i = 0; i < prop.Length && i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].DataPropertyName = prop[i];
                dgv.Columns[i].HeaderText = tieuDe[i];
            }

            foreach (int i in new[] { 2, 3 })
            {
                if (i >= dgv.Columns.Count) continue;

                dgv.Columns[i].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgv.Columns[i].DefaultCellStyle.Format = "N0";

                dgv.Columns[i].HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }

        // ===================== TÙY CHỌN KỲ THỐNG KÊ =====================

        private void Kieu_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rdo && rdo.Checked)
                CapNhatKy();
        }

        private KieuThongKe LayKieu() =>
            rdoThang.Checked ? KieuThongKe.Thang :
            rdoQuy.Checked ? KieuThongKe.Quy :
            KieuThongKe.Nam;

        private void CapNhatKy()
        {
            var kieu = LayKieu();

            bool coKy = kieu != KieuThongKe.Nam;

            lblKy.Visible = coKy;
            cmbKy.Visible = coKy;

            if (!coKy) return;

            lblKy.Text = kieu == KieuThongKe.Thang ? "Tháng" : "Quý";

            int soMuc = kieu == KieuThongKe.Thang ? 12 : 4;

            int mucHienTai = kieu == KieuThongKe.Thang
                ? DateTime.Now.Month
                : (DateTime.Now.Month - 1) / 3 + 1;

            cmbKy.Items.Clear();

            for (int i = 1; i <= soMuc; i++)
                cmbKy.Items.Add(i);

            cmbKy.SelectedIndex = mucHienTai - 1;
        }

        // ===================== XEM BÁO CÁO =====================

        private void btnXem_Click(object sender, EventArgs e)
        {
            XemBaoCao(baoKhiTrong: true);

            if (_dt == null || _dt.Rows.Count == 0)
                return;

            using (frmXemBaoCaoDoanhThu frm =
                new frmXemBaoCaoDoanhThu(_dt, _tieuDe, _tong))
            {
                frm.ShowDialog(this);
            }
        }

        private void XemBaoCao(bool baoKhiTrong)
        {
            var kieu = LayKieu();
            int nam = (int)nudNam.Value;

            int ky = (kieu != KieuThongKe.Nam &&
                      cmbKy.SelectedItem is int k)
                ? k
                : 0;

            DataTable dt = null;
            decimal tong = 0;
            string tieuDe = "";

            bool thanhCong = Ui.Run(() =>
            {
                dt = _bll.DoanhThuDauBep(kieu, nam, ky);
                tong = _bll.TongDoanhThu(dt);
                tieuDe = _bll.TieuDe(kieu, nam, ky);
            });

            if (!thanhCong) return;

            _dt = dt;
            _tong = tong;
            _tieuDe = tieuDe;

            dgv.DataSource = dt;

            lblTieuDeBC.Text = _tieuDe;
            lblSoDong.Text = "Số đầu bếp: " + dt.Rows.Count;
            lblTong.Text = "Tổng doanh thu: " + _tong.ToString("N0");

            if (baoKhiTrong && _tong == 0)
                Ui.Info("Không có doanh thu nào trong kỳ này.");
        }

        private bool CoDuLieu()
        {
            if (_dt != null && _dt.Rows.Count > 0)
                return true;

            Ui.Error("Chưa có dữ liệu. Hãy bấm \"Xem báo cáo\" trước.");
            return false;
        }

        // ===================== XUẤT FILE EXCEL =====================

        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (!CoDuLieu()) return;

            using var dlg = new SaveFileDialog
            {
                Title = "Xuất báo cáo",
                Filter = "File Excel (*.xlsx)|*.xlsx",
                FileName = $"DoanhThuDauBep_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            bool ok = Ui.Run(() =>
            {
                using var wb = new XLWorkbook();

                var ws = wb.Worksheets.Add("Doanh thu");

                ws.Cell(1, 1).Value = _tieuDe;

                var tieuDe = ws.Range(1, 1, 1, 4);
                tieuDe.Merge();
                tieuDe.Style.Font.Bold = true;
                tieuDe.Style.Font.FontSize = 14;
                tieuDe.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                string[] cot =
                {
                    "Mã đầu bếp",
                    "Tên đầu bếp",
                    "Số phần",
                    "Doanh thu"
                };

                for (int i = 0; i < cot.Length; i++)
                    ws.Cell(3, i + 1).Value = cot[i];

                var hdr = ws.Range(3, 1, 3, 4);
                hdr.Style.Font.Bold = true;
                hdr.Style.Fill.BackgroundColor =
                    XLColor.FromHtml("#E8DCCD");

                int dong = 4;

                foreach (DataRow r in _dt.Rows)
                {
                    ws.Cell(dong, 1).Value =
                        Convert.ToString(r["MaDauBep"]);

                    ws.Cell(dong, 2).Value =
                        Convert.ToString(r["TenDauBep"]);

                    ws.Cell(dong, 3).Value =
                        Convert.ToInt32(r["SoPhan"]);

                    ws.Cell(dong, 4).Value =
                        Convert.ToDecimal(r["DoanhThu"]);

                    dong++;
                }

                ws.Cell(dong, 2).Value = "Tổng cộng";
                ws.Cell(dong, 3).FormulaA1 =
                    $"SUM(C4:C{dong - 1})";

                ws.Cell(dong, 4).FormulaA1 =
                    $"SUM(D4:D{dong - 1})";

                ws.Range(dong, 1, dong, 4).Style.Font.Bold = true;

                ws.Range(4, 3, dong, 4)
                    .Style.NumberFormat.Format = "#,##0";

                var bang = ws.Range(3, 1, dong, 4);

                bang.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                bang.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                ws.Columns().AdjustToContents(3, dong);

                wb.SaveAs(dlg.FileName);
            });

            if (ok && Ui.Confirm("Đã xuất file thành công. Mở file ngay?"))
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(dlg.FileName)
                    {
                        UseShellExecute = true
                    });
            }
        }

        // ===================== IN BÁO CÁO =====================

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (!CoDuLieu()) return;

            _dongDaIn = 0;

            using var doc = new PrintDocument
            {
                DocumentName = "Doanh thu đầu bếp"
            };

            doc.PrintPage += InTrang;

            using var xem = new PrintPreviewDialog
            {
                Document = doc,
                Width = 900,
                Height = 700
            };

            xem.ShowDialog(this);
        }

        private void InTrang(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var vung = e.MarginBounds;

            using var fTieuDe = new Font("Segoe UI", 14, FontStyle.Bold);
            using var fDam = new Font("Segoe UI", 10, FontStyle.Bold);
            using var fThuong = new Font("Segoe UI", 10);

            using var giua = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using var trai = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            using var phai = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            const float cao = 26f;

            float wMa = 100;
            float wPhan = 90;
            float wTien = 150;
            float wTen = vung.Width - wMa - wPhan - wTien;

            float xMa = vung.Left;
            float xTen = xMa + wMa;
            float xPhan = xTen + wTen;
            float xTien = xPhan + wPhan;

            float y = vung.Top;

            if (_dongDaIn == 0)
            {
                g.DrawString(
                    _tieuDe,
                    fTieuDe,
                    Brushes.Black,
                    new RectangleF(vung.Left, y, vung.Width, 36),
                    giua);

                y += 56;
            }

            g.FillRectangle(
                Brushes.Gainsboro,
                vung.Left,
                y,
                vung.Width,
                cao);

            g.DrawString(
                "Mã đầu bếp",
                fDam,
                Brushes.Black,
                new RectangleF(xMa + 4, y, wMa, cao),
                trai);

            g.DrawString(
                "Tên đầu bếp",
                fDam,
                Brushes.Black,
                new RectangleF(xTen + 4, y, wTen, cao),
                trai);

            g.DrawString(
                "Số phần",
                fDam,
                Brushes.Black,
                new RectangleF(xPhan, y, wPhan - 4, cao),
                phai);

            g.DrawString(
                "Doanh thu",
                fDam,
                Brushes.Black,
                new RectangleF(xTien, y, wTien - 4, cao),
                phai);

            y += cao;

            while (_dongDaIn < _dt.Rows.Count)
            {
                if (y + cao > vung.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                var r = _dt.Rows[_dongDaIn];

                g.DrawString(
                    Convert.ToString(r["MaDauBep"]),
                    fThuong,
                    Brushes.Black,
                    new RectangleF(xMa + 4, y, wMa, cao),
                    trai);

                g.DrawString(
                    Convert.ToString(r["TenDauBep"]),
                    fThuong,
                    Brushes.Black,
                    new RectangleF(xTen + 4, y, wTen, cao),
                    trai);

                g.DrawString(
                    Convert.ToDecimal(r["SoPhan"]).ToString("N0"),
                    fThuong,
                    Brushes.Black,
                    new RectangleF(xPhan, y, wPhan - 4, cao),
                    phai);

                g.DrawString(
                    Convert.ToDecimal(r["DoanhThu"]).ToString("N0"),
                    fThuong,
                    Brushes.Black,
                    new RectangleF(xTien, y, wTien - 4, cao),
                    phai);

                g.DrawLine(
                    Pens.LightGray,
                    vung.Left,
                    y + cao,
                    vung.Right,
                    y + cao);

                y += cao;
                _dongDaIn++;
            }

            if (y + cao > vung.Bottom)
            {
                e.HasMorePages = true;
                return;
            }

            g.DrawString(
                "Tổng doanh thu",
                fDam,
                Brushes.Black,
                new RectangleF(xTen + 4, y, wTen, cao),
                trai);

            g.DrawString(
                _tong.ToString("N0"),
                fDam,
                Brushes.Black,
                new RectangleF(xTien, y, wTien - 4, cao),
                phai);

            g.DrawLine(
                Pens.Black,
                vung.Left,
                y,
                vung.Right,
                y);

            e.HasMorePages = false;
        }

        // ===================== GIAO DIỆN =====================

        private void ApDungGiaoDien()
        {
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F);

            nudNam.BackColor = Ui.Surface;
            nudNam.ForeColor = Ui.Text;

            cmbKy.BackColor = Ui.Surface;
            cmbKy.ForeColor = Ui.Text;
            cmbKy.FlatStyle = FlatStyle.Flat;

            rdoThang.ForeColor = Ui.Dark;
            rdoQuy.ForeColor = Ui.Dark;
            rdoNam.ForeColor = Ui.Dark;

            lblKy.ForeColor = Ui.Dark;

            Button[] dsNut =
            {
                btnXem,
                btnXuat,
                btnIn
            };

            foreach (Button b in dsNut)
            {
                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = Ui.Border;

                b.BackColor = Ui.Surface;
                b.ForeColor = Ui.Dark;

                b.FlatAppearance.MouseOverBackColor = Ui.Hover;
                b.FlatAppearance.MouseDownBackColor = Ui.Header;

                b.Cursor = Cursors.Hand;
            }

            btnXem.BackColor = Ui.Accent;
            btnXem.ForeColor = Color.White;
            btnXem.FlatAppearance.BorderColor = Ui.Accent;

            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Ui.Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Ui.Border;

            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Ui.Dark;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Ui.Dark;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgv.ColumnHeadersHeight = 36;

            dgv.DefaultCellStyle.BackColor = Ui.Surface;
            dgv.DefaultCellStyle.ForeColor = Ui.Text;
            dgv.DefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Ui.Text;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.RowTemplate.Height = 32;

            lblTieuDeBC.ForeColor = Ui.Dark;
            lblTieuDeBC.Font =
                new Font("Segoe UI", 12F, FontStyle.Bold);

            lblSoDong.ForeColor = Ui.Text;

            lblTong.ForeColor = Ui.Accent;
            lblTong.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}
