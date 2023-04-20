import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { EmailAttachment } from '../../models/email/email.model';
import { JsUtilityService } from '../utility-services/js-utility.service';


@Injectable()
export class AttachmentControllerService {
    constructor(
        private http: HttpClient,
        private jsUtilityService: JsUtilityService) { }

    saveEmailAttachmentTempFile(formData: FormData): Observable<EmailAttachment[]> {
        return this.http.post<EmailAttachment[]>(APIUrls.Attachment.SaveEmailAttachmentTempFile, formData);
    }

    removeEmailAttachmentTempFile(emailAttachmentTempId?: number): Observable<HttpResponseData> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<HttpResponseData>(APIUrls.Attachment.RemoveEmailAttachmentTempFile + '?emailAttachmentTempId=' + emailAttachmentTempId, {});
    }

    getEmailAttachments(emailId: string, tempEmailId: string): Observable<EmailAttachment[]> {

        let params = new HttpParams();
        if (emailId) {
            params = params.append('emailId', emailId);
        }
        if (tempEmailId) {
            params = params.append('tempEmailId', tempEmailId);
        }

        return this.http.get<EmailAttachment[]>(APIUrls.Attachment.GetEmailAttachments, { params });
    }

    downloadEmailAttachment(attachmentId: string): Observable<any> {
        return this.http
            .get(APIUrls.Attachment.DownloadEmailAttachment + '?attachmentId=' + attachmentId,
                { responseType: 'blob', observe: 'response' })
            .pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }));
    }

    downloadEmailTempAttachment(attachmentId: string): Observable<any> {
        return this.http
            .get(APIUrls.Attachment.DownloadEmailTempAttachment + '?attachmentId=' + attachmentId,
                { responseType: 'blob', observe: 'response' })
            .pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }));
    } 
}
