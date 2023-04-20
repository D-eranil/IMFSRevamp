import { Component, OnInit, ViewChild } from '@angular/core';
import { ConfirmationService, ConfirmEventType, MessageService, LazyLoadEvent } from 'primeng/api';
import { Panel } from 'primeng/panel';
import { UserResponseModel, UserSearchModel } from 'src/app/models/user/user.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { UserControllerService } from 'src/app/services/controller-services/user-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { UserDetailsModalComponent } from '../user-details-modal/user-details-modal.component';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})

export class UserListComponent implements OnInit {

    users: UserResponseModel[];
    totalRecords: number;
    searchModel: UserSearchModel = new UserSearchModel();
    cols: any[];
    collapsed = false;

    loading: boolean;
    @ViewChild('userSearchPanel') userSearchPanel: Panel;
    @ViewChild('detailsModal') detailsModal: UserDetailsModalComponent;
    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private userControllerService: UserControllerService,
        private confirmationService: ConfirmationService

    ) { }

    ngOnInit() {
         this.loadUsers();
    }

    loadLazyUsers(event: LazyLoadEvent) {
        this.loading = true;
        const eve = JSON.stringify(event);

        /*
        setTimeout(() => {
            this.userControllerService.getUsers({lazyEvent: JSON.stringify(event)}).then(res => {
                this.customers = res.customers;
                this.totalRecords = res.totalRecords;
                this.loading = false;
            })
        }, 1000);
        */
    }

    loadUsers() {
        const that = this;
        that.imfsUtilityService.showLoading('Loading Users');
        that.userControllerService.getAllUsers().subscribe(
            (response: UserResponseModel[]) => {
                that.imfsUtilityService.hideLoading();
                that.users = response;
                this.collapsePanel();
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.hideLoading();
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading users');
            }

        );
    }

    togglePanel() {
        if (this.userSearchPanel) {
            this.userSearchPanel.collapsed = !this.userSearchPanel.collapsed;
        }
    }

    collapsePanel() {
        if (this.userSearchPanel) {
            this.userSearchPanel.collapsed = true;
        }
    }

    expandPanel() {
        if (this.userSearchPanel) {
            this.userSearchPanel.collapsed = false;
        }
    }

    userSearch() {
        console.log(this.searchModel);
        if (this.searchModel) {
            this.userControllerService.searchUser(this.searchModel).subscribe(
                (response: UserResponseModel[]) => {
                    this.imfsUtilityService.hideLoading();
                    this.users = response;
                    this.collapsePanel();
                },
                (err: any) => {
                    console.log(err);
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('error', 'Failed', 'Error updating user');
                }
            );


        }


    }

    clearAll() {
        this.searchModel.fullName = '';
        this.searchModel.firstName = '';
        this.searchModel.lastName = '';
        this.searchModel.email = '';
        this.searchModel.customerName = '';
        this.searchModel.customerNumber = '';
        this.searchModel.jobTitle = '';
    }

    editUser(editItem: UserResponseModel) {
        console.log('User:' + editItem);
        this.detailsModal.header = 'Edit User Info';
        this.detailsModal.open(editItem);
    }

    getUser(userItem: any) {
        if (userItem.active) {
            return true;
        }
        return false;
    }

    deactivateUser(editItem: UserResponseModel) {

        const confirmationMessage = 'Deactivate user ' + editItem.email + '?';
        this.confirmationService.confirm({
            message: confirmationMessage,
            header: 'Deactivate user',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.userControllerService.deactivateUser(editItem.id).subscribe(
                    (response: HttpResponseData) => {
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('success', 'Success', 'user updated successfully');
                        this.userSearch();
                    },
                    (err: any) => {
                        console.log(err);
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('error', 'Failed', 'Error updating user');
                    }
                );

            },
            reject: (type: any) => {

                switch (type) {
                    case ConfirmEventType.REJECT:
                        break;
                    case ConfirmEventType.CANCEL:
                        break;
                }
            }
        });

    }

    activateUser(editItem: UserResponseModel) {

        const confirmationMessage = 'Activate user ' + editItem.email + '?';
        this.confirmationService.confirm({
            message: confirmationMessage,
            header: 'Activate user',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.userControllerService.activateUser(editItem.id).subscribe(
                    (response: HttpResponseData) => {
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('success', 'Success', 'user updated successfully');
                        this.userSearch();
                    },
                    (err: any) => {
                        console.log(err);
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('error', 'Failed', 'Error updating user');
                    }
                );

            },
            reject: (type: any) => {

                switch (type) {
                    case ConfirmEventType.REJECT:
                        break;
                    case ConfirmEventType.CANCEL:
                        break;
                }
            }
        });

    }

}
