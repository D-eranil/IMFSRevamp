import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { CustomerModel, CustomerResponseModel, CustomerContactResponseModel } from 'src/app/models/customer/customer.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';



@Injectable()
export class ORPCustomerControllerService {
    constructor(private http: HttpClient) { }
    getCustomer(customerName: string): Observable<CustomerResponseModel> {
        return this.http.get<CustomerResponseModel>(APIUrls.Customer.GetCustomers + '?customerName=' + customerName, {});
    }
    getCustomerContact(customerNumber: string): Observable<CustomerContactResponseModel> {
        return this.http.get<CustomerContactResponseModel>(APIUrls.Customer.GetCustomersContacts + '?customerNumber=' + customerNumber, {});
    }


}
