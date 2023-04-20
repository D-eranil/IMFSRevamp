using IMFS.DataAccess.Context;
using IMFS.DataAccess.DBContexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.DataAccess.UnitWork
{
    public interface IUnitOfWorkORP: IDisposable
    {
        IDbContext DbContext { get; }        
        int Save();
        IContextManager ContextManager { get; }
    }
}
