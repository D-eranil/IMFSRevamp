using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IMFS.Web.Models.DBModel;

namespace IMFS.DataAccess.Repository
{
    public partial interface IGenericORPrepository<T> where T : BaseEntity
    {
        T GetById(object id);

        void Insert(T entity, bool saveDataContext = true);

        void Update(T entity, bool saveDataContext = true);

        void Delete(T entity, bool saveDataContext = true);

        IQueryable<T> Table { get; }
        
        void SaveDataContext();
    }
}
