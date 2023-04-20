import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import {
  ApplicationContact,
    ApplicationDetailsModel,
    ApplicationDetailsResponseModel, ApplicationDownloadInput, ApplicationSearchModel, AwaitingInvoiceModel, RecentApplicationsModel
} from 'src/app/models/application/application.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { RecentQuoteModel } from '../../models/quote/quote.model';
import { JsUtilityService } from './../utility-services/js-utility.service';
import { AwaitingInvoices, RecentApplications } from '../../models/application/application.model';
import { ApplicationContactResponseModel } from 'src/app/models/applicationContact/applicationContact.model';


@Injectable()
export class ApplicationControllerService {
    constructor(private http: HttpClient, private jsUtilityService: JsUtilityService) { }

    getApplicationDetails(applicationId: number): Observable<ApplicationDetailsResponseModel> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<ApplicationDetailsResponseModel>(APIUrls.Application.GetApplicationDetails + '?applicationId=' + applicationId.toString());
    }

    getSignatoriesDetails(applicationId: number, resellerID: number): Observable<ApplicationContactResponseModel> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<ApplicationContactResponseModel>(APIUrls.Application.getSignatoriesDetails + '?applicationNumber=' + applicationId.toString() + '&resellerId=' + resellerID.toString());
    }

    cancelApplication(applicationId: number): Observable<HttpResponseData> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<HttpResponseData>(APIUrls.Application.CancelApplication + '?applicationId=' + applicationId.toString());
    }

    getContacts(resellerId: string): Observable<HttpResponseData> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<HttpResponseData>(APIUrls.Application.GetContacts + '?resellerId=' + resellerId);
    }

    getExistingContacts(resellerId: string, contactType: number): Observable<HttpResponseData> {
        // tslint:disable-next-line: max-line-length
        return this.http.get<HttpResponseData>(APIUrls.Application.GetExistingContacts + '?resellerId=' + resellerId + '&tabId=' + contactType);
    }

    saveApplication(saveModel: ApplicationDetailsModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Application.SaveApplication, saveModel);
    }

    saveContacts(saveModel: ApplicationContact): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Application.saveContacts, saveModel);
    }

    deleteContact(contactID: string): Observable<HttpResponseData> {
        const body = {};
        return this.http.post<HttpResponseData>(APIUrls.Application.deleteContact + '?contactID=' + contactID, body);
    }

    searchApplication(inputModel: ApplicationSearchModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Application.SearchApplication, inputModel);
    }

    lookupApplicationNumber(inputModel: ApplicationSearchModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Application.LookupApplicationNumber, inputModel);
    }


    GetDealStatus(): Observable<any> {
        return this.http.get<any>(APIUrls.Application.GetDealStatus);
    }


    downloadApplication(inputModel: ApplicationDownloadInput): Observable<any> {
        const headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http
            .post(APIUrls.Application.DownloadApplication, inputModel, { headers, responseType: 'blob', observe: 'response' })
            .pipe(
                catchError((res: HttpErrorResponse) => {
                    return this.jsUtilityService.convertBlobToText(res.error);
                }),
            );
    }

    getRecentApplications(): Observable<RecentApplications> {
        return this.http.get<RecentApplications>(APIUrls.Application.RecentApplication, {});
    }

    getAwaitingInvoices(): Observable<AwaitingInvoices> {
        return this.http.get<AwaitingInvoices>(APIUrls.Application.AwaitingInvoice, {});
    }
    getApplicationFileList(Id: number): Observable<any> {
        let Source = 2
        return this.http.get(APIUrls.Quote.GetQuoteAttachments + "?" + "id=" + Id + '&source=' + Source)
    }

    public downloadQuoteAttachment(fileId: number): Observable<any> {
        return this.http.get(APIUrls.Quote.DownloadQuoteAttachment + '?fileId=' + fileId,
            { responseType: 'blob', observe: 'response' }).pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }))
    }
}
