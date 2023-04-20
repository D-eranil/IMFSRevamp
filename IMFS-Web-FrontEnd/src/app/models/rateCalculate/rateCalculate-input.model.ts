export class RateCalculateInputModel {
    Source: string;
    QuoteTotal: number;
    Funder: string;
    Duration: string[];
    Frequency: string[];
    IncludeTax: boolean;
    TaxRate: number;
    FinanceType: string;
    FunderPlan: string;
    GstInclude:number;
    QuoteLines: RateCalculateLineItem[];
}

export class RateCalculateLineItem {
    Imsku: string;
    VendorSKU: string;
    Qty: number;
    Vendor: string;
    Vsr: string;
    SellPrice: string;
    LineTotal: number;
    ProductType: string;
    Type: string;
    Category: string;
    LineNumber?: number;
}


export class RateCalculateResponseModel {
  status: string;
  message: string;
  financeDetails: RateCalculateFinanceDetailsModel[];
}

export class RateCalculateFinanceDetailsModel {
  funderID: string;
  funder: string;
  funderPlanId: string;
  funderPlanDescription: string;
  duration: string;
  financeType: string;
  financeTypeID: string;
  frequency: string;
  financeTotal: number;
}


