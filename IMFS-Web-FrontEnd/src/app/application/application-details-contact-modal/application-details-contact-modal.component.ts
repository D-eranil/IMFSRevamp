import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ApplicationContact } from 'src/app/models/application/application.model';
import { ApplicationContactData } from 'src/app/models/applicationContact/applicationContact.model';
import { ContactTypeOptions } from 'src/app/models/drop-down-options/drop-down-options.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { ApplicationControllerService } from 'src/app/services/controller-services/application-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';


@Component({
    selector: 'app-application-details-contact-modal',
    templateUrl: './application-details-contact-modal.component.html',
    styleUrls: ['./application-details-contact-modal.component.scss']
})

export class ApplicationDetailsContactModalComponent implements OnInit {

    currentItem: ApplicationContactData = new ApplicationContactData();
    displayDialog = false;
    header = 'Application Contact';
    guarantorContact = false;
    trusteeContact = false;
    todayDate: Date;
    contactTypes = ContactTypeOptions;
    selectedContact: any;
    contacts: ApplicationContact[] = [];
    filteredContacts: any [] = [];
    // filteredContacts: ApplicationContact[] = [];
    contactType: string;
    selectedContactType: string;
    resellerId: string;

    @Output() refreshEmit = new EventEmitter();


    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private applicationControllerService: ApplicationControllerService,

    ) { }

    ngOnInit() {

        this.todayDate = new Date();

    }

    open(type: string, resellerId: string) {

        this.resellerId = resellerId;
        this.contactType = type;



        if (type === 'Guarantor') {
            this.selectedContactType = '4';
            this.guarantorContact = true;
            this.trusteeContact = false;
          }

          if (type === 'Trustee') {
          this.selectedContactType = '5';
          this.guarantorContact = false;
          this.trusteeContact = true;
        }

        if (type === 'BeneficialOwner') {
          this.selectedContactType = '7';
            this.guarantorContact = false;
            this.trusteeContact = false;
        }


        if(this.selectedContact?.contactType) {
          this.selectedContact.contactType = this.selectedContactType;
          this.currentItem = new ApplicationContactData;
        }

        if(this.currentItem?.contactType) {
          this.currentItem.contactType = Number(this.selectedContactType);
          this.selectedContact = null;
        }



        this.getContacts(this.selectedContactType);
        this.displayDialog = true;
        // this.currentItem = _.cloneDeep(editItem);
    }

    getContacts(selectedContactType: string) {
        this.imfsUtilityService.showLoading('Loading...');
        this.applicationControllerService.getExistingContacts(this.resellerId,Number(selectedContactType)).subscribe(
            (response: any) => {
            // (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();

                // response = {
                //   "existingContacts": [
                //     {
                //       "contactID": "5505b420-ab3b-423d-95eb-8c66982fc851",
                //       "resellerID": null,
                //       "contactName": "asdasf",
                //       "contactEmail": "dfgdg@dfgm.gdd",
                //       "contactAddress": "34345asds",
                //       "contactDOB": '',
                //       "contactDriversLicNo": "3454353",
                //       "contactPhone": "343434343443",
                //       "contactTypeDescription": "Trustee",
                //       "contactABNACN": "345345",
                //       "contactType": 4,
                //       "isContactSignatory": true
                //     },
                //     {
                //       "contactID": "977dcb1b-3e1d-476b-bc74-9c7a1cbe8ad8",
                //       "resellerID": null,
                //       "contactName": "Test 1",
                //       "contactEmail": "Test1@test.com",
                //       "contactAddress": "Test 1",
                //       "contactDOB": '',
                //       "contactDriversLicNo": "55555",
                //       "contactPhone": "55555",
                //       "contactTypeDescription": "Trustee",
                //       "contactABNACN": "5555",
                //       "contactType": 7,
                //       "isContactSignatory": true
                //     },
                //     {
                //       "contactID": "6a777853-c5d1-480b-9503-4ee5575bc79f",
                //       "resellerID": null,
                //       "contactName": "test trustee contact",
                //       "contactEmail": "test@email.com",
                //       "contactAddress": "Shop 4/5",
                //       "contactDOB": '',
                //       "contactDriversLicNo": "78945623",
                //       "contactPhone": "0421346888",
                //       "contactTypeDescription": "Trustee",
                //       "contactABNACN": "567686867887",
                //       "contactType": 5,
                //       "isContactSignatory": false
                //     },
                //     {
                //       "contactID": "53f40767-eeab-4ee2-b58c-d0a16a5fda92",
                //       "resellerID": null,
                //       "contactName": "test2",
                //       "contactEmail": "test2@dotstark.com",
                //       "contactAddress": "test2",
                //       "contactDOB": '',
                //       "contactDriversLicNo": "333",
                //       "contactPhone": "333",
                //       "contactTypeDescription": "Trustee",
                //       "contactABNACN": "333",
                //       "contactType": 4,
                //       "isContactSignatory": true
                //     }
                //   ],
                //   "hasError": false,
                //   "errorMessage": ""};

                this.filteredContacts = response.existingContacts;

                if (this.contactType === 'Guarantor') {
                    const filteredContactsByType = this.filteredContacts.filter(c => c.contactType === 1 ||
                         c.contactType === 2 ||  c.contactType === 3 ||  c.contactType === 4);
                    this.contacts = filteredContactsByType;
                }

                if (this.contactType === 'Trustee') {
                    const filteredContactsByType = this.filteredContacts.filter(c => c.contactType === 5);
                    this.contacts = filteredContactsByType;
                }

                if (this.contactType === 'BeneficialOwner') {
                    const filteredContactsByType = this.filteredContacts.filter(c => c.contactType === 7);
                    this.contacts = filteredContactsByType;
                }
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in getting quote');
            }
        );
    }



    // open(editItem: ApplicationContact) {
    //     this.displayDialog = true;
    //     this.currentItem = _.cloneDeep(editItem);
    // }

    saveContact() {
        if (this.selectedContact?.contactName) {

          if (!this.selectedContact.contactName) {
            this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Name');
            return;
        }

        if (!this.selectedContact.contactEmail) {
            this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Email');
            return;
        }

        if (!this.selectedContact.contactAddress) {
            this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Address');
            return;
        }

        if (!this.selectedContact.contactDriversLicNo) {
            this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Drivers Licence No');
            return;
        }

        if (!this.selectedContact.contactPhone) {
            this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Phone');
            return;
        }

            this.selectedContact.contactType = this.selectedContactType;
            this.refreshEmit.emit(this.selectedContact);
        }
        else {

            if (!this.currentItem.fullName) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Name');
                return;
            }

            if (!this.currentItem.email) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Email');
                return;
            }

            if (!this.currentItem.residentialAddress) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Address');
                return;
            }

            if (!this.currentItem.dob) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter DoB');
                return;
            }

            if (!this.currentItem.driversLicNo) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Drivers Licence No');
                return;
            }

            if (!this.currentItem.phone) {
                this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Phone');
                return;
            }


            if (this.contactType === 'Guarantor') {

                if (!this.selectedContactType) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please select Contact Type');
                    return;
                }
                else {
                    this.currentItem.type = this.contactType;
                }

            }

            if (this.contactType === 'Trustee') {
                this.currentItem.type = this.contactType;
                // this.currentItem.contactType = 5;
                if (!this.currentItem.abn) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Contact ABN');
                    return;
                }
            }

            if (this.contactType === 'BeneficialOwner') {
                this.currentItem.type = this.contactType;
            }
            this.currentItem.contactType = Number(this.selectedContactType);

            this.refreshEmit.emit(this.currentItem);

        }

        this.displayDialog = false;

        // this.imfsUtilityService.showLoading('Saving...');

        // this.financeProductTypeControllerService.saveFinanceProductTypes(this.currentItem).subscribe(
        //     (response: HttpResponseData) => {
        //         this.imfsUtilityService.hideLoading();
        //         if (response.status === 'Success') {
        //             this.imfsUtilityService.showToastr('success', 'Save Successful', 'Finance Product Type information saved.');
        //             this.displayDialog = false;
        //             this.refreshEmit.emit();
        //         } else {
        //             this.imfsUtilityService.showToastr('error', 'Error', response.message);
        //         }
        //     },
        //     (err: any) => {
        //         console.log(err);
        //         this.imfsUtilityService.hideLoading();
        //         this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save finance Product type');
        //     }

        // );
    }

}
