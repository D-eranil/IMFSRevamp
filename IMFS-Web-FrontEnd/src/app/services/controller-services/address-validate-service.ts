import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { HttpClient } from "@angular/common/http";

import { environment } from 'src/environments/environment';


const baseUrl: string = environment.API_BASE + '/' + 'Quote' + '/';
@Injectable()
export class addressvalidateservice {


    url: string = "https://international-street.api.smartystreets.com/"
    key: string = "113674716974139577"

    constructor(private httpclient: HttpClient) {
    }

    /** Getting Address form Api */

    getaddressauto(search: string, country: string): Observable<any> {
        const headers = { 'content-type': 'application/json' }
       return this.httpclient.get<any>("https://international-autocomplete.api.smartystreets.com/lookup?key=" + this.key + "&search=" + search + "&country=" + country)
        
    }

}