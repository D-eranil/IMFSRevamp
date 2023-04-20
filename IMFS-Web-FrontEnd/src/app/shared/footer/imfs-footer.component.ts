import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Message } from 'primeng/api';
import { AppComponent } from '../../app.component';
import { ComponentVisibilityService } from '../../services/utility-services/component-visibility.service';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-imfs-footer',
    templateUrl: 'imfs-footer.component.html',
    styleUrls: ['imfs-footer.component.scss'],
    encapsulation: ViewEncapsulation.None,
})

export class FooterComponent implements OnInit {

    constructor(
        public componentVisibilityService: ComponentVisibilityService,
        private router: Router,
        public datepipe: DatePipe,
        private fb: FormBuilder,
        public app: AppComponent) {
    }

    ngOnInit() {

    }

    logOut() {
        // this.authenticationService.logout();
    }

}
