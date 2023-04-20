import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FinanceProductTypeModel } from '../../models/finance-product-type/finance-product-type.model';


@Injectable()
export class FinanceProductTypeControllerService {
    constructor(private http: HttpClient) { }

    getFinanceProductTypes(includeInactive = false): Observable<FinanceProductTypeModel[]> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<FinanceProductTypeModel[]>(APIUrls.FinanceProductType.GetFinanceProductTypes + '?includeInactive=' + includeInactive, {});
    }

    saveFinanceProductTypes(saveModel: FinanceProductTypeModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.FinanceProductType.SaveFinanceProductType, saveModel);
    }
}
