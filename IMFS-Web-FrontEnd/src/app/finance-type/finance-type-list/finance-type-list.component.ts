import { Component, OnInit, ViewChild } from '@angular/core';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FinanceTypeDetailsModalComponent } from '../finance-type-details-modal/finance-type-details-modal.component';

@Component({
  selector: 'app-finance-type',
  templateUrl: './finance-type-list.component.html',
})

export class FinanceTypeListComponent implements OnInit {

  financeTypes: FinanceTypeModel[];

  @ViewChild('detailsModal') detailsModal: FinanceTypeDetailsModalComponent;
  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private financeTypeControllerService: FinanceTypeControllerService
  ) { }

  ngOnInit() {
    this.loadFinanceTypes();
  }

  loadFinanceTypes() {
    const that = this;
    that.imfsUtilityService.showLoading('Loading Finance Types');
    that.financeTypeControllerService.getFinanceTypes(true).subscribe(
      (response: FinanceTypeModel[]) => {
        that.imfsUtilityService.hideLoading();
        that.financeTypes = response;
      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.hideLoading();
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading finance types');
      }

    );
  }


  createNewFinanceType() {
    const newFinanceTypeModel = new FinanceTypeModel();
    newFinanceTypeModel.id = 0;
    this.detailsModal.header = 'Create New Finance Type';
    this.detailsModal.open(newFinanceTypeModel);
  }

  editFinanceType(editItem: FinanceTypeModel) {
    this.detailsModal.header = 'Finance Type Details';
    this.detailsModal.open(editItem);
  }

}
