import { Component, OnInit, ViewChild } from '@angular/core';
import { FinanceProductTypeModel } from 'src/app/models/finance-product-type/finance-product-type.model';
import { FinanceProductTypeControllerService } from 'src/app/services/controller-services/finance-product-type-controller.servcie';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import {FinanceProductTypeDetailsModalComponent} from '../finance-product-type-details-modal/finance-product-type-details-modal.component';

@Component({
  selector: 'app-finance-product-type',
  templateUrl: './finance-product-type-list.component.html',
})

export class FinanceProductTypeListComponent implements OnInit {

  financeProductTypes: FinanceProductTypeModel[];

  @ViewChild('detailsModal') detailsModal: FinanceProductTypeDetailsModalComponent;
  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private financeProductTypeControllerService: FinanceProductTypeControllerService
  ) { }

  ngOnInit() {
    this.loadFinanceProductTypes();
  }

  loadFinanceProductTypes() {
    const that = this;
    that.imfsUtilityService.showLoading('Loading Finance Types');
    that.financeProductTypeControllerService.getFinanceProductTypes(true).subscribe(
      (response: FinanceProductTypeModel[]) => {
        that.imfsUtilityService.hideLoading();
        that.financeProductTypes = response;
      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.hideLoading();
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading finance types');
      }

    );
  }

  createNewFinanceProductType() {
    const newFinanceProductTypeModel = new FinanceProductTypeModel();
    newFinanceProductTypeModel.id = 0;
    this.detailsModal.header = 'Create New Finance Product Type';
    this.detailsModal.open(newFinanceProductTypeModel);
  }

  editFinanceProductType(editItem: FinanceProductTypeModel) {
    this.detailsModal.header = 'Finance Product Type Details';
    this.detailsModal.open(editItem);
  }

}
