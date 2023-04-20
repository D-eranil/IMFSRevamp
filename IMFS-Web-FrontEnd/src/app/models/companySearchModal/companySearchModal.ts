export class AbnDetailsResponseModel {
  abn: string;
  acn:string;
  addressPostcode:string;
  addressState: string;
  entityName: string;
}
export class CompanySearchResponseModel {
        message: string;
        names:CompanySearchDetailResponseModel[];
          
}
export class CompanySearchDetailResponseModel{
        abn: string;
        abnStatus: string;
        isCurrent: true;
        name: string;
        nameType: string;
        postcode: string;
        score: string;
        state: string;
}