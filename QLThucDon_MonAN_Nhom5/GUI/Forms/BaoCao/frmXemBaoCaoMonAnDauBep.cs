
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    public partial class frmXemBaoCaoMonAnDauBep : Form
    {
        private ReportDocument _report;

        public frmXemBaoCaoMonAnDauBep(DataTable duLieu, string tieuDe)
        {
            InitializeComponent();

            Text = tieuDe;
            Width = 1100;
            Height = 700;

            try
            {
                if (duLieu == null || duLieu.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để hiển thị báo cáo!");
                    return;
                }

                string duongDan = Path.Combine(
                    Application.StartupPath,
                    "Reports",
                    "crptMonAnDauBep.rpt"
                );

                if (!File.Exists(duongDan))
                {
                    MessageBox.Show("Không tìm thấy file báo cáo:\n" + duongDan);
                    return;
                }

                DataTable dt = duLieu.Copy();
                dt.TableName = "tblMonAnDauBep";

                _report = new ReportDocument();
                _report.Load(duongDan);

                _report.SetDataSource(dt);

                // Sử dụng Viewer đã có trong Designer
                crystalReportViewer1.ReportSource = _report;
                crystalReportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị báo cáo:\n\n" + ex.Message,
                    "Lỗi báo cáo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            crystalReportViewer1.ReportSource = null;

            if (_report != null)
            {
                _report.Close();
                _report.Dispose();
                _report = null;
            }

            base.OnFormClosed(e);
        }
    }
}
