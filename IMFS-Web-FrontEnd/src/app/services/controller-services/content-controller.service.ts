import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { SystemConfigResponse, SystemMessagesResponse } from './../../models/misc/misc.model';


@Injectable()
export class ContentControllerService {
    constructor(private http: HttpClient) { }
    loadSystemConfig(): Observable<SystemConfigResponse> {
        return this.http.get<SystemConfigResponse>(APIUrls.Content.GetSystemConfig);
    }

    loadSystemMessages(): Observable<SystemMessagesResponse> {
        const headersAdditional: HttpHeaders = new HttpHeaders();
        headersAdditional.append('Cache-control', 'no-cache');
        headersAdditional.append('Cache-control', 'no-store');
        headersAdditional.append('Expires', '0');
        headersAdditional.append('Pragma', 'no-cache');
        return this.http.get<SystemMessagesResponse>(APIUrls.Content.GetSystemMessages,
            { headers: headersAdditional });
    }
}
