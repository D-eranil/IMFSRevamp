using EntityFrameworkExtras.EF6;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMFS.DataAccess.Models.StoreProcedures
{
    [StoredProcedure("sp_GetCustomerContacts")]
    public class spGetCustomerContacts
    {
        [StoredProcedureParameter(SqlDbType.VarChar)]
        public string CustomerNumber { get; set; }

        [StoredProcedureParameter(SqlDbType.VarChar)]
        public string Type { get; set; }

        [StoredProcedureParameter(SqlDbType.Bit)]
        public bool IncludeInactive { get; set; }
        public spGetCustomerContacts()
        {
            CustomerNumber = string.Empty;
        }
    }

    public class spGetCustomerContactsResult
    {
        public int Id { get; set; }
        public string CustomerNumber { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneCombined { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string ContactGroup { get; set; }
        public string ContactType { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public DateTime? LastUsed { get; set; }
        public string Marketing { get; set; }
        public bool BusinessUpdate { get; set; }
        public bool AnnuityQuote { get; set; }
        public bool AnnuityCountdown { get; set; }
        public string Vendor { get; set; }
        public string VendorId { get; set; }
        public string CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public string UpdatedUserId { get; set; }
        public string UpdatedUserName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int? AddressId { get; set; }
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool? HasMultiResellers { get; set; }
        public bool? CustomerUnsubscribe { get; set; }
        public bool? ContactUnsubscribe { get; set; }
    }
}
