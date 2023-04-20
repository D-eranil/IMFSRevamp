import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FinanceTypeModel } from '../../models/finance-type/finance-type.model';


@Injectable()
export class FinanceTypeControllerService {
    constructor(private http: HttpClient) { }

    getFinanceTypes(includeInactive = false): Observable<FinanceTypeModel[]> {
        return this.http.get<FinanceTypeModel[]>(APIUrls.FinanceType.GetFinanceTypes + '?includeInactive=' + includeInactive, {});
    }

    saveFinanceTypes(saveModel: FinanceTypeModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.FinanceType.SaveFinanceType, saveModel);
    }
}



