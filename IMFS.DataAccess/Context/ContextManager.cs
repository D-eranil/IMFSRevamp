using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.DataAccess.Context
{
    public class ContextManager : IContextManager
    {
        Func<string> _getCountryCode;

        public ContextManager(Func<string> getCountryCode)
        {
            _getCountryCode = getCountryCode;
        }

        public string GetCountryCode()
        {
            return _getCountryCode();
        }
    }
}
