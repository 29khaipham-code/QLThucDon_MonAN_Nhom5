using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace QLThucDon_MonAN_Nhom5.DAL
{
   
    /// Lớp cha trừu tượng cho các DAL có khoá là một cột mã (KH001, MA001...).
    /// Chứa sẵn GetAll / GetById / NextCode (Template Method); lớp con chỉ khai báo
    /// tên bảng, câu SELECT, cách ánh xạ dòng -> DTO và các hàm ghi riêng. 
    public abstract class BaseDAL<T> : IReadRepository<T>
    {
        protected abstract string TableName { get; }          // vd "KhachHang"
        protected abstract string KeyColumn { get; }          // vd "MaKhachHang"
        protected abstract string CodePrefix { get; }         // vd "KH"
        protected virtual int CodeWidth => 3;                 // số chữ số của mã
        protected abstract string SelectSql { get; }          // SELECT ... FROM ... (chưa có WHERE/ORDER)
        protected abstract string OrderBySql { get; }         // vd "MaKhachHang"
        protected virtual string KeyColumnInSelect => KeyColumn;   // vd "m.MaMonAn" khi SELECT có bí danh bảng

        protected abstract T Map(SqlDataReader r);

        public virtual List<T> GetAll() =>
            DbHelper.ExecuteList<T>(SelectSql + " ORDER BY " + OrderBySql, Map);

        public virtual T GetById(string ma) =>
            DbHelper.ExecuteList<T>(SelectSql + " WHERE " + KeyColumnInSelect + "=@Ma", Map,
                DbHelper.P("@Ma", ma)).FirstOrDefault();

        public virtual string NextCode() =>
            DbHelper.NextCode(TableName, KeyColumn, CodePrefix, CodeWidth);
    }
}
