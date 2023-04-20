import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from './../../../environments/environment';
import { IMFSUtilityService } from './imfs-utility-services';

@Injectable()

export class HttpsAuthRequestInterceptor implements HttpInterceptor {

    constructor(
        private router: Router,
        private injector: Injector,
        private imfsUtilityService: IMFSUtilityService) { }

    private get oktaService() {
        return this.injector.get(OAuthService);
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url.startsWith(environment.API_BASE)){
        req = req.clone({
            headers: req.headers.set('Access-Control-Allow-Origin', environment.APP_BASE)
        });
        req = req.clone({
            headers: req.headers.set('Access-Control-Allow-Credentials', 'true')
        });
        req = req.clone({
            headers: req.headers.set('Access-Control-Allow-Headers',
                'Cache-Control, Pragma, Origin, Authorization, Content-Type, X-Requested-With')
        });
        req = req.clone({
            headers: req.headers.set('Access-Control-Allow-Methods', 'GET, PUT, POST, OPTIONS')
        });

        if (this.oktaService.hasValidIdToken()) {
            req = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + this.oktaService.getAccessToken()),
            });
        }
        const idToken = this.oktaService.getIdToken();
        if (idToken) {
            req = req.clone({
                setHeaders: {
                    Token: idToken
                }
            });
        }
    }
        return next.handle(req).pipe(tap((event: HttpEvent<any>) => {
            if (event instanceof HttpResponse) {
                // do stuff with response if you want
            }
        }, (err: any) => {
            if (err instanceof HttpErrorResponse) {
                // forbidden error
                if (err.status === 403) {
                    this.imfsUtilityService.hideLoading();
                    this.router.navigate(['/']); // return to home page
                    this.imfsUtilityService.showToastr('error', 'Unauthorize', 'You are not authorised to do this action');
                }
            }
        }));
    }
}
