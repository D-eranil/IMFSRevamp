import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuotePercentRateInputModel } from 'src/app/models/quote-percent-rate/quote-percent-rate-input.model';
import { QuotePercentRateModel } from 'src/app/models/quote-percent-rate/quote-percent-rate.model';
import { QuotePercentRateResponseModel } from 'src/app/models/quote-percent-rate/quote-percent-rate-response.model';


@Injectable()
export class QuotePercentRateControllerService {
    constructor(private http: HttpClient, private jsUtilityService: JsUtilityService) { }

    /*searchQuoteRates(keyword: string): Observable<QuoteRateModel> {
        return this.http.get<QuoteRateModel>(APIUrls.QuoteRate.SearchQuoteRates + '?keyword=' + keyword);
    }*/

    getQuotePercentRates(inputModel: QuotePercentRateInputModel): Observable<QuotePercentRateModel[]> {
        return this.http.post<QuotePercentRateModel[]>(APIUrls.QuotePercentRate.GetRate, inputModel);
    }


    exportQuotePercentRates(inputModel: QuotePercentRateInputModel): Observable<any> {
        const headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http
            .post(APIUrls.QuotePercentRate.ExportRate, inputModel, { headers, responseType: 'blob' as 'json', observe: 'response' })
            .pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }));
    }

    saveQuotePercentRates(saveModel: QuotePercentRateResponseModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuotePercentRate.SaveRate, saveModel);
    }


    uploadQuotePercentRates(formData: FormData): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuotePercentRate.UploadRate, formData);
    }

}
