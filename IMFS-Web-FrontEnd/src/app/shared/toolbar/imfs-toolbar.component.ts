import { AuthenticationService } from './../../services/utility-services/authenication.service';
import { OktaService } from './../../services/utility-services/okta.service';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Message } from 'primeng/api';
import { AppComponent } from '../../app.component';
import { ComponentVisibilityService } from '../../services/utility-services/component-visibility.service';
import { DatePipe } from '@angular/common';
import { DeclarationListEmitMode } from '@angular/compiler';

@Component({
    selector: 'app-imfs-toolbar',
    templateUrl: 'imfs-toolbar.component.html',
    styleUrls: ['imfs-toolbar.component.scss'],
    encapsulation: ViewEncapsulation.None,
})

export class ToolbarComponent implements OnInit {
    CurrentUserName: string;
    currentDate: Date;
    currentURL = this.router.url;

    constructor(
        public componentVisibilityService: ComponentVisibilityService,
        private authenticationService: AuthenticationService,
        private router: Router,
        public datepipe: DatePipe,
        private oktaService: OktaService,
        public app: AppComponent) {
    }

    ngOnInit() {
        this.currentDate = new Date();
        this.currentURL = this.router.url;
    }

    logOut() {
	this.oktaService.logout()
        this.oktaService.afterLogout();
  }

    navigateToHome() {
      this.router.navigate(['/home']);
   }

    getCurrentUserName() {
        const currentUser = this.authenticationService.getCurrentUserInfo();
        if (currentUser) {
            return currentUser.firstName;
        }
        return '';
    }
}
