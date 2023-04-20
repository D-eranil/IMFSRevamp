import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { FunderModel } from 'src/app/models/funder/funder.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';

@Component({
  selector: 'app-funder-details-modal',
  templateUrl: './funder-details-modal.component.html',
})

export class FunderDetailsModalComponent implements OnInit {

  currentItem: FunderModel = new FunderModel();
  displayDialog = false;
  header = 'Funder';

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private funderControllerService: FunderControllerService
  ) { }

  ngOnInit() {

  }


  open(editItem: FunderModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
  }

  saveFunder() {

    // error check here
    // make sure all fields has values
    // if not show error and return;

    if (!this.currentItem.funderCode) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Funder Code');
      return;
    }

    if (!this.currentItem.funderName) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Funder Name');
      return;
    }


    this.imfsUtilityService.showLoading('Saving...');
    // tslint:disable-next-line: deprecation
    this.funderControllerService.saveFunders(this.currentItem).subscribe(
      (response: HttpResponseData) => {
        this.imfsUtilityService.hideLoading();
        if (response.status === 'Success') {
          this.imfsUtilityService.showToastr('success', 'Save Successful', 'Funder information saved.');
          this.displayDialog = false;
          this.refreshEmit.emit();
        } else {
          this.imfsUtilityService.showToastr('error', 'Error', response.message);
        }
      },
      (err: any) => {
        console.log(err);
        this.imfsUtilityService.hideLoading();
        this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save funder');
      }

    );
  }

}
