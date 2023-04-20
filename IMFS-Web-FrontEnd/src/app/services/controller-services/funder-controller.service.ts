import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FunderModel } from '../../models/funder/funder.model';


@Injectable()
export class FunderControllerService {
    constructor(private http: HttpClient) { }

    getFunders(includeInactive = false): Observable<FunderModel[]> {
        return this.http.get<FunderModel[]>(APIUrls.Funder.GetFunders + '?includeInactive=' + includeInactive, {});
    }

    saveFunders(saveModel: FunderModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Funder.SaveFunder, saveModel);
    }
}
