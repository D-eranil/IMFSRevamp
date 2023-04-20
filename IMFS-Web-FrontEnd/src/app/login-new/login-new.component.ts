import { Component, OnInit } from '@angular/core';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { ComponentVisibilityService } from '../services/utility-services/component-visibility.service';
import { OktaService } from '../services/utility-services/okta.service';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-new',
  templateUrl: './login-new.component.html',
  styleUrls: ['./login-new.component.scss']
})
export class LoginNewComponent implements OnInit {

  constructor(@Inject(DOCUMENT) private document: Document,
    private componentVisibilityService: ComponentVisibilityService,
    private imfsUtilityService: IMFSUtilityService,
    private oktaService: OktaService,
    private router: Router)
  {
    this.componentVisibilityService.publicPageFire(true);
  }

  ngOnInit(): void {
  }

  login() {
    this.oktaService.login();
  }

  register() {
    window.location.assign("https://au.ingrammicro.com/site/cms?page=Become-a-Reseller")
  }

  //loadStyle(styleName: string) {
  //  const head = this.document.getElementsByTagName('head')[0];
  //  let themeLink = this.document.getElementById(
  //    'client-theme'
  //  ) as HTMLLinkElement;
  //  if (themeLink) {
  //    themeLink.href = styleName;
  //  } else {
  //    const style = this.document.createElement('link');
  //    style.id = 'allCss';
  //    style.rel = 'stylesheet';
  //    style.href = `${styleName}`;
  //    head.appendChild(style);
  //  }
  //}
}
