import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { VendorModel } from '../../models/vendor/vendor.model';


@Injectable()
export class VendorControllerService {
    constructor(private http: HttpClient) { }

    /*searchQuoteRates(keyword: string): Observable<QuoteRateModel> {
        return this.http.get<QuoteRateModel>(APIUrls.QuoteRate.SearchQuoteRates + '?keyword=' + keyword);
    }*/

    getVendors(includeInactive = false): Observable<VendorModel[]> {
        return this.http.get<VendorModel[]>(APIUrls.Vendor.GetVendors + '?includeInactive=' + includeInactive, { });
    }

    saveVendors(saveModel: VendorModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Vendor.SaveVendor, saveModel);
    }

}
