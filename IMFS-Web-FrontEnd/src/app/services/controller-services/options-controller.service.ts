import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { TypesModel, CategoriesModel, StatusModel } from 'src/app/models/options/options.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';



@Injectable()
export class OptionsControllerService {
    constructor(private http: HttpClient) { }

    getTypes(includeInactive = false): Observable<TypesModel[]> {
        return this.http.get<TypesModel[]>(APIUrls.Options.GetTypes + '?includeInactive=' + includeInactive, {});
    }

    getCategories(includeInactive = false): Observable<CategoriesModel[]> {
        return this.http.get<CategoriesModel[]>(APIUrls.Options.GetCategories + '?includeInactive=' + includeInactive, {});
    }

    getStatus(includeInactive = false, quoteOnly = false, applicationOnly = false): Observable<StatusModel[]> {
        return this.http.get<StatusModel[]>(APIUrls.Options.GetStatus + '?includeInactive=' + includeInactive + '&quoteOnly=' + quoteOnly + '&applicationOnly=' + applicationOnly, {});
    }

}

