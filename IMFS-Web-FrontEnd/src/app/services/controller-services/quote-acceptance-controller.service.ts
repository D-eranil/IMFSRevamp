import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { OTCEncryptionModel, OTCModel } from 'src/app/models/otc/otc.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { Email, EmailHistory, GetWriteEmailModel, WriteEmailViewModel } from '../../models/email/email.model';
import {QuoteDetailsResponseModel, RejectQuoteModel} from '../../models/quote/quote.model'

@Injectable()
export class QuoteAcceptanceControllerService {
    constructor(private http: HttpClient) { }

    sendCodeEmail(saveModel: Email): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteAcceptance.SendCodeEmail, saveModel);
    }

    verifyCode(otcModel: OTCModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteAcceptance.VerifyCode, otcModel);
    }

    getDecodedQuoteId(otcEncryptionModel: OTCEncryptionModel): Observable<HttpResponseData> {
        // tslint:disable-next-line: max-line-length
        return this.http.post<HttpResponseData>(APIUrls.QuoteAcceptance.GetDecodedQuoteId, otcEncryptionModel);
    }


    requestChangesQuote(quoteId: string): Observable<HttpResponseData> {
        return this.http.get<HttpResponseData>(APIUrls.QuoteAcceptance.RequestChangesQuote + '?quoteId=' + quoteId, {});
    }

    rejectQuote(quoteId: string): Observable<RejectQuoteModel> {
        return this.http.post<RejectQuoteModel>(APIUrls.QuoteAcceptance.RejectQuote + '?quoteId=' + quoteId, {});
    }
    rejectQuoteSend(rejectQuote:RejectQuoteModel): Observable<any> {
        return this.http.post<any>(APIUrls.QuoteAcceptance.RejectQuote, rejectQuote );
    }

}
