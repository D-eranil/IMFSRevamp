
export class ApplicationSearchModel {
    applicationNumber?: number;
    status: number;
    financeType?: string[];
    fromDate?: Date;
    toDate?: Date;
    endCustomerName: string;
    TriggerSource?:string;
    resellerId:number;
}


export class ApplicationSearchResponseModel {
    id: number;
    applicationNumber: number;
    status: number;
    statusDescr: string;
    endCustomerName: string;
    resellerId: string;
    financeTotal: number;
    quoteTotal: number;
    financeType: number;
    financeTypeName: string;
    createdDate: Date;
    displayLabel: string;
}

export class ApplicationDetailsResponseModel {
    status: string;
    message: string;
    applicationDetails: ApplicationDetailsModel;

}

export class ApplicationDetailsModel {
    id: number;
    applicationNumber: number;
    quoteID: number;
    status: number;
    statusDescription: string;
    funderPlan: number;
    financeValue: number;
    financeType: string;
    quoteTotal: number;
    financeTotal?: number;
    aveAnnualSales?: number;
    isGuarantorPropertyOwner?: boolean;
    guarantorSecurityValue?: number;
    guarantorSecurityOwing?: number;
    isGuarantorSecurity?: boolean;

    leaseAgreementSignorEmail: string;
    certificateOfAcceptanceSignorEmail: string;
    antiMoneyLaunderingSignorEmail: string;
    directDebitACHSignorEmail: string;

    financeLink: string;
    financeDuration: string;
    financeFrequency: string;
    financeFunder: string;
    financeFunderName: string;
    financeFunderEmail: string;
    createdDate: Date;
    imfsContactName: string;
    imfsContactEmail: string;
    imfsContactPhone: string;
    businessActivity: string;
    goodsDescription: string;
    isApplicationSigned?: boolean;

    funderQuote: string;
    revisedApprovedAmount: number;
    downPaymentAmount: number;
    paymentWithoutTax: number;
    estimatedTaxAmount: number;
    paymentAmount: number;
    upFrontPayment: number;
    bankGuaranteeLetterOfCreditPcr: number;
    termUpperLimit: string;
    leaseStructureNotPermitted: string;
    arrearsMonthlyNotPermitted: boolean;
    arrearsQuarterlyNotPermitted: boolean;

    entityDetails: EntityDetails;
    resellerDetails: ResellerDetails;
    endCustomerDetails: EndCustomerDetails;
    applicationContacts: ApplicationContact[];

    public ApplicationDetailsModel() {
        this.entityDetails = new EntityDetails();
        this.resellerDetails = new ResellerDetails();
        this.endCustomerDetails = new EndCustomerDetails();
        this.applicationContacts = [];
    }
}

export class EntityDetails {
    entityType: string;
    entityTrustName: string;
    entityTrustABN: string;
    entityTrustType: string;
    entityTypeOther: string;
    entityTrustOther: string;
}

export class ResellerDetails {
    resellerID: string;
    resellerName: string;
    resellerContactName: string;
    resellerContactEmail: string;
    resellerContactPhone: string;
}

export class EndCustomerDetails {

    endCustomerName: string;
    endCustomerTradingAs: string;
    endCustomerYearsTrading: string;
    endCustomerABN: string;
    endCustomerType: string;
    endCustomerContactName: string;
    endCustomerContactPhone: string;
    endCustomerContactEmail: string;
    endCustomerSignatoryName: string;
    endCustomerSignatoryPosition: string;
    endCustomerPhone: string;
    endCustomerFax: string;
    endCustomerPrimaryAddressLine1: string;
    endCustomerPrimaryAddressLine2: string;
    endCustomerPrimaryCity: string;
    endCustomerPrimaryState: string;
    endCustomerPrimaryCountry: string;
    endCustomerPrimaryPostcode: string;
    endCustomerDeliveryAddressLine1: string;
    endCustomerDeliveryAddressLine2: string;
    endCustomerDeliveryCity: string;
    endCustomerDeliveryState: string;
    endCustomerDeliveryCountry: string;
    endCustomerDeliveryPostcode: string;
    endCustomerPostalAddressLine1: string;
    endCustomerPostalAddressLine2: string;
    endCustomerPostalCity: string;
    endCustomerPostalState: string;
    endCustomerPostalCountry: string;
    endCustomerPostalPostcode: string;

}


export class ApplicationContact {
    contactType: number;
    contactDescription: string;
    contactID: string;
    contactEmail: string;
    resellerID: string;
    contactName: string;
    contactDOB?: Date;
    contactAddress: string;
    contactDriversLicNo: string;
    contactABNACN: string;
    contactPosition: string;
    isContactSignatory?: boolean;
    contactPhone: string;
}

export class ApplicationDownloadInput {
    ApplicationNumber: number;
    Version: number;
    DownloadMode: string;
}

export class RecentApplications {
    status: string;
    message: string;
    recentAppDetails: RecentApplicationsModel[];
}

export class RecentApplicationsModel {
    id?: number;
    applicationNumber?: number;
    endUser: string;
    status: string;
    finanaceAmount?: number;
    financeType?: string;
    createdDate?: Date;
}

export class AwaitingInvoices {
    status: string;
    message: string;
    recentInvoiceDetails: AwaitingInvoiceModel[];
}

export class AwaitingInvoiceModel {
    id?: number;
    applicationNumber?: number;
    status: string;
    endCustomerName: string;
    finanaceAmount?: number;
    financeType?: string;
    createdDate?: Date;
    approvedDate?: Date;
}
