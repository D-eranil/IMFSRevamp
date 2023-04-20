import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient, HttpParams } from "@angular/common/http";
import {AbnDetailsResponseModel, CompanySearchResponseModel} from '../../models/companySearchModal/companySearchModal';
import { environment } from 'src/environments/environment';


const baseUrl: string = environment.API_BASE + '/' + 'Quote' + '/';
@Injectable()
export class SearchService {

    constructor(private httpclient: HttpClient) { }

    getBusinessName(name:string):Observable<any> {
      const guid = '36cc9a1f-4810-4d75-babd-1bf395d5bb5f'; 
      return this.httpclient.get(baseUrl + 'GetAbnInfoListingByName?name='+name+'&guid='+guid);  
    }
    
    getAbn(abn:string): Observable<AbnDetailsResponseModel> {
      const guid = '36cc9a1f-4810-4d75-babd-1bf395d5bb5f'; 
      return this.httpclient.get<AbnDetailsResponseModel>(baseUrl + 'GetAbnDetails?abn='+abn+'&guid='+guid);    
    }
    
}