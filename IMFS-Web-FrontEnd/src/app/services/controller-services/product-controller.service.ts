import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { ProductDetailsModel, ProductInputModel } from 'src/app/models/product/product.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';



@Injectable()
export class ProductControllerService {
    constructor(private http: HttpClient) { }

    getProductDetails(inputModel: ProductInputModel): Observable<any> {
        return this.http.post<any>(APIUrls.Product.GetProductDetails, inputModel);
    }



}
