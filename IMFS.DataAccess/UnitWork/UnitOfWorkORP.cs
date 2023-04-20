using IMFS.DataAccess.Context;
using IMFS.DataAccess.DBContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.DataAccess.UnitWork
{
    public class UnitOfWorkORP : IUnitOfWorkORP
    {

        protected string ConnectionString;
        protected string CountryCode;

        private ORPDBContext _ORPDbcontext;
        private IContextManager _contextManager;

        public UnitOfWorkORP(string connectionString, Func<string> getCountryCode, string nzConnectionString = "")
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
                        this._ORPDbcontext = null;
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
                if (_ORPDbcontext == null)
                {
                    _ORPDbcontext = new ORPDBContext(this.ConnectionString);
                }
                return _ORPDbcontext;
            }
        }


        public int Save()
        {
            return _ORPDbcontext.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ORPDbcontext != null)
                {
                    _ORPDbcontext.Dispose();
                    _ORPDbcontext = null;
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
