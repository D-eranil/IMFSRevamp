import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CurrentUserInfo } from '../models/user/user.model';
import { AuthenticationService } from '../services/utility-services/authenication.service';
import { OktaService } from './../services/utility-services/okta.service';
import { HomeService } from '../services/controller-services/home-controller.service';
import { ApplicationControllerService } from '../services/controller-services/application-controller.service';
import { QuoteControllerService } from '../services/controller-services/quote-controller.service';
import { RecentQuotes } from '../models/quote/quote.model';
import { AwaitingInvoices, RecentApplications } from '../models/application/application.model';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { DatePipe } from '@angular/common';
import { IMFSUtilityService } from '../services/utility-services/imfs-utility-services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})

export class HomeComponent implements OnInit {
  providers: [DatePipe]
  recentQuotes: RecentQuotes;
  show: boolean;
  recentApplications: RecentApplications;
  showCheckApplicationButton: boolean = false;
  awaitingInvoices: AwaitingInvoices;
  claims: any;
  title = 'sso';
  imfsUtilityService: any;
  recentQuotesLoading: boolean;
  recentApplicationLoading: boolean;
  awaitingInvoicesLoading: boolean;
  currentDateTime: Date;
  resellerName?: string;
  currentURL: string;

  constructor(
    private router: Router,
    private imfsUtilities: IMFSUtilityService,
    public authenticationService: AuthenticationService,
    public homeService: HomeService,
    private quoteControllerServices: QuoteControllerService,
    private applicationControllerServices: ApplicationControllerService,
    public oktaService: OktaService) {
    console.log('home - constructor');
  }

  ngOnInit(): void {
    if (!localStorage.getItem('reloadOnce')) {
      localStorage.setItem('reloadOnce', 'no reload')
      location.reload()
    }
    console.log('home - ngOnInit');
    this.currentURL = this.router.url;
    this.getCurrentUser();
    this.quoteControllerServices.getRecentQuotes().subscribe(data => {
      this.recentQuotes = data;
    });

    this.applicationControllerServices.getRecentApplications().subscribe(data => {

      const checkCreatedStatus = data.recentAppDetails.some(el => el.status === 'Application Created');
      const checkPendingStatus = data.recentAppDetails.some(el => el.status === 'Application Pending');
      this.showCheckApplicationButton = checkCreatedStatus || checkPendingStatus;
      this.recentApplications = data;
    });

    this.applicationControllerServices.getAwaitingInvoices().subscribe(data => {
      this.awaitingInvoices = data;
    });

    this.currentDateTime = new Date();
    this.resellerName = this.authenticationService.getCurrentUserInfo() !== null
                        ? this.authenticationService.getCurrentUserInfo()?.companyName : "N/A";
  }

  getClaims(): any {
    if (this.oktaService.hasValidIdToken()) {
      return this.oktaService.getClaims();
    }
    return '';
  }

  editQueue(quote: any) {
    void this.router.navigate([IMFSRoutes.Quote], { queryParams: { id: quote.quoteNumber, mode: 'edit'}});
  }

  viewQueue(quote: any) {
    void this.router.navigate([IMFSRoutes.Quote], { queryParams: { id: quote.quoteNumber, mode: 'view' }});
  }

  editApplication(application: any) {
    void this.router.navigate([IMFSRoutes.Application], { queryParams: { id: application.id, appNo: application.applicationNumber, mode: 'edit' }});
  }

  viewApplication(application: any) {
    console.log(application)
    void this.router.navigate([IMFSRoutes.Application], { queryParams: { id: application.id, appNo: application.applicationNumber, mode: 'view' }});
  }

  checkApplications() {
    this.imfsUtilities.showLoading('Checking...');
    this.applicationControllerServices.GetDealStatus().subscribe(
      (response: any) => {
          console.log(response);
          this.imfsUtilities.hideLoading();
          if(!response?.error) {
            this.imfsUtilities.showToastr('success','Success', response?.message);
          } else {
            this.imfsUtilities.showToastr('error', 'Failed', response?.message);
          }

      },
      (err: any) => {
        console.log(err);
        this.imfsUtilities.hideLoading();
        if(err?.error?.error) {
          this.imfsUtilities.showToastr('error','Failed', err?.error?.message);
        }
      }
    );
  }

  getCurrentUser(): CurrentUserInfo | null {
    return this.authenticationService.getCurrentUserInfo();
  }
}
