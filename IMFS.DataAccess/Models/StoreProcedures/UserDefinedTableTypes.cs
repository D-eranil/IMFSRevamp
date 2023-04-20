using EntityFrameworkExtras.EF6;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.DataAccess.Models.StoreProcedures
{
    #region generic user define types
    [UserDefinedTableType("IntIDs")]
    public class IntIDs
    {
        [UserDefinedTableTypeColumn(1)]
        public int ID { get; set; }
    }

    [UserDefinedTableType("StringIDs")]
    public class StringIDs
    {
        [UserDefinedTableTypeColumn(1)]
        public string ID { get; set; }
    }
    #endregion
}
