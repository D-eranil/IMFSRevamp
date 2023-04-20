using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.OTC;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.Quote
{
    public interface IQuoteAcceptanceManager
    {
        void InsertOTC(OTC otc);

        void UpdateOTC(OTC otc);

        bool VerifyCode(OTCModel model);

        OTC GetOTC(OTCModel model);

        void DeletePreviousCodes(int quoteId);
    }
}
