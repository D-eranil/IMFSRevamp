import { Component, OnInit } from '@angular/core';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { ComponentVisibilityService } from '../services/utility-services/component-visibility.service';
import { OktaService } from '../services/utility-services/okta.service';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(@Inject(DOCUMENT) private document: Document,
    private componentVisibilityService: ComponentVisibilityService,
    private imfsUtilityService: IMFSUtilityService,
    private oktaService: OktaService,
    private router: Router)
  {
    this.componentVisibilityService.publicPageFire(true);
  }

  ngOnInit() {
  }

  login() {
    this.oktaService.login();
  }

  register() {
    window.location.assign("https://au.ingrammicro.com/site/cms?page=Become-a-Reseller")
  }
}
