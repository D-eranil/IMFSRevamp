import { Component } from '@angular/core';
import { IMFSUtilityService } from './services/utility-services/imfs-utility-services';

@Component({
  selector: 'app-test',
  templateUrl: './test.component.html',
})
export class TestComponent {
  title = 'IMFS-Web-FrontEnd';

  constructor(private imfsUtilityService: IMFSUtilityService) {

  }

  showMessage() {
    this.imfsUtilityService.showToastr('error', 'Successful', 'Message is displayed successfully');
  }

  showLoading() {
    this.imfsUtilityService.showLoading('Please wait');
  }

  showDialog() {
    this.imfsUtilityService.showDialog('My Dialog', 'This is <b>html</b> string');
  }
}
