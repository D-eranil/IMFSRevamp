import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuoteTotalRateResponseModel } from '../../models/quote-Total-rate/quote-total-rate-response.model';
import { QuoteTotalRateModel } from '../../models/quote-Total-rate/quote-total-rate.model';
import { QuoteTotalRateInputModel } from '../../models/quote-Total-rate/quote-total-rate-input.model';


@Injectable()
export class QuoteTotalRateControllerService {
    constructor(private http: HttpClient, private jsUtilityService: JsUtilityService) { }

    /*searchQuoteRates(keyword: string): Observable<QuoteRateModel> {
        return this.http.get<QuoteRateModel>(APIUrls.QuoteRate.SearchQuoteRates + '?keyword=' + keyword);
    }*/

    getQuoteTotalRates(inputModel: QuoteTotalRateInputModel): Observable<QuoteTotalRateModel[]> {
        return this.http.post<QuoteTotalRateModel[]>(APIUrls.QuoteTotalRate.GetRate, inputModel);
    }


    exportQuoteTotalRates(inputModel: QuoteTotalRateInputModel): Observable<any> {
        const headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http
            .post(APIUrls.QuoteTotalRate.ExportRate, inputModel, { headers, responseType: 'blob' as 'json', observe: 'response' })
            .pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }));
    }

    saveQuoteTotalRates(saveModel: QuoteTotalRateResponseModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteTotalRate.SaveRate, saveModel);
    }


    uploadQuoteTotalRates(formData: FormData): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteTotalRate.UploadRate, formData);
    }

}
