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
    /// <summary>Mô tả một cột của báo cáo: dùng chung cho lưới, file Excel và bản in.</summary>
    public class CotBaoCao
    {
        public string Ten { get; }          // tên cột trong DataTable (trùng tên cột SQL)
        public string TieuDe { get; }       // chữ hiện ở đầu cột
        public float TrongSo { get; }       // độ rộng tương đối khi in và khi cột tự giãn
        public bool Phai { get; }           // true: căn phải (cột số)
        public string DinhDang { get; }     // "N0" (có dấu phân cách nghìn), "dd/MM/yyyy", "0.##"...
        public int Rong { get; }            // độ rộng cố định trên lưới (px); 0 = cột tự giãn (Fill)

        public CotBaoCao(string ten, string tieuDe, float trongSo = 1f, bool phai = false, string dinhDang = null, int rong = 0)
        {
            Ten = ten; TieuDe = tieuDe; TrongSo = trongSo; Phai = phai; DinhDang = dinhDang; Rong = rong;
        }
    }

    /// <summary>Một mục của ComboBox bộ lọc (mã + chữ hiển thị).</summary>
    public class MucLoc
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
    }

    /// <summary>Các hàm dùng chung cho mọi form báo cáo thông tin.</summary>
    public static class BaoCaoHelper
    {
        // ===================== LƯỚI =====================
        /// <summary>Gắn các cột vào lưới. Nếu số cột trong Designer khác số cột báo cáo thì xoá và tạo lại.</summary>
        public static void CauHinhLuoi(DataGridView dgv, CotBaoCao[] cot)
        {
            dgv.AutoGenerateColumns = false;
            dgv.ReadOnly = true;

            if (dgv.Columns.Count != cot.Length)
            {
                dgv.Columns.Clear();
                foreach (var c in cot)
                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = c.Ten });
            }

            for (int i = 0; i < cot.Length; i++)
            {
                var c = cot[i];
                var col = dgv.Columns[i];
                col.DataPropertyName = c.Ten;
                col.HeaderText = c.TieuDe;

                if (c.Rong > 0)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = c.Rong;
                }
                else
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    col.FillWeight = c.TrongSo * 100f;
                    col.MinimumWidth = 120;
                }

                if (c.Phai)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (!string.IsNullOrEmpty(c.DinhDang)) col.DefaultCellStyle.Format = c.DinhDang;
            }
        }

        // ===================== BỘ LỌC =====================
        /// <summary>Nạp ComboBox bộ lọc: dòng đầu là "Tất cả..." (mã rỗng), sau đó là các mục.</summary>
        public static void NapLoc(ComboBox cmb, string chuTatCa, IEnumerable<MucLoc> ds)
        {
            var list = new List<MucLoc> { new MucLoc { Ma = "", Ten = chuTatCa } };
            list.AddRange(ds);
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.DisplayMember = nameof(MucLoc.Ten);
            cmb.ValueMember = nameof(MucLoc.Ma);
            cmb.DataSource = list;
            cmb.SelectedIndex = 0;
        }

        /// <summary>Mã đang chọn trong bộ lọc; "" nghĩa là "Tất cả".</summary>
        public static string LayMaLoc(ComboBox cmb) => cmb.SelectedValue as string ?? "";

        public static bool CoDuLieu(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0) return true;
            Ui.Error("Chưa có dữ liệu để xuất hoặc in. Hãy bấm \"Xem\" trước.");
            return false;
        }

        private static string DinhDangGiaTri(object v, CotBaoCao c)
        {
            if (v == null || v == DBNull.Value) return "";
            if (v is IFormattable f && !string.IsNullOrEmpty(c.DinhDang))
                return f.ToString(c.DinhDang, CultureInfo.CurrentCulture);
            return Convert.ToString(v);
        }

        // ===================== XUẤT EXCEL (.xlsx, dùng ClosedXML) =====================
        public static void XuatExcel(IWin32Window owner, DataTable dt, string tieuDe, CotBaoCao[] cot, string tenFileGoc)
        {
            if (!CoDuLieu(dt)) return;

            using var dlg = new SaveFileDialog
            {
                Title = "Xuất báo cáo",
                Filter = "File Excel (*.xlsx)|*.xlsx",
                FileName = $"{tenFileGoc}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };
            if (dlg.ShowDialog(owner) != DialogResult.OK) return;

            bool ok = Ui.Run(() =>
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Báo cáo");

                // Dòng 1: tiêu đề, gộp qua tất cả các cột
                ws.Cell(1, 1).Value = tieuDe;
                var rTieuDe = ws.Range(1, 1, 1, cot.Length);
                rTieuDe.Merge();
                rTieuDe.Style.Font.Bold = true;
                rTieuDe.Style.Font.FontSize = 14;
                rTieuDe.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Dòng 3: tiêu đề cột
                for (int i = 0; i < cot.Length; i++) ws.Cell(3, i + 1).Value = cot[i].TieuDe;
                var hdr = ws.Range(3, 1, 3, cot.Length);
                hdr.Style.Font.Bold = true;
                hdr.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8DCCD");

                // Từ dòng 4: dữ liệu. Ghi đúng kiểu (số, ngày, chữ) để Excel còn sắp xếp và tính toán được
                int dong = 4;
                foreach (DataRow r in dt.Rows)
                {
                    for (int i = 0; i < cot.Length; i++)
                    {
                        object v = r[cot[i].Ten];
                        if (v == null || v == DBNull.Value) continue;

                        var cell = ws.Cell(dong, i + 1);
                        switch (v)
                        {
                            case DateTime d: cell.Value = d; break;
                            case decimal m: cell.Value = m; break;
                            case int n: cell.Value = n; break;
                            case long l: cell.Value = l; break;
                            case double db: cell.Value = db; break;
                            default: cell.Value = Convert.ToString(v); break;
                        }
                    }
                    dong++;
                }

                // Định dạng từng cột
                for (int i = 0; i < cot.Length; i++)
                {
                    var vung = ws.Range(4, i + 1, dong - 1, i + 1);
                    if (!string.IsNullOrEmpty(cot[i].DinhDang))
                        vung.Style.NumberFormat.Format = cot[i].DinhDang == "N0" ? "#,##0" : cot[i].DinhDang;
                    if (cot[i].Phai) vung.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                var bang = ws.Range(3, 1, dong - 1, cot.Length);
                bang.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                bang.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                ws.Columns().AdjustToContents(3, dong - 1);
                foreach (var c in ws.ColumnsUsed())
                    if (c.Width > 60) c.Width = 60;        // cột chữ dài (cách làm, nguyên liệu) không giãn quá rộng
                ws.SheetView.FreezeRows(3);                // giữ dòng tiêu đề khi cuộn

                wb.SaveAs(dlg.FileName);
            });

            if (ok && Ui.Confirm("Đã xuất file thành công. Mở file ngay?"))
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
        }

        // ===================== IN =====================
        public static void In(IWin32Window owner, DataTable dt, string tieuDe, CotBaoCao[] cot)
        {
            if (!CoDuLieu(dt)) return;

            int dong = 0;   // dòng đang in dở, giữ lại giữa các trang
            using var doc = new PrintDocument { DocumentName = tieuDe };
            doc.DefaultPageSettings.Landscape = cot.Length > 6;   // báo cáo nhiều cột thì in ngang
            doc.PrintPage += (s, e) => dong = VeTrang(e, dt, tieuDe, cot, dong);

            using var xem = new PrintPreviewDialog { Document = doc, Width = 1000, Height = 720 };
            xem.ShowDialog(owner);
        }

        /// <summary>Vẽ một trang, trả về chỉ số dòng tiếp theo cần in.</summary>
        private static int VeTrang(PrintPageEventArgs e, DataTable dt, string tieuDe, CotBaoCao[] cot, int dong)
        {
            var g = e.Graphics;
            var vung = e.MarginBounds;
            float co = cot.Length > 6 ? 8.5f : 10f;

            using var fTieuDe = new Font("Segoe UI", 14, FontStyle.Bold);
            using var fDam = new Font("Segoe UI", co, FontStyle.Bold);
            using var fThuong = new Font("Segoe UI", co);
            using var giua = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var trai = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
            using var phai = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };

            const float cao = 24f;
            float y = vung.Top;

            // Chia chiều rộng trang theo trọng số của từng cột
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

            // Tiêu đề chỉ ở trang đầu
            if (dong == 0)
            {
                g.DrawString(tieuDe, fTieuDe, Brushes.Black, new RectangleF(vung.Left, y, vung.Width, 36), giua);
                y += 50;
            }

            // Dòng tiêu đề cột (lặp lại ở mỗi trang)
            g.FillRectangle(Brushes.Gainsboro, vung.Left, y, vung.Width, cao);
            for (int i = 0; i < cot.Length; i++)
                g.DrawString(cot[i].TieuDe, fDam, Brushes.Black,
                    new RectangleF(x[i] + 3, y, w[i] - 6, cao), cot[i].Phai ? phai : trai);
            y += cao;

            // Các dòng dữ liệu; hết chỗ thì sang trang mới
            while (dong < dt.Rows.Count)
            {
                if (y + cao > vung.Bottom) { e.HasMorePages = true; return dong; }

                var r = dt.Rows[dong];
                for (int i = 0; i < cot.Length; i++)
                    g.DrawString(DinhDangGiaTri(r[cot[i].Ten], cot[i]), fThuong, Brushes.Black,
                        new RectangleF(x[i] + 3, y, w[i] - 6, cao), cot[i].Phai ? phai : trai);
                g.DrawLine(Pens.LightGray, vung.Left, y + cao, vung.Right, y + cao);
                y += cao;
                dong++;
            }

            e.HasMorePages = false;
            return dong;
        }
    }
}