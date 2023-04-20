import { OktaService } from './okta.service';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { CurrentUserInfo } from 'src/app/models/user/user.model';
import { UserControllerService } from 'src/app/services/controller-services/user-controller.service';
import { IMFSUtilityService } from './imfs-utility-services';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { Router } from '@angular/router';

@Injectable()
export class AuthenticationService {
    constructor(
        private userControllerService: UserControllerService,
        private oktaServicce: OktaService,
        private router: Router,
        private imfsUtilityService: IMFSUtilityService,
        private cookieService: CookieService) { }

    setCurrentUserInfo() {
        this.userControllerService.getCurrentUserInfo().subscribe(
            (response: CurrentUserInfo) => {
                if (response) {
                    this.cookieService.set('userData', JSON.stringify(response));
                }
            },
            (err: any) => {
                this.imfsUtilityService.showToastr('error', 'Error', 'Error loading data');
            },
        );
    }

    userHasRole(roleName: string) {
        // const currentUser = this.getCurrentUserInfo();
        const currentUser = {
          role: 'IMStaffAdmin'
        }
        return currentUser?.role === roleName;
    }

    getCurrentUserInfo(): CurrentUserInfo | null {
        if (this.oktaServicce.hasValidIdToken()) {
            const userData = this.cookieService.get('userData');
            if (userData) {
                return JSON.parse(userData) as CurrentUserInfo;
            }
        }

        return null;
    }

}
