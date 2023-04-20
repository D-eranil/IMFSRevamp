import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Subject } from 'rxjs';
import { LoadingBarModel } from '../../models/utility-models/imfs-utility.model';


export type Severities = 'success' | 'info' | 'warning' | 'error';

@Injectable()
export class IMFSUtilityService {
    public displayDialogService = new Subject<any>();
    loadingChange: Subject<any> = new Subject<any>();

    loadingCount = 0;

    constructor(private messageService: MessageService) { }

    showToastr(severity: Severities, summary: string, detail: string) {
        switch (severity) {
            case 'success': {
                this.messageService.add({
                    key: 'globalToast',
                    severity: 'success',
                    summary,
                    detail
                });
                break;
            }
            case 'info': {
                this.messageService.add({
                    key: 'globalToast',
                    severity: 'info',
                    summary,
                    detail
                });
                break;
            }
            case 'warning': {
                this.messageService.add({
                    key: 'globalToast',
                    severity: 'warn',
                    summary,
                    detail,
                    sticky: true
                });
                break;
            }
            case 'error': {
                this.messageService.add({
                    key: 'globalToast',
                    severity: 'error',
                    summary,
                    detail
                });
                break;
            }
        }
    }

    showLoading(loadingMessage: string, showCancelButton = false) {
        const loadingBarModel: LoadingBarModel = new LoadingBarModel();
        loadingBarModel.showLoading = true;
        loadingBarModel.loadingMessage = loadingMessage;
        loadingBarModel.showCancelButton = showCancelButton;
        // loadingBarModel.cancelAction = cancelAction;
        this.loadingCount++;
        if (this.loadingCount > 0) {
            setTimeout(() => {
                this.loadingChange.next(loadingBarModel);
            }, 0);
        }
    }

    hideLoading() {
        this.loadingCount--;
        if (this.loadingCount <= 0) {
            this.loadingCount = 0;
            const loadingBarModel: LoadingBarModel = new LoadingBarModel();
            loadingBarModel.showLoading = false;
            loadingBarModel.loadingMessage = '';
            loadingBarModel.showCancelButton = false;
            setTimeout(() => this.loadingChange.next(loadingBarModel), 0);
        }
    }

    showDialog(title: string, contentHTML: string) {
        this.displayDialogService.next({ title, contentHTML });
    }


    showErrorModal(summary: string, detail: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'globalErrorToast',
            sticky: true,
            severity: 'error',
            summary,
            detail
        });
    }
}
