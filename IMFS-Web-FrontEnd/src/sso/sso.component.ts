import { AfterViewInit, Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { Subscription } from 'rxjs';
import { IMFSRoutes } from '../models/routes/imfs-routes';
import { ComponentVisibilityService } from '../services/utility-services/component-visibility.service';

@Component({
    selector: 'app-root',
    templateUrl: './sso.component.html',
})

export class SSOComponent implements OnInit, AfterViewInit, OnDestroy {
    claims: any;
    title = 'sso';
    isAuthorize = false;
    oAuthSubscription: Subscription;

    constructor(private router: Router,
        private componentVisibilityService: ComponentVisibilityService,
        private oAuthService: OAuthService) {
        console.log('sso - constructor');
        this.oAuthSubscription = this.oAuthService.events.subscribe(({ type }: OAuthEvent) => {
            console.log(`sso.component - constructor ${type}`);
            switch (type) {
                case 'silently_refreshed':
                case 'token_received': {
                    console.log('sso - ngOnInit - redirect to home');
                    void this.router.navigate([IMFSRoutes.Home]);
                    break;
                }
            }
        });
    }

    ngOnInit(): void {
        console.log('login - ngOnInit');
        if (this.router.url.indexOf('#') === -1) {
          console.log('ad-sso - ngOnInit - redirect to login');
          void this.router.navigate([IMFSRoutes.Login]);
        } else {
          if (this.router.url.indexOf('error') > 0) {
            this.isAuthorize = false;
            this.componentVisibilityService.publicPageFire(true);
          } else {
            this.isAuthorize = true;
          }
        }
      }
    
      ngAfterViewInit() {
        setTimeout(() => {
          if (this.oAuthService.hasValidAccessToken()) {
            console.log('sso - ngAfterViewInit - hasValidAccessToken redirect to home');
            void this.router.navigate([IMFSRoutes.Home]);
          }
        }, 5000); // 5s
      }
    
      ngOnDestroy() {
        if (this.oAuthSubscription) {
          console.log('sso - ngOnDestroy - unsubscribe');
          this.oAuthSubscription.unsubscribe();
        }
      }
}
