import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { VendorModel } from 'src/app/models/vendor/vendor.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { VendorControllerService } from 'src/app/services/controller-services/vendor-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';

@Component({
  selector: 'app-vendor-details-modal',
  templateUrl: './vendor-details-modal.component.html',
})

export class VendorDetailsModalComponent implements OnInit {

  currentItem: VendorModel = new VendorModel();
  displayDialog = false;
  header = 'Vendor';

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private vendorControllerService: VendorControllerService
  ) { }

  ngOnInit() {

  }


  open(editItem: VendorModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
  }

  saveVendor() {

    // error check here
    // make sure all fields has values
    // if not show error and return;

    if (!this.currentItem.vendorCode) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Vendor Code');
      return;
    }

    if (!this.currentItem.vendorName) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Vendor Name');
      return;
    }

    this.imfsUtilityService.showLoading('Saving...');
    this.vendorControllerService.saveVendors(this.currentItem).subscribe(
      (response: HttpResponseData) => {
        this.imfsUtilityService.hideLoading();
        if (response.status === 'Success') {
          this.imfsUtilityService.showToastr('success', 'Save Successful', 'Vendor information saved.');
          this.displayDialog = false;
          this.refreshEmit.emit();
        } else {
          this.imfsUtilityService.showToastr('error', 'Error', response.message);
        }
      },
      (err: any) => {
        console.log(err);
        this.imfsUtilityService.hideLoading();
        this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save Vendor');
      }

    );
  }

}
