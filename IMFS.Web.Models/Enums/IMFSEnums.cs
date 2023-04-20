using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Enums
{
    public class IMFSEnums
    {
        public enum QuoteDuration
        {
            Months12 = 12,
            Months24 = 24,
            Months36 = 36,
            Months48 = 48,
            Months60 = 60
        }

        public enum PaymentType
        {
            monthly,
            quarterly,
            upfront
        }

        public enum QuoteLogTypes
        {
            QuoteCreated = 1,
            NewVersionCreated = 2,         
            EmailSent = 3,
            Update = 4,
            AttachFile = 5,
            DeleteFile = 6,  
            QuoteAccepted = 7,
            ApplicationFormSent = 8
        }

        public enum LogTypes
        {
            Quote_AttachmentNewFile,
            Quote_AttachmentDownload,
            Quote_AttachmentEdit,
            Quote_AttachmentDelete,
            Quote_AttachmentError,            

            //Web Services
            WebService_Call,
            WebService_Error,            
        }

        public enum QuotePriceType
        {
            BySKU,
            ByTotal,
            ByPercent
        }

        public enum CurrencyType
        {
            USD,
            AUD,
            NZD
        }

        #region Email Status

        public enum EmailStatus
        {
            New,
            Neglect,
            Completed,
            AutoAttach,
            ReplyAndComplete,
            ForwardAndComplete,
            Outbound
        }

        #endregion Email Status

        #region Email Type

        public enum EmailType
        {
            Received,
            Sent,
        }

        #endregion Email Type

        #region email body type

        public enum EmailBodyType
        {
            HTML,
            Text
        }

        #endregion email body type

        #region Email Importance

        public enum EmailImportance
        {
            Normal,
            High,
        }

        #endregion Email Importance
    }
}
