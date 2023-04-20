import { CustomerModel } from '../customer/customer.model';

export class QuoteDetailsModel {
    id: number;
    quoteHeader: QuoteHeader;
    customerDetails: CustomerDetails;
    endUserDetails: EndUserDetails;
    selectedCustomer: CustomerModel;
    reason:string;
    message:string;
    quoteLines: QuoteLine[];

    public QuoteDetailsModel() {
        this.quoteHeader = new QuoteHeader();
        this.quoteLines = [];
        this.customerDetails = new CustomerDetails();
        this.endUserDetails = new EndUserDetails();
    }
}

export class QuoteHeader {
    quoteNumber: string;
    quoteName: string;
    quoteVersion: number;
    quoteOrigin: string;
    quoteType: string;
    financeType: string;
    financeTypeName: string;
    expiryDate: Date;
    status: string;
    quoteTotal: number;
    gstInclude:number;
    funderCode: string;
    funderId: number;
    financeValue?: number;
    frequency: string;
    quoteDuration: string;
    funderPlan: string;
}

export class CustomerDetails {
    customerNumber: string;
    customerName: string;
    customerABN: string;
    customerAddressLine1: string;
    customerAddressLine2: string;
    customerAddressState: string;
    customerAddressCity: string;
    customerPostCode: string;
    customerContact: string;
    customerEmail: string;
    customerPhone: string;
    customerCountry: string;
}

export class EndUserDetails {
    endCustomerName: string;
    endCustomerABN: string;
    endCustomerYearsTrading: string;
    endCustomerAddressLine1: string;
    endCustomerAddressLine2: string;
    endCustomerState: string;
    endCustomerCity: string;
    endCustomerPostCode: string;
    endCustomerContact: string;
    endCustomerEmail: string;
    endCustomerPhone: string;
    authorisedSignatoryName: string;
    authorisedSignatoryPosition: string;
    endCustomerCountry: string;
}

export class QuoteLine {
    imsku: string;
    vpn: string;
    description: string;
    lineNumber?: number;
    vsr: string;
    item: string;
    itemName: string;
    category: string;
    categoryName: string;
    salePrice: number;
    costPrice: number;
    qty: number;
    margin: number;
    lineTotal: number;
    totalGST: number;
    financeProductTypeID?: number;
    totalinc: number;
}

export class QuoteDetailsResponseModel {
    status: string;
    message: string;
    quoteDetails: QuoteDetailsModel;
}

export class QuoteSearchModel {
    quoteNumber?: number;
    quoteStatus: number;
    quoteFinancetype: number;
    createdDate?: Date;
    expiryDate?: Date;
    endUser: string;
    resellerAccount:number;
    TriggerSource?:string;
}

export class QuoteSearchResponseModel {
    quoteNumber: number;
    quoteName: string;
    endCustomer: string;
    quoteTotal: number;
    quoteStatus: number;
    quoteFinancetype: number;
    createdDate: Date;
    expiryDate: Date;
    displayLabel: string;
    resellerAccount:string;
}

export class QuoteDownloadInput {
    QuoteId: number;
    Version: number;
    DownloadMode: string;
}

export class RecentQuotes {
  status: string;
  message: string;
  recentQuoteDetails: RecentQuoteModel [];
}

export class RecentQuoteModel {
  quoteNumber?: number;
  quoteName: string;
  endCustomerName: string;
  quoteTotal?: number;
  status?: string;
  quoteExpiryDate?: Date;
  quoteCreatedDate?: Date;
  quoteLastModified?: Date;
  resellerAccount: string;
}
export class RejectQuoteModel{
quoteId:string;
  reason: string;
  comment: string;
}
export class QuoteDocuments {
    fileId?: string | number;
    quoteId?: string;
    fileName?: string;
    description?: Number;
    createdDate?:Date;
    uploadBy?:string;
}

  
