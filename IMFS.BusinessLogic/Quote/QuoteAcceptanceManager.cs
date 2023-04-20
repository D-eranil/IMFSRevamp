using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.OTC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IMFS.BusinessLogic.Quote
{
    public class QuoteAcceptanceManager : IQuoteAcceptanceManager
    {
        private readonly IRepository<QuoteLog> _quoteLogRepository;
        private readonly IRepository<OTC> _otcRepository;
        private readonly IQuoteManager _quoteManager;

        public QuoteAcceptanceManager( IRepository<QuoteLog> quoteLogRepository,
            IRepository<OTC> otcRepository,
            IQuoteManager quoteManager)
        {
            _quoteLogRepository = quoteLogRepository;
            _otcRepository = otcRepository;
            _quoteManager = quoteManager;
        }

        public void DeletePreviousCodes(int quoteId)
        {
            var otcCodes = (from otcRepo in _otcRepository.Table where otcRepo.QuoteId == quoteId select otcRepo).ToList();
            foreach(var code in otcCodes)
            {
                _otcRepository.Delete(code);
            }


        }

        public OTC GetOTC(OTCModel model)
        {
            int tempQuoetId;
            int.TryParse(model.QuoteId, out tempQuoetId);
            var otc = (from otcRepo in _otcRepository.Table where otcRepo.Code == model.Code && otcRepo.QuoteId == tempQuoetId select otcRepo).FirstOrDefault();
            if (otc != null)
            {
                return otc;
            }

            return null;
        }

        public void InsertOTC(OTC otc)
        {
            _otcRepository.Insert(otc);
        }

        

        public void UpdateOTC(OTC otc)
        {   
            _otcRepository.Update(otc);
        }

        
        public bool VerifyCode(OTCModel model)
        {
            int tempQuoetId;
            int.TryParse(model.QuoteId, out tempQuoetId);

            var time = System.DateTime.Now.AddDays(-1).ToLocalTime();
            var otc = (from otcRepo in _otcRepository.Table where otcRepo.Code == model.Code && otcRepo.QuoteId == tempQuoetId && otcRepo.Sent >= time select otcRepo).FirstOrDefault();
            if (otc != null)
            {
                return true;
            }               
            return false;
        }
    }
}
