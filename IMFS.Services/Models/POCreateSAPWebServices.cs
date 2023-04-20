using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Services.Models
{

    public class POCreateSAPConfig
    {
        public string CompanyCode { get; set; }
        public string SalesOrganization { get; set; }
        public string PurchasingOrganization { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }

        public string Url { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserCredentialID { get; set; }
        public string POCreateInputTemplatePath { get; set; }
    }

    public class POCreateSAPInput
    {
        public string CorrelationID { get; set; }
        public string DocumentType { get; set; }
        public string PurchasingGroup { get; set; }

        public string HeaderText { get; set; }
        public string HeaderNotes { get; set; }

        public string HeaderMandatoryFields { get; set; }
        public List<POCreateSAPInputItem> Items { get; set; }


        public POCreateSAPInput()
        {
            Items = new List<POCreateSAPInputItem>();
        }
    }

    public class POCreateSAPInputItem
    {
        public string ChangeID { get; set; }
        public string POItemNumber { get; set; }
        public string GoodsReceiptIndicator { get; set; }
        public string GoodsReceiptBasedInvoiceReceipt { get; set; }
        public string InvoiceIndicator { get; set; }
        public string FreeItem { get; set; }

        public string Currency { get; set; }
        public string Material { get; set; }
        public string Quantity { get; set; }
        public string Vendor { get; set; }
        public decimal? SpecialPrice { get; set; }
        public string ConfirmationControl { get; set; }

        public string LineComment { get; set; }
    }


    public class SAPPOCreateOutput
    {
        public bool HasError { get; set; }
        public List<string> ErrorMessages { get; set; }
        public string PONumber { get; set; }
        public string RawRequest { get; set; }
        public string RawResponse { get; set; }
        public SAPPOCreateOutput()
        {

        }
    }
}

