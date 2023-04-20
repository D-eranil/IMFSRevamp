import { Component, OnInit, ViewChild } from '@angular/core';
import { FunderModel } from 'src/app/models/funder/funder.model';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FunderDetailsModalComponent } from '../funder-details-modal/funder-details-modal.component';

@Component({
  selector: 'app-funder',
  templateUrl: './funder-list.component.html',
})

export class FunderListComponent implements OnInit {

  funders: FunderModel[];

  @ViewChild('detailsModal') detailsModal: FunderDetailsModalComponent;
  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private funderControllerService: FunderControllerService
  ) { }

  ngOnInit() {
    this.loadFunders();
  }

  loadFunders() {
    const that = this;
    that.imfsUtilityService.showLoading('Loading Funders');
    that.funderControllerService.getFunders(true).subscribe(
      (response: FunderModel[]) => {
        that.imfsUtilityService.hideLoading();
        that.funders = response;
      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.hideLoading();
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading funders');
      }

    );
  }


  createNewFunder() {
    const newFunder = new FunderModel();
    newFunder.id = 0;
    this.detailsModal.header = 'Create New Funder';
    this.detailsModal.open(newFunder);
  }

  editFunder(editItem: FunderModel) {
    this.detailsModal.header = 'Funder Details';
    this.detailsModal.open(editItem);
  }

}
