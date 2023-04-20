namespace IMFS.BusinessLogic.ContextManager
{
    public interface IContextManager
    {
        string GetCurrentUserId();
        string GetCurrentUserName();
        string GetCurrentUserEmail();
        string GetCurrentIPAddress();
        string GetCurrentCustomerNumber();
        string GetCountryCode();
    }
}
