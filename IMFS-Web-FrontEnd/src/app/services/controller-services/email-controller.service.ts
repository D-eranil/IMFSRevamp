import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { Email, EmailHistory, GetWriteEmailModel, WriteEmailViewModel } from '../../models/email/email.model';


@Injectable()
export class EmailControllerService {
    constructor(private http: HttpClient) { }

    saveEmail(saveModel: Email): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Email.SendEmail, saveModel);
    }

    getWriteEmailModel(inputModel: GetWriteEmailModel): Observable<WriteEmailViewModel> {
        // tslint:disable-next-line: max-line-length
        return this.http.post<WriteEmailViewModel>(APIUrls.Email.GetWriteEmailModel, inputModel);
        }

    getEmailHistory(quoteId: number): Observable<EmailHistory[]> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<EmailHistory[]>(APIUrls.Email.GetEmailHistory + '?quoteId=' + quoteId, {});
    }

    getApplicationEmailHistory(applicationId: number): Observable<EmailHistory[]> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<EmailHistory[]>(APIUrls.Email.GetApplicationEmailHistory + '?applicationId=' + applicationId, {});
    }

}
