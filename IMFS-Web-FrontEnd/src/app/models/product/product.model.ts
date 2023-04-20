export class ProductDetailsModel {
  internalSKUID: string;
  vendorSKUID: string;
  productDescription: string;
  vsrid: string;
  currencyCode: string;
  unitNetAmount: number;
}


export class ProductInputModel {
  imskus: string[];
  vpns: string[];
  resellerNumber: string;
  ean: string;
}

