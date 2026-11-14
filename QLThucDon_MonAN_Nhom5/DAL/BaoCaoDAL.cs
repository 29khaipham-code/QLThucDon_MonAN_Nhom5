using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.Interfaces;
namespace QLThucDon_MonAN_Nhom5.DAL
{
    //Các truy vấn báo cáo trả về DataTable để gắn thẳng vào DataGridView / RDLC
    public class BaoCaoDAL : IBaoCaoDAL
    {

        // ---------- Yêu cầu 5: doanh thu từng đầu bếp theo tháng / quý / năm ----------
        // Doanh thu = tổng ThanhTien các món đầu bếp thực hiện (trước VAT và giảm giá). Đầu bếp không có doanh thu vẫn hiện dòng 0.
        // ---------- Doanh thu đầu bếp: sử dụng Stored Procedure ----------

        public DataTable DoanhThuTheoThang(int nam, int thang)
            => DoanhThu(nam, thang, null);

        public DataTable DoanhThuTheoQuy(int nam, int quy)
            => DoanhThu(nam, null, quy);

        public DataTable DoanhThuTheoNam(int nam)
            => DoanhThu(nam, null, null);

        private static DataTable DoanhThu(int nam, int? thang, int? quy)
        {
            return DbHelper.ExecuteProcTable(
                "sp_DoanhThuDauBep",
                DbHelper.P("@Nam", nam),
                DbHelper.P("@Thang", thang),
                DbHelper.P("@Quy", quy)
            );
        }

        // ---------- Yêu cầu 6: thông tin đầy đủ các món ăn (kèm nguyên liệu) ----------
        // ---------- Yêu cầu 6: thông tin đầy đủ các món ăn ----------
        public DataTable MonAnDayDu()
        {
            return DbHelper.ExecuteProcTable("sp_MonAnDayDu");
        }

        // ---------- Yêu cầu 7: danh sách món ăn của từng đầu bếp ----------
        // ---------- Yêu cầu 7: danh sách món ăn của từng đầu bếp ----------
        public DataTable MonAnTheoDauBep(string maDauBep = null)
        {
            return DbHelper.ExecuteProcTable(
                "sp_MonAnTheoDauBep",
                DbHelper.P("@Bep", DbHelper.NullIfEmpty(maDauBep))
            );
        }

        // ---------- Yêu cầu 8: thông tin đầy đủ các thực đơn (null = tất cả) ----------
        // ---------- Yêu cầu 8: thông tin đầy đủ các thực đơn ----------
        public DataTable ThucDonDayDu(string soThucDon = null)
        {
            return DbHelper.ExecuteProcTable(
                "sp_ThucDonDayDu",
                DbHelper.P("@So", DbHelper.NullIfEmpty(soThucDon))
            );
        }
    }

    }
