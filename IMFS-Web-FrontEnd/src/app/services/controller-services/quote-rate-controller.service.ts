import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuoteRateResponseModel } from '../../models/quote-rate/quote-rate-response.model';
import { QuoteRateModel } from '../../models/quote-rate/quote-rate.model';
import { QuoteRateInputModel } from '../../models/quote-rate/quote-rate-input.model';


@Injectable()
export class QuoteRateControllerService {
    constructor(private http: HttpClient, private jsUtilityService: JsUtilityService) { }

    /*searchQuoteRates(keyword: string): Observable<QuoteRateModel> {
        return this.http.get<QuoteRateModel>(APIUrls.QuoteRate.SearchQuoteRates + '?keyword=' + keyword);
    }*/

    getQuoteRates(inputModel: QuoteRateInputModel): Observable<QuoteRateModel[]> {
        return this.http.post<QuoteRateModel[]>(APIUrls.QuoteRate.GetRate, inputModel);
    }


    exportQuoteRates(inputModel: QuoteRateInputModel): Observable<any> {
        const headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http
            .post(APIUrls.QuoteRate.ExportRate, inputModel, { headers, responseType: 'blob' as 'json', observe: 'response' })
            .pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }));
    }

    saveRates(saveModel: QuoteRateResponseModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteRate.SaveRate, saveModel);
    }


    uploadRates(formData: FormData): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.QuoteRate.UploadRate, formData);
    }

}
