import { Home } from '../../models/home/home.model';
//import { RecentQuotes } from '../../models/home/home.model';
//import { RecentApplications } from '../../models/home/home.model';
//import { AwaitingInvoices } from '../../models/home/home.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private http: HttpClient) { }

  //formDataRecentQuotes: RecentQuotes = new RecentQuotes();
  //readonly baseURLRecentQuotes = 'https://localhost:44399/Quote/GetRecentQuotes';
  //listRecentQuotes: RecentQuotes[];

  //formDataRecentApplications: RecentApplications = new RecentApplications();
  //readonly baseURLRecentApplicaions = 'https://localhost:44399/Application/getRecentApplications';
  //listRecentApplicaions: RecentApplications[];

  //formDataAwaitingInvoices: AwaitingInvoices = new AwaitingInvoices();
  //readonly baseURLAwaitingInvoices = 'https://localhost:44399/Application/getAwaitingInvoices';
  //listAwaitingInvoices: AwaitingInvoices[];

  //getRecentQuotes() {
  //  this.http.get(this.baseURLRecentQuotes)
  //    .toPromise()
  //    .then(res => this.listRecentQuotes = res as RecentQuotes[]);
  //}

  //getRecentApplications() {
  //  this.http.get(this.baseURLRecentApplicaions)
  //    .toPromise()
  //    .then(res => this.listRecentApplicaions = res as RecentApplications[]);
  //}

  //getAwaitingInvoices() {
  //  this.http.get(this.baseURLAwaitingInvoices)
  //    .toPromise()
  //    .then(res => this.listAwaitingInvoices = res as AwaitingInvoices[]);
  //}
}
