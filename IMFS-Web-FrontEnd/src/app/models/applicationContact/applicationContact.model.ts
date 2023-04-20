export class ApplicationContactResponseModel {
    hasError: boolean;
    errorMessage: string;
    guarantors: ApplicationContactData [];
    trustees: ApplicationContactData [];
    accountants: ApplicationContactData [];
    owners: ApplicationContactData [];

}

export class ApplicationContactData {
  fullName: string;
  email: string;
  residentialAddress: string;
  dob: any;
  driversLicNo: number;
  phone: number;
  type: string;
  contactType: number;
  abn: string;
  signatory: boolean;
  contactID: any;
}
