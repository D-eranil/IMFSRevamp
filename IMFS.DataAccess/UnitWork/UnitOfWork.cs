using IMFS.DataAccess.Context;
using IMFS.DataAccess.DBContexts;
using Microsoft.AspNetCore.Http;
using System;

namespace IMFS.DataAccess.UnitWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected string ConnectionString;
        protected string CountryCode;
        
        private IMFSDBContext _dbcontext;
        private IContextManager _contextManager;

        public UnitOfWork(string connectionString, Func<string> getCountryCode, string nzConnectionString = "")
        {
            this.ConnectionString = connectionString;
            this.CountryCode = getCountryCode();

            // For IMFS rate calculator api, country code is passed in the request header and need to change the connection string
            if (!string.IsNullOrEmpty(nzConnectionString))
            {
                try
                {                    
                    if (!string.IsNullOrEmpty(this.CountryCode) && this.CountryCode.ToLower() == "nz")
                    {
                        this.ConnectionString = nzConnectionString;
                        this._dbcontext = null;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public IContextManager ContextManager
        {
            get
            {
                if (_contextManager == null)
                {                    
                    _contextManager = new ContextManager(ReturnCountryCode);
                }
                return _contextManager;
            }
        }

        private string ReturnCountryCode()
        {
            return this.CountryCode;
        }


        public IDbContext DbContext
        {
            get
            {
                if (_dbcontext == null)
                {
                    _dbcontext = new IMFSDBContext(this.ConnectionString);
                }
                return _dbcontext;
            }
        }


        public int Save()
        {
            return _dbcontext.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbcontext != null)
                {
                    _dbcontext.Dispose();
                    _dbcontext = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
