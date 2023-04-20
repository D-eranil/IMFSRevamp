import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { QuoteDetailsModel, QuoteDetailsResponseModel, QuoteDownloadInput, QuoteSearchModel, RecentQuoteModel } from 'src/app/models/quote/quote.model';
import { RateCalculateInputModel, RateCalculateResponseModel } from 'src/app/models/rateCalculate/rateCalculate-input.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { JsUtilityService } from './../utility-services/js-utility.service';
import { RecentQuotes } from '../../models/quote/quote.model';
import { ProductDetailsModel } from 'src/app/models/product/product.model';

@Injectable()
export class QuoteControllerService {
    constructor(
        private http: HttpClient,
        private jsUtilityService: JsUtilityService) { }

    getQuoteDetails(quoteId: number): Observable<QuoteDetailsResponseModel> {
        return this.http.get<QuoteDetailsResponseModel>(APIUrls.Quote.GetQuoteDetails + '?quoteId=' + quoteId.toString());
    }

    
    getProductDetailsBySkuOrVpn(paramsUrl: string): Observable<any> {
        return this.http.get<any>(APIUrls.Quote.GetProductsBySkuOrVpn + paramsUrl);
    }

    saveQuote(saveModel: QuoteDetailsModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Quote.SaveQuote, saveModel);
    }

    calculateRate(inputModel: RateCalculateInputModel): Observable<RateCalculateResponseModel> {
        return this.http.post<RateCalculateResponseModel>(APIUrls.Quote.CalculateRate, inputModel);
    }

    searchQuote(inputModel: QuoteSearchModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Quote.SearchQuote, inputModel);
    }

    lookupQuoteNumber(inputModel: QuoteSearchModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.Quote.LookupQuoteNumber, inputModel);
    }

    downloadQuote(inputModel: QuoteDownloadInput): Observable<any> {
        const headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http
            .post(APIUrls.Quote.DownloadQuote, inputModel, { headers, responseType: 'blob', observe: 'response' })
            .pipe(
                catchError((res: HttpErrorResponse) => {
                    return this.jsUtilityService.convertBlobToText(res.error);
                }),
            );
    }

    getRecentQuotes(): Observable<RecentQuotes> {
        return this.http.get<RecentQuotes>(APIUrls.Quote.GetRecentQuotes);
    }

    getQuoteFileList(Id: number): Observable<any> {
        let Source = 1
        return this.http.get(APIUrls.Quote.GetQuoteAttachments + "?" + "id=" + Id + '&source=' + Source)
    }

    public downloadQuoteAttachment(fileId: number): Observable<any> {
        return this.http.get(APIUrls.Quote.DownloadQuoteAttachment + '?fileId=' + fileId,
            { responseType: 'blob', observe: 'response' }).pipe(catchError((res: any) => {
                return this.jsUtilityService.convertBlobToText(res.error);
            }))
    }
} 
