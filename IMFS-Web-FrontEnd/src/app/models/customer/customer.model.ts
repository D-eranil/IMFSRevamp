export class CustomerModel {
    customerID: string;
    customerNumber: string;
    customerName: string;
    abn: string;

    addressLine1: string;
    addressLine2: string;
    city: string;
    postCode: string;
    state: string;
    country: string;
    displayLabel: string;

}

export class CustomerResponseModel {
    status: string;
    message: string;
    customerDetails: CustomerModel[];

}
export class CustomerContactResponseModel {
    status: string;
    message: string;
    customerDetails: CustomerContactModel[];

}
export class CustomerContactModel {
    contactName: string;
    contactEmail: string;
    contactNumber: string;
}