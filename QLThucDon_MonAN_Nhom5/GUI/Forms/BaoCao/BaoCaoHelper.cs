
using ClosedXML.Excel;
using QLThucDon_MonAN_Nhom5.GUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Mô tả một cột của báo cáo:
    /// dùng chung cho lưới, file Excel và bản in.
    /// </summary>
    public class CotBaoCao
    {
        public string Ten { get; }
        public string TieuDe { get; }
        public float TrongSo { get; }
        public bool Phai { get; }
        public string DinhDang { get; }
        public int Rong { get; }

        public CotBaoCao(
            string ten,
            string tieuDe,
            float trongSo = 1f,
            bool phai = false,
            string dinhDang = null,
            int rong = 0)
        {
            Ten = ten;
            TieuDe = tieuDe;
            TrongSo = trongSo;
            Phai = phai;
            DinhDang = dinhDang;
            Rong = rong;
        }
    }

    /// <summary>
    /// Một mục của ComboBox bộ lọc (mã + chữ hiển thị).
    /// </summary>
    public class MucLoc
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
    }

    /// <summary>
    /// Các hàm dùng chung cho mọi form báo cáo.
    /// </summary>
    public static class BaoCaoHelper
    {
        // ===================== KIỂM TRA DỮ LIỆU =====================

        /// <summary>
        /// Kiểm tra DataTable có dữ liệu hay không.
        /// </summary>
        public static bool CoDuLieu(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return true;

            Ui.Error(
                "Chưa có dữ liệu để xuất hoặc in. Hãy bấm \"Xem\" trước.");

            return false;
        }

        /// <summary>
        /// Kiểm tra các cột báo cáo có tồn tại trong DataTable.
        /// </summary>
        private static bool KiemTraCot(DataTable dt, CotBaoCao[] cot)
        {
            if (dt == null)
            {
                Ui.Error("Không có dữ liệu báo cáo.");
                return false;
            }

            if (cot == null || cot.Length == 0)
            {
                Ui.Error("Báo cáo chưa được cấu hình cột dữ liệu.");
                return false;
            }

            foreach (var c in cot)
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Ten))
                {
                    Ui.Error("Cấu hình cột báo cáo không hợp lệ.");
                    return false;
                }

                if (!dt.Columns.Contains(c.Ten))
                {
                    Ui.Error(
                        "Không tìm thấy cột dữ liệu \""
                        + c.Ten
                        + "\" trong báo cáo.");

                    return false;
                }
            }

            return true;
        }

        // ===================== CẤU HÌNH LƯỚI =====================

        /// <summary>
        /// Gắn các cột vào lưới.
        /// Nếu số cột trong Designer khác số cột báo cáo thì tạo lại.
        /// </summary>
        public static void CauHinhLuoi(
            DataGridView dgv,
            CotBaoCao[] cot)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (cot == null || cot.Length == 0)
                throw new ArgumentException(
                    "Danh sách cột báo cáo không hợp lệ.",
                    nameof(cot));

            dgv.AutoGenerateColumns = false;
            dgv.ReadOnly = true;

            if (dgv.Columns.Count != cot.Length)
            {
                dgv.Columns.Clear();

                foreach (var c in cot)
                {
                    dgv.Columns.Add(
                        new DataGridViewTextBoxColumn
                        {
                            Name = c.Ten
                        });
                }
            }

            for (int i = 0; i < cot.Length; i++)
            {
                var c = cot[i];
                var col = dgv.Columns[i];

                col.Name = c.Ten;
                col.DataPropertyName = c.Ten;
                col.HeaderText = c.TieuDe;

                if (c.Rong > 0)
                {
                    col.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;

                    col.Width = c.Rong;
                }
                else
                {
                    col.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;

                    col.FillWeight = c.TrongSo * 100f;
                    col.MinimumWidth = 120;
                }

                if (c.Phai)
                {
                    col.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    col.HeaderCell.Style.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
                else
                {
                    col.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    col.HeaderCell.Style.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;
                }

                if (!string.IsNullOrEmpty(c.DinhDang))
                    col.DefaultCellStyle.Format = c.DinhDang;
            }
        }

        // ===================== BỘ LỌC =====================

        /// <summary>
        /// Nạp ComboBox bộ lọc:
        /// dòng đầu là "Tất cả..." (mã rỗng).
        /// </summary>
        public static void NapLoc(
            ComboBox cmb,
            string chuTatCa,
            IEnumerable<MucLoc> ds)
        {
            if (cmb == null)
                throw new ArgumentNullException(nameof(cmb));

            var list = new List<MucLoc>
            {
                new MucLoc
                {
                    Ma = "",
                    Ten = chuTatCa
                }
            };

            if (ds != null)
                list.AddRange(ds.Where(x => x != null));

            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.DisplayMember = nameof(MucLoc.Ten);
            cmb.ValueMember = nameof(MucLoc.Ma);
            cmb.DataSource = list;
            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// Mã đang chọn trong bộ lọc; "" nghĩa là "Tất cả".
        /// </summary>
        public static string LayMaLoc(ComboBox cmb)
        {
            if (cmb == null || cmb.SelectedValue == null)
                return "";

            return cmb.SelectedValue as string ?? "";
        }

        // ===================== ĐỊNH DẠNG GIÁ TRỊ =====================

        private static string DinhDangGiaTri(
            object v,
            CotBaoCao c)
        {
            if (v == null || v == DBNull.Value)
                return "";

            if (v is IFormattable f
                && !string.IsNullOrEmpty(c.DinhDang))
            {
                return f.ToString(
                    c.DinhDang,
                    CultureInfo.CurrentCulture);
            }

            return Convert.ToString(v);
        }

        // ===================== XUẤT EXCEL =====================

        /// <summary>
        /// Xuất báo cáo ra file Excel (.xlsx) bằng ClosedXML.
        /// </summary>
        public static void XuatExcel(
            IWin32Window owner,
            DataTable dt,
            string tieuDe,
            CotBaoCao[] cot,
            string tenFileGoc)
        {
            if (!CoDuLieu(dt))
                return;

            if (!KiemTraCot(dt, cot))
                return;

            if (string.IsNullOrWhiteSpace(tenFileGoc))
            {
                Ui.Error("Tên file xuất báo cáo không hợp lệ.");
                return;
            }

            string duongDan = null;

            using (var dlg = new SaveFileDialog
            {
                Title = "Xuất báo cáo",
                Filter = "File Excel (*.xlsx)|*.xlsx",
                FileName = string.Format(
                    "{0}_{1:yyyyMMdd_HHmm}.xlsx",
                    tenFileGoc,
                    DateTime.Now)
            })
            {
                if (dlg.ShowDialog(owner) != DialogResult.OK)
                    return;

                duongDan = dlg.FileName;
            }

            bool ok = Ui.Run(() =>
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Báo cáo");

                    // Dòng 1: tiêu đề báo cáo.
                    ws.Cell(1, 1).Value = tieuDe;

                    var rTieuDe = ws.Range(
                        1, 1, 1, cot.Length);

                    rTieuDe.Merge();
                    rTieuDe.Style.Font.Bold = true;
                    rTieuDe.Style.Font.FontSize = 14;

                    rTieuDe.Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                    // Dòng 3: tiêu đề cột.
                    for (int i = 0; i < cot.Length; i++)
                    {
                        ws.Cell(3, i + 1).Value = cot[i].TieuDe;
                    }

                    var hdr = ws.Range(
                        3, 1, 3, cot.Length);

                    hdr.Style.Font.Bold = true;

                    hdr.Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#E8DCCD");

                    // Từ dòng 4: dữ liệu.
                    int dong = 4;

                    foreach (DataRow r in dt.Rows)
                    {
                        for (int i = 0; i < cot.Length; i++)
                        {
                            object v = r[cot[i].Ten];

                            if (v == null || v == DBNull.Value)
                                continue;

                            var cell = ws.Cell(dong, i + 1);

                            switch (v)
                            {
                                case DateTime d:
                                    cell.Value = d;
                                    break;

                                case decimal m:
                                    cell.Value = m;
                                    break;

                                case int n:
                                    cell.Value = n;
                                    break;

                                case long l:
                                    cell.Value = l;
                                    break;

                                case double db:
                                    cell.Value = db;
                                    break;

                                case float f:
                                    cell.Value = f;
                                    break;

                                case short s:
                                    cell.Value = s;
                                    break;

                                case bool b:
                                    cell.Value = b;
                                    break;

                                default:
                                    cell.Value = Convert.ToString(v);
                                    break;
                            }
                        }

                        dong++;
                    }

                    // Định dạng từng cột.
                    for (int i = 0; i < cot.Length; i++)
                    {
                        var vung = ws.Range(
                            4,
                            i + 1,
                            dong - 1,
                            i + 1);

                        if (!string.IsNullOrEmpty(cot[i].DinhDang))
                        {
                            vung.Style.NumberFormat.Format =
                                cot[i].DinhDang == "N0"
                                    ? "#,##0"
                                    : cot[i].DinhDang;
                        }

                        if (cot[i].Phai)
                        {
                            vung.Style.Alignment.Horizontal =
                                XLAlignmentHorizontalValues.Right;
                        }
                    }

                    // Kẻ khung bảng.
                    var bang = ws.Range(
                        3,
                        1,
                        dong - 1,
                        cot.Length);

                    bang.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;

                    bang.Style.Border.InsideBorder =
                        XLBorderStyleValues.Thin;

                    // Tự căn độ rộng cột.
                    ws.Columns().AdjustToContents(3, dong - 1);

                    foreach (var c in ws.ColumnsUsed())
                    {
                        if (c.Width > 60)
                            c.Width = 60;
                    }

                    // Giữ tiêu đề khi cuộn.
                    ws.SheetView.FreezeRows(3);

                    wb.SaveAs(duongDan);
                }
            });

            if (!ok)
                return;

            if (Ui.Confirm("Đã xuất file thành công. Mở file ngay?"))
            {
                Ui.Run(() =>
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(duongDan)
                        {
                            UseShellExecute = true
                        });
                });
            }
        }

        // ===================== IN BÁO CÁO =====================

        /// <summary>
        /// Mở cửa sổ xem trước khi in báo cáo.
        /// </summary>
        public static void In(
            IWin32Window owner,
            DataTable dt,
            string tieuDe,
            CotBaoCao[] cot)
        {
            if (!CoDuLieu(dt))
                return;

            if (!KiemTraCot(dt, cot))
                return;

            Ui.Run(() =>
            {
                int dong = 0;

                using (var doc = new PrintDocument())
                {
                    doc.DocumentName = tieuDe;

                    // Báo cáo nhiều cột thì in ngang.
                    doc.DefaultPageSettings.Landscape =
                        cot.Length > 6;

                    doc.PrintPage += (s, e) =>
                    {
                        dong = VeTrang(
                            e,
                            dt,
                            tieuDe,
                            cot,
                            dong);
                    };

                    using (var xem = new PrintPreviewDialog())
                    {
                        xem.Document = doc;
                        xem.Width = 1000;
                        xem.Height = 720;

                        xem.ShowDialog(owner);
                    }
                }
            });
        }

        /// <summary>
        /// Vẽ một trang, trả về chỉ số dòng tiếp theo cần in.
        /// </summary>
        private static int VeTrang(
            PrintPageEventArgs e,
            DataTable dt,
            string tieuDe,
            CotBaoCao[] cot,
            int dong)
        {
            var g = e.Graphics;
            var vung = e.MarginBounds;

            float co = cot.Length > 6 ? 8.5f : 10f;

            using (var fTieuDe =
                new Font("Segoe UI", 14, FontStyle.Bold))
            using (var fDam =
                new Font("Segoe UI", co, FontStyle.Bold))
            using (var fThuong =
                new Font("Segoe UI", co))
            using (var giua = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using (var trai = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            using (var phai = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                const float cao = 24f;

                float y = vung.Top;

                // Chia chiều rộng trang theo trọng số cột.
                float tong = cot.Sum(c => c.TrongSo);

                var x = new float[cot.Length];
                var w = new float[cot.Length];

                float xt = vung.Left;

                for (int i = 0; i < cot.Length; i++)
                {
                    w[i] = vung.Width * cot[i].TrongSo / tong;
                    x[i] = xt;
                    xt += w[i];
                }

                // Tiêu đề chỉ hiển thị ở trang đầu.
                if (dong == 0)
                {
                    g.DrawString(
                        tieuDe,
                        fTieuDe,
                        Brushes.Black,
                        new RectangleF(
                            vung.Left,
                            y,
                            vung.Width,
                            36),
                        giua);

                    y += 50;
                }

                // Tiêu đề cột lặp lại ở mỗi trang.
                g.FillRectangle(
                    Brushes.Gainsboro,
                    vung.Left,
                    y,
                    vung.Width,
                    cao);

                for (int i = 0; i < cot.Length; i++)
                {
                    g.DrawString(
                        cot[i].TieuDe,
                        fDam,
                        Brushes.Black,
                        new RectangleF(
                            x[i] + 3,
                            y,
                            w[i] - 6,
                            cao),
                        cot[i].Phai ? phai : trai);
                }

                y += cao;

                // In dữ liệu.
                while (dong < dt.Rows.Count)
                {
                    if (y + cao > vung.Bottom)
                    {
                        e.HasMorePages = true;
                        return dong;
                    }

                    var r = dt.Rows[dong];

                    for (int i = 0; i < cot.Length; i++)
                    {
                        g.DrawString(
                            DinhDangGiaTri(
                                r[cot[i].Ten],
                                cot[i]),
                            fThuong,
                            Brushes.Black,
                            new RectangleF(
                                x[i] + 3,
                                y,
                                w[i] - 6,
                                cao),
                            cot[i].Phai ? phai : trai);
                    }

                    g.DrawLine(
                        Pens.LightGray,
                        vung.Left,
                        y + cao,
                        vung.Right,
                        y + cao);

                    y += cao;
                    dong++;
                }

                e.HasMorePages = false;

                return dong;
            }
        }
    }
}
