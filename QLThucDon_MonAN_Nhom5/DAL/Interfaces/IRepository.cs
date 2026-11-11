using System.Collections.Generic;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    /// <summary>Interface 1: chỉ đọc - lấy tất cả, lấy theo mã, sinh mã mới.</summary>
    public interface IReadRepository<T>
    {
        List<T> GetAll();
        T GetById(string ma);
        string NextCode();
    }

    /// <summary>Interface 2: CRUD cho bảng đơn (KhachHang, NguyenLieu).</summary>
    public interface IRepository<T> : IReadRepository<T>
    {
        int Insert(T item);
        int Update(T item);
        int Delete(string ma);
    }

    /// <summary>Interface 3: CRUD cho đối tượng có danh sách chi tiết đi kèm
    /// (MonAn + nguyên liệu, DauBep + món làm được, ThucDon + chi tiết thực đơn).</summary>
    public interface IMasterDetailRepository<TMaster, TDetail> : IReadRepository<TMaster>
    {
        void Insert(TMaster master, List<TDetail> details);
        void Update(TMaster master, List<TDetail> details);
        int Delete(string ma);
    }
}
