using IMFS.BusinessLogic.ApplicationManagement;
using IMFS.BusinessLogic.Emails;
using IMFS.BusinessLogic.FinanceProductType;
using IMFS.BusinessLogic.FinanceType;
using IMFS.BusinessLogic.Funder;
using IMFS.BusinessLogic.FunderPlan;
using IMFS.BusinessLogic.Log;
using IMFS.BusinessLogic.Product;
using IMFS.BusinessLogic.Quote;
using IMFS.BusinessLogic.QuotePercentRate;
using IMFS.BusinessLogic.QuoteTotalRate;
using IMFS.BusinessLogic.Rate;
using IMFS.BusinessLogic.RoleManagement;
using IMFS.BusinessLogic.UserManagement;
using IMFS.BusinessLogic.Vendor;
using Unity;

namespace IMFS.BusinessLogic
{
    public class BusinessLogicUnityContainer
    {
        public static void ConfigureContainer(IUnityContainer container)
        {
            // Could be used to register more types
            container.RegisterType<IRateManager, RateManager>();
            container.RegisterType<IFunderManager, FunderManager>();
            container.RegisterType<IVendorManager, VendorManager>();
            container.RegisterType<IFinanceProductTypeManager, FinanceProductTypeManager>();
            container.RegisterType<IFinanceTypeManager, FinanceTypeManager>();
            container.RegisterType<IQuoteManager, QuoteManager>();
            container.RegisterType<IIMFSLogManager, IMFSLogManager>();
            container.RegisterType<IQuoteTotalRateManager, QuoteTotalRateManager>();
            container.RegisterType<IQuotePercentRateManager, QuotePercentRateManager>();
            container.RegisterType<IProductManager, ProductManager>();
            container.RegisterType<IFunderPlanManager, FunderPlanManager>();
            container.RegisterType<IEmailManager, EmailManager>();
            container.RegisterType<IQuoteDownloadManager, QuoteDownloadManager>();
            container.RegisterType<IQuoteAcceptanceManager, QuoteAcceptanceManager>();
            container.RegisterType<IUserManager, UserManager>();
            container.RegisterType<IRoleManager, RoleManager>();
            container.RegisterType<IApplicationManager, ApplicationManager>();

        }
    }
}
