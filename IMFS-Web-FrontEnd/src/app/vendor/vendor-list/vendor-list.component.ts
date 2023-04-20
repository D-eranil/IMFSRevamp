import { Component, OnInit, ViewChild } from '@angular/core';
import { VendorModel } from 'src/app/models/vendor/vendor.model';
import { VendorControllerService } from 'src/app/services/controller-services/vendor-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { VendorDetailsModalComponent } from '../vendor-details-modal/vendor-details-modal.component';

@Component({
  selector: 'app-vendor',
  templateUrl: './vendor-list.component.html',
})

export class VendorListComponent implements OnInit {

  vendors: VendorModel[];

  @ViewChild('detailsModal') detailsModal: VendorDetailsModalComponent;
  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private vendorControllerService: VendorControllerService
  ) { }

  ngOnInit() {
    this.loadVendors();
  }

  loadVendors() {
    const that = this;
    that.imfsUtilityService.showLoading('Loading Vendors');
    that.vendorControllerService.getVendors(true).subscribe(
      (response: VendorModel[]) => {
        that.imfsUtilityService.hideLoading();
        that.vendors = response;
      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.hideLoading();
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading vendors');
      }

    );
  }


  createNewVendor() {
    const newVendor = new VendorModel();
    newVendor.id = 0;
    this.detailsModal.header = 'Create New Vendor';
    this.detailsModal.open(newVendor);
  }

  editVendor(editItem: VendorModel) {
    this.detailsModal.header = 'Vendor Details';
    this.detailsModal.open(editItem);
  }

}
