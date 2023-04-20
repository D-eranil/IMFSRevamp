using IMFS.Web.Models.DBModel;
using System.Linq;

namespace IMFS.DataAccess.Repository
{
    public partial interface IRepository<T> where T : BaseEntity
    {
        T GetById(object id);

        void Insert(T entity, bool saveDataContext = true);

        void Update(T entity, bool saveDataContext = true);

        void Delete(T entity, bool saveDataContext = true);

        void AttachAndDelete(T entity, bool saveDataContext = true);

        IQueryable<T> Table { get; }

        void SaveDataContext();
    }
}
