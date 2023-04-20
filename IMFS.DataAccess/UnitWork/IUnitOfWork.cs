using IMFS.DataAccess.Context;
using IMFS.DataAccess.DBContexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.DataAccess.UnitWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContext DbContext { get; }

        IContextManager ContextManager { get; }
        int Save();
    }
}
