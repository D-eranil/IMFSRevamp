using IMFS.DataAccess.DBContexts;
using IMFS.DataAccess.UnitWork;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.OPRDBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace IMFS.DataAccess.Repository
{
    public partial class GenericORPRepository<T> : IGenericORPrepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;
        private Guid _repoId;


        public GenericORPRepository(IUnitOfWorkORP unitOfWork)
        {
            this._context = unitOfWork.DbContext;
            this._repoId = Guid.NewGuid();
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(T entity, bool saveDataContext = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);
                if (saveDataContext)
                {
                    this._context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
            //catch(Exception ex)
            //{
            //    // Removes not saved Entity from Context
            //    this.Entities.Remove(entity);
            //    throw new Exception(ex.Message, ex);
            //}
        }

        public void Update(T entity, bool saveDataContext = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (saveDataContext)
                {
                    this._context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Delete(T entity, bool saveDataContext = true)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);

                if (saveDataContext)
                {
                    this._context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        public void SaveDataContext()
        {
            this._context.SaveChanges();
        }      

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }
    }
}
