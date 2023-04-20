import { Location } from '@angular/common';
import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { combineLatest } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { Email, EmailAttachment, EmailViewModel, WriteEmailViewModel } from 'src/app/models/email/email.model';
import { QuoteDetailsResponseModel } from 'src/app/models/quote/quote.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { AttachmentControllerService } from 'src/app/services/controller-services/attachment-controller.service';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { QuoteControllerService } from 'src/app/services/controller-services/quote-controller.service';
import { IMFSFormService } from 'src/app/services/utility-services/imfs-form-service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { EmailUploadModalComponent } from '../email-upload-modal/email-upload-modal.component';
import { GetWriteEmailModel } from './../../models/email/email.model';


@Component({
    selector: 'app-email',
    templateUrl: './email.component.html',
    styleUrls: ['./email.component.scss']
})


export class EmailComponent implements OnInit {

    constructor(
        private formBuilder: FormBuilder,
        private imfsUtilities: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private formUtility: IMFSFormService,
        private quoteControllerService: QuoteControllerService,
        private emailControllerService: EmailControllerService,
        private attachmentControllerService: AttachmentControllerService,
        private router: Router,
        private route: ActivatedRoute,
        private confirmationService: ConfirmationService,
        private messageService: MessageService,
        private location: Location) {
    }
    quoteEmailForm: FormGroup;
    emailDetails = new EmailViewModel();

    direct: boolean = false;
    tempEmailId: string;
    filesToUpload: File[] = [];
    emailFiles: EmailAttachment[] = [];
    active = true;
    selectedFiles: {id: number, fileName: string} [] = [];

    quoteId: string;
    emailId: string;
    applicationId: string;
    applicationNumber: string;
    toEmail: string;

    @ViewChild('fileUpload') fileUpload: FileUpload;
    @ViewChild('uploadModal') uploadModal: EmailUploadModalComponent;


    ngOnInit(): void {

        const obsComb = combineLatest([this.route.paramMap, this.route.queryParams]);

        obsComb.pipe(takeWhile(() => this.active)).subscribe((params: any) => {
            const quoteId = params[1].quoteId;
            if (quoteId) {
                this.quoteId = quoteId;
            }

            const direct = Boolean(params[1].direct);
            if (direct) {
                this.direct = direct;
            }

            const emailId = params[1].emailId;
            if (emailId) {
                this.emailId = emailId;
            }

            const applicationId = params[1].applicationId;
            const applicationNumber = params[1].applicationNumber;
            console.log(params)
            if (applicationId) {
                this.applicationId = applicationId;
            }
            if (applicationNumber) {
                this.applicationNumber = applicationNumber;
            }

            const toEmailAddress = params[1].toEmail;
            if (toEmailAddress) {
                this.toEmail = toEmailAddress;
            }

        });
        this.initForm();
        if (this.emailId) {
            this.getWriteEmailModel('Quote', this.emailId);
            this.disableAllControls();
        }
        else if (this.applicationId) {
            this.getWriteEmailModel('Application', '');
        }
        else {
            this.getWriteEmailModel('Quote', '');
        }

    }


    addAttachmentToEmail(event: any) {

      const targetIndex = this.selectedFiles.findIndex(x => x.id === event.id);

      if (targetIndex > -1) {
        this.selectedFiles.splice(targetIndex, 1);
      } else {
      this.selectedFiles.push(event);
      }

      const targetIndexInEmail = this.emailFiles.findIndex(x => x.id === event.id);


      if (targetIndexInEmail > -1) {
        this.emailFiles.splice(targetIndexInEmail, 1);
      } else {
      this.emailFiles.push(event);
      }

      console.log(this.emailFiles)

    }


    initForm() {
        this.quoteEmailForm = this.formBuilder.group({
            FromEmail: new FormControl('', Validators.required),
            ToEmail: new FormControl('', Validators.required),
            CcEmail: new FormControl(''),
            SubjectEmail: new FormControl('', Validators.required),
            BodyEmail: new FormControl(''),
        });
    }

    getEmailId() {
        if (this.emailId) {
            return true;
        }
        return false;
    }


    disableAllControls() {
        this.quoteEmailForm.controls.FromEmail.disable();
        this.quoteEmailForm.controls.SubjectEmail.disable();
        this.quoteEmailForm.controls.BodyEmail.disable();
        this.quoteEmailForm.controls.ToEmail.disable();
        this.quoteEmailForm.controls.CcEmail.disable();
    }

    getEndUserEmail(quoteId: number) {

        this.quoteControllerService.getQuoteDetails(quoteId).subscribe(
            (response: QuoteDetailsResponseModel) => {
                console.log(response.quoteDetails.endUserDetails.endCustomerEmail);
                this.quoteEmailForm.controls.ToEmail.patchValue(response.quoteDetails.endUserDetails.endCustomerEmail);
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilities.showToastr('error', 'Failed', 'Error Saving quote');
            }
        );
    }

    getWriteEmailModel(emailMode: string, emailId: string) {
        const getWriteEmailModel = new GetWriteEmailModel();
        getWriteEmailModel.EmailId = +emailId;
        getWriteEmailModel.EmailMode = emailMode;
        if (this.quoteId) {
            getWriteEmailModel.QuoteId = +this.quoteId;
        }
        if (this.applicationId) {
            getWriteEmailModel.ApplicationId = +this.applicationId;
        }
        getWriteEmailModel.removeAcceptanceLink = this.direct;
        this.emailControllerService.getWriteEmailModel(getWriteEmailModel).subscribe(
            (response: WriteEmailViewModel) => {
                this.quoteEmailForm.controls.FromEmail.patchValue(response.fromAddress);
                this.quoteEmailForm.controls.SubjectEmail.patchValue(response.subject);
                this.quoteEmailForm.controls.BodyEmail.patchValue(response.body);

                if (this.emailId) {
                    this.quoteEmailForm.controls.ToEmail.patchValue(response.toAddress);
                }

                if (this.toEmail) {
                    this.quoteEmailForm.controls.ToEmail.patchValue(this.toEmail);
                }

                this.quoteEmailForm.controls.CcEmail.patchValue(response.ccEmail);
                this.tempEmailId = response.tempEmailId;

                if (this.emailId) {
                    this.getEmailAttachmentsByEmailId(this.emailId);
                }
                else {
                    this.getEmailAttachments();
                }
                if (this.quoteId) {
                    this.getEndUserEmail(Number(this.quoteId));
                }
            },
            (err: any) => {
                console.log(err);
            }
        );
    }


    sendEmail() {
        const email = new Email();
        email.FromAddress = this.quoteEmailForm.get('FromEmail')?.value;
        email.ToAddress = this.quoteEmailForm.get('ToEmail')?.value;
        email.CCEmail = this.quoteEmailForm.get('CcEmail')?.value;
        email.Subject = this.quoteEmailForm.get('SubjectEmail')?.value;
        email.Body = this.quoteEmailForm.get('BodyEmail')?.value;
        // Documents are fetched using tempEmailId
        email.TempEmailId = this.tempEmailId;
        email.QuoteId = this.quoteId;
        email.ApplicationId = this.applicationId;

        const selectedDocs: number [] = [];
        this.selectedFiles.forEach(
          ele => {
            selectedDocs.push(ele.id)
          }
        );

        email.quotesAttached = selectedDocs;
        this.imfsUtilities.showLoading('Sending Email...');
        // save quote details to server
        this.emailControllerService.saveEmail(email).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilities.hideLoading();
                this.imfsUtilities.showToastr('success', 'Success', 'Email sent Successfully:' + response.data);
                this.location.back();
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilities.hideLoading();
                this.imfsUtilities.showToastr('error', 'Failed', 'Error Sending Email');
            }
        );

    }

    removeAttachment(file: any) {

        if (!this.emailId) {
            this.imfsUtilities.showLoading('Deleting file...');
            this.attachmentControllerService.removeEmailAttachmentTempFile(file?.id).subscribe(
                (response: HttpResponseData) => {

                    this.imfsUtilities.hideLoading();
                    this.getEmailAttachments();
                },
                (err: any) => {
                    this.imfsUtilities.hideLoading();
                    console.log(err);
                    this.imfsUtilities.showToastr('error', 'Error', 'Unable to delete file.');
                }
            );
        }

    }


    attachFile() {
        this.uploadModal.open(this.tempEmailId);
    }

    cancelEmail() {
        this.location.back();
    }

    attachQuote() {

    }

    getEmailAttachments() {
      this.attachmentControllerService.getEmailAttachments(this.emailDetails.id, this.tempEmailId).subscribe(
        (response: EmailAttachment[]) => {
          response.forEach(
            attachment => {
              const index = this.emailFiles.findIndex(x => x.id === attachment.id)
              if(index < 0) {
                this.addAttachmentToEmail(attachment);
              }
            }
          )

        },
            (err: any) => {
                console.log(err);
                this.imfsUtilities.showToastr('error', 'Error', 'Unable to load attachments');
            }
        );
    }

    getEmailAttachmentsByEmailId(emailId: string) {
        this.attachmentControllerService.getEmailAttachments(emailId, this.tempEmailId).subscribe(
            (response: EmailAttachment[]) => {
                response.forEach(
                  attachment => {
                    const index = this.emailFiles.findIndex(x => x.id === attachment.id)
                    if(index < 0) {
                      this.addAttachmentToEmail(attachment);
                    }
                  }
                )
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilities.showToastr('error', 'Error', 'Unable to load attachments');
            }
        );
    }

    downloadAttachment(attachmentId: string, event: any) {
        event.preventDefault();
        if (!this.emailId) {
            this.downloadTempAttachment(attachmentId);
        } else {
            this.imfsUtilities.showLoading('Downloading..');
            this.attachmentControllerService.downloadEmailAttachment(attachmentId).subscribe(
                (res: any) => {
                    this.jsUtilityService.fileSaveAs(res);
                    this.imfsUtilities.hideLoading();
                },
                (err: any) => {
                    this.imfsUtilities.hideLoading();
                    console.log(err);
                    this.imfsUtilities.showToastr('error', 'Error', 'Unable to download file.');
                }
            );
        }
    }

    downloadTempAttachment(attachmentId: string) {
        this.imfsUtilities.showLoading('Downloading..');
        this.attachmentControllerService.downloadEmailTempAttachment(attachmentId).subscribe(
            (res: any) => {
                this.jsUtilityService.fileSaveAs(res);
                this.imfsUtilities.hideLoading();
            },
            (err: any) => {
                this.imfsUtilities.hideLoading();
                console.log(err);
                this.imfsUtilities.showToastr('error', 'Error', 'Unable to download file.');
            }
        );
    }



}
