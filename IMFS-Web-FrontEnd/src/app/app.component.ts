import { UserControllerService } from 'src/app/services/controller-services/user-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { OktaService } from './services/utility-services/okta.service';
import { Component, HostListener,  OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { IMFSRoutes } from './models/routes/imfs-routes';
import { environment } from './../environments/environment';
import { MenuComponent } from '../app/menu/menu.component';
import { ComponentVisibilityService } from './services/utility-services/component-visibility.service';
import { OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { AuthenticationService } from './services/utility-services/authenication.service';
import * as _ from 'lodash-es';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
  interval: any;
  timeLeft: number = environment.WaitForLogoutTime;


  @HostListener('window:mousemove')
  @HostListener('window:keydown')
  @HostListener('window:scroll')
  @HostListener('window:click')
  refreshUserState() {
    clearTimeout(this.userActivity);
    clearTimeout(this.interval);
    if(this.showInactivityAlert){
      this.showInactivityAlert = !this.showInactivityAlert;
      this.timeLeft = environment.WaitForLogoutTime;
    }
    this.setTimeout();
  }
  title = 'IMFS-Web-FrontEnd';
  fullWidth: boolean;
  activeTopbarItem: Element | null;
  topbarMenuButtonClick: boolean;
  topbarMenuClick: boolean;
  topbarMenuActive: boolean;

  isFullScreen = false;
  isPublicPage = false;
  currentURL = this.router.url;
  checkingUserStatus = false;

  userActivity: any;

  userInactive: Subject<any> = new Subject();
  showInactivityAlert: boolean = false;

  logoutAfterTime() {
    this.interval = setInterval(() => {
      if(this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.showInactivityAlert = false;
        this.oktaService.logout();
        this.oktaService.afterLogout();
        clearInterval(this.interval)
      }
    },1000)
  }

  constructor(
    private primengConfig: PrimeNGConfig,
    private router: Router,
    private activeRoute: ActivatedRoute,
    private authenticationService: AuthenticationService,
    private componentVisibilityService: ComponentVisibilityService,
    private imfsUtilityService: IMFSUtilityService,
    private oAuthService: OAuthService,
    private oktaService: OktaService,
    private userControllerService: UserControllerService
  ) {

    // console.log(`app - constructor - ${String(this.oktaService.hasValidIdToken())}`);
    this.oAuthService.events.subscribe(({ event }: any) => {
      // console.log(`app.component - constructor ${event.type}`);
      // console.log(event);
      switch (event.type) {
        case 'invalid_nonce_in_state': {
          this.oktaService.login();
          break;
        }
        case 'silently_refreshed':
        case 'token_received': {
          this.checkUserStatus();
          break;
        }
        case 'logout':
        case 'session_terminated': {
          this.oktaService.afterLogout();
          void this.router.navigate([IMFSRoutes.Login]);
          break;
        }
        case 'token_expires': {
          // this.oktaService.afterLogout();
          this.router.navigate(['/test']); 
          // void this.router.navigate([IMFSRoutes.Login]);
          break;
        }
      }
    });
    if (!this.oktaService.hasValidIdToken()) {
      this.oktaService.checkLoginState();
    } else {
      this.checkUserStatus();
    }
  }

  setTimeout() {
      const inactivityTime = this.router.url.split('?')[0] !== '/quote/quote-acceptance' ? environment.InactivityTime : environment.InactivityTimeOnQuoteAccept;
      this.userActivity = setTimeout(
        () => this.userInactive.next(undefined),
        inactivityTime
      );
  }

  @ViewChild('tabmenu') tabmenu: MenuComponent;

  ngOnInit() {
    this.primengConfig.ripple = true;

      if(this.oktaService.isLoggedIn){
        this.setTimeout();
        this.userInactive.subscribe(() => {
          this.showInactivityAlert = true;
          this.logoutAfterTime();
        });
        }



    this.componentVisibilityService.fullScreenOn().subscribe((isFullScreen) => {
      if (isFullScreen) {
        this.isFullScreen = isFullScreen;
      }
    });

    this.componentVisibilityService.publicPageOn().subscribe((isPublicPage) => {
      if (isPublicPage) {
        this.isPublicPage = isPublicPage;
      }
    });

    //this.currentURL = this.router.url == "/" || "/home" ? true : false;
    this.currentURL = this.router.url;
  }

  onWrapperClick() {

  }

  onTopbarMenuClick(event: Event) {
    this.topbarMenuClick = true;
  }

  checkUserStatus() {
    if (this.checkingUserStatus) { return; }
    this.checkingUserStatus = true;

    this.imfsUtilityService.showLoading('Loading User Data');
    this.userControllerService.checkUserStatus().subscribe(
      () => {
        this.checkingUserStatus = false;
        this.authenticationService.setCurrentUserInfo();
        this.imfsUtilityService.hideLoading();
      },
      (err: any) => {
        this.imfsUtilityService.showToastr('error', 'Error', 'Error loading data');
        this.imfsUtilityService.hideLoading();
      },
    );
  }

  changeOfRoutes() {
    this.isFullScreen = false;
    this.isPublicPage = false;
    let currentRoute = this.router.url;
    // console.log(`app - changeOfRoutes - ${String(this.oktaService.hasValidIdToken())} - ${currentRoute}`);

    const urlString = window.location.hash.split('?');
    const pathArray = urlString[0].split('/');
    pathArray.shift();
    const pathTo = pathArray.join('/');

    if (!this.oktaService.hasValidIdToken()) {
      if (this.router.url.indexOf('#') > -1) {
        currentRoute = this.router.url.substr(0, this.router.url.indexOf('#'));
      }

      const publicRoutes: string[] = ['/sso/call-back', '/quote/quote-acceptance'];

      if (!_.some(publicRoutes, function (publicRoute) {
        return _.startsWith(currentRoute, publicRoute);
      })){
        // console.log('app - changeOfRoutes - redirect to login');

        //void this.router.navigate(['/home'], {
        //  queryParams: { returnUrl: pathTo },
        //  queryParamsHandling: 'merge',
        //});
      }
    }
  }
}
