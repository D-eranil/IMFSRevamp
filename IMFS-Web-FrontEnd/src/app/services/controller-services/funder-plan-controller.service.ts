import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { FunderPlanModel } from 'src/app/models/funder-plan/funder-plan.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';



@Injectable()
export class FunderPlanControllerService {
    constructor(private http: HttpClient) { }

    getFunderPlans(): Observable<FunderPlanModel[]> {
        return this.http.get<FunderPlanModel[]>(APIUrls.FunderPlan.GetFunderPlans, {});
    }

}
