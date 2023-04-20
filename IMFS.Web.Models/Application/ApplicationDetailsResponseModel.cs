using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationDetailsResponseModel : ErrorModel
    {
        public ApplicationDetailsModel ApplicationDetails { get; set; }

        public ApplicationDetailsResponseModel()
        {

        }
    }

    #region Signatories Tabs

    public class GuarantorsListResult
    {
        public string ContactID { get; set; }
        public int ApplicationNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactPhone { get; set; }
        public string ContactDescription { get; set; }
        public bool IsContactSignatory { get; set; }
        public string ResellerID { get; set; }
    }

    public class GuarantorsSelectListResult : GuarantorsListResult
    {
       
        
    }
    public class TrusteesListResult
    {
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
        public int ApplicationNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactPhone { get; set; }
        public string ContactDescription { get; set; }
        public string ContactABN { get; set; }
        public bool IsContactSignatory { get; set; }
    }
    public class TrusteesSelectListResult : TrusteesListResult
    {
       
    }
    public class AccountantListResult
    {
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
        public int ApplicationNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactPhone { get; set; }
        public string ContactDescription { get; set; }
        public bool IsContactSignatory { get; set; }
    }

    public class OwnerListResult
    {
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
        public int ApplicationNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactPhone { get; set; }
        public string ContactDescription { get; set; }
        public bool IsContactSignatory { get; set; }
    }

    public class OwnerSelectListResult : OwnerListResult
    {
        
    }

    public class SignatoriesTabModel : ErrorModel
    {
        public IList<GuarantorsListModel> Guarantors { get; set; }
        public IList<TrusteesListModel> Trustees { get; set; }
        public IList<AccountantListModel> Accountants { get; set; }
        public IList<OwnersListModel> Owners { get; set; }
    }

    //guarantors
    public class GuarantorsListModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ResidentialAddress { get; set; }
        public DateTime? DOB { get; set; }
        public string DriversLicNo { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public bool Signatory { get; set; }
        public int ContactType { get; set; }
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
    }
    //trustees
    public class TrusteesListModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ResidentialAddress { get; set; }
        public DateTime? DOB { get; set; }
        public string DriversLicNo { get; set; }
        public string Phone { get; set; }
        public string ABN { get; set; }
        public bool Signatory { get; set; }
        public int ContactType { get; set; }
        public string ContactID { get; set; }
        public string ResellerID { get; set; }

    }
    //accountant
    public class AccountantListModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactID { get; set; }
        public string ResellerID { get; set; }

    }
    //owners
    public class OwnersListModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ResidentialAddress { get; set; }
        public DateTime? DOB { get; set; }
        public string DriversLicNo { get; set; }
        public string Phone { get; set; }
        public bool Signatory { get; set; }
        public int ContactType { get; set; }
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
    }


    public class ExistingContactsSelectListModel
    {
        public string ContactID { get; set; }
        public string ResellerID { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactPhone { get; set; }
        public string ContactTypeDescription { get; set; }
        public string ContactABNACN { get; set; }
        public int ContactType { get; set; }
        public bool IsContactSignatory { get; set; }
    }


    public class ExistingContactsModel : ErrorModel
    {
        public IList<ExistingContactsSelectListModel> ExistingContacts { get; set; }
    }

    public class ContactsRequestModel : ErrorModel
    {
        public string ContactID { get; set; }
        public int ApplicationNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ResellerID { get; set; }
        public int ContactType { get; set; }
        public string ContactName { get; set; }
        public string ContactDOB { get; set; }
        public string ContactAddress { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactABNACN { get; set; }
        public string ContactPosition { get; set; }
        public bool IsContactSignatory { get; set; }
        public string ContactPhone { get; set; }
    }

    public class ContactsResponseModel : ErrorModel
    {
        public string ContactID { get; set; }
    }
    #endregion
}