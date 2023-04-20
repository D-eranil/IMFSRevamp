using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class AddressList : BaseEntity
    {
        public AddressCompare addressCompare { get; set; }
        public Match match { get; set; }
    }

    public class AddressCompare
    {
        public AddressInfo addressInfo { get; set; }
        public SuggestedAddress suggestedAddress { get; set; }
        public AddressInputAnalysis addressInputAnalysis { get; set; }
    }

    public class AddressInfo
    {
        public string contact { get; set; }
        public string companyName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        //public string building { get; set; }
    }

    public class SuggestedAddress
    {
        public string contact { get; set; }
        public string companyName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        //public string building { get; set; }
        //public string premise { get; set; }
        //public string subBuilding { get; set; }
    }

    public class AddressInputAnalysis
    {
        public string addressInputCorrect { get; set; }
        public string primaryInfo { get; set; }
        public string secondaryInfo { get; set; }
        public string generalAddress { get; set; }
        public string noDelivery { get; set; }
        public string rdiStatus { get; set; }
        public string vacantAddress { get; set; }
        public string validPostBox { get; set; }
        public string zipCodePrecision { get; set; }
        public string addressInputFeedback { get; set; }
    }

    public class Match
    {
        public string addressQuality { get; set; }
        public string verificationQuality { get; set; }
        public string preProcessScore { get; set; }
        public string matchScore { get; set; }
        public string promptThreshold { get; set; }
        public GeoCode geoCode { get; set; }
}

    public class GeoCode
    {
        public string geoAccuracyMatch { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public string coordinateLicense { get; set; }   
    }

    public class AbnDetailsResponseSchema
    {
        public string Abn { get; set; }
        //[IgnoreDataMember]
        //public string AbnStatus { get; set; }
        //[IgnoreDataMember]
        //public string AbnStatusEffectiveFrom { get; set; }
        public string Acn { get; set; }
        //[IgnoreDataMember]
        //public string AddressDate { get; set; }
        public string AddressPostcode { get; set; }
        public string AddressState { get; set; }
        public string EntityName { get; set; }
        //[IgnoreDataMember]
        //public string EntityTypeCode { get; set; }
        //[IgnoreDataMember]
        //public string EntityTypeName { get; set; }
        //[IgnoreDataMember]
        //public string Gst { get; set; }
        //[IgnoreDataMember]
        //public string Message { get; set; }
        //[IgnoreDataMember]
        //public List<BusinessName> BusinessName { get; set; }
    }
    public class BusinessName
    {
        //public string Name { get; set; }
    }

    public class AbnMessageResponse
    {
        public string Message { get; set; }
        public List<AbnListingResponseSchema> Names { get; set; }
    }
    public class AbnListingResponseSchema
    {
        public string Abn { get; set; }
        public string AbnStatus { get; set; }
        public bool IsCurrent { get; set; }
        public string Name { get; set; }
        public string NameType { get; set; }
        //[JsonProperty("Postcode")]
        public string Postcode { get; set; }
        public string Score { get; set; }
        public string State { get; set; }
    }
}
