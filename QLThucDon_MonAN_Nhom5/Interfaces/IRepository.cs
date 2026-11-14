
using System.Collections.Generic;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IRepository<T>
    {
        List<T> GetAll();

        T GetById(string ma);

        int Insert(T item);

        int Update(T item);

        int Delete(string ma);

        string NextCode();
    }
}
