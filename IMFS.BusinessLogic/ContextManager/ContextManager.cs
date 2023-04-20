using System;

namespace IMFS.BusinessLogic.ContextManager
{
    public class ContextManager : IContextManager
    {
        Func<string> _getCurrentUserId;
        Func<string> _getCurrentIPAddress;
        Func<string> _getCurrentUserName;
        Func<string> _getCurrentCustomerNumber;
        Func<string> _getCurrentUserEmail;
        Func<string> _getCountryCode;

        public ContextManager(Func<string> getCurrentUserId, Func<string> getCurrentUserName,
            Func<string> getCurrentIPAddress,
            Func<string> getCurrentCustomerNumber,
            Func<string> getCurrentUserEmail,
            Func<string> getCountryCode
            )
        {
            _getCurrentUserId = getCurrentUserId;
            _getCurrentIPAddress = getCurrentIPAddress;
            _getCurrentUserName = getCurrentUserName;
            _getCurrentCustomerNumber = getCurrentCustomerNumber;
            _getCurrentUserEmail = getCurrentUserEmail;
            _getCountryCode = getCountryCode;

        }

        public string GetCurrentUserId()
        {
            return _getCurrentUserId();
        }

        public string GetCurrentUserName()
        {
            return _getCurrentUserName();
        }

        public string GetCurrentIPAddress()
        {
            return _getCurrentIPAddress();
        }

        public string GetCurrentUserEmail()
        {
            return _getCurrentUserEmail();
        }

        public string GetCurrentCustomerNumber()
        {
            return _getCurrentCustomerNumber();
        }

        public string GetCountryCode()
        {
            return _getCountryCode();
        }
    }
}
