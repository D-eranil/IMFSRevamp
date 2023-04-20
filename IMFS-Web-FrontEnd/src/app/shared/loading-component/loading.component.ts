import { IMFSUtilityService } from './../../services/utility-services/imfs-utility-services';
import { LoadingBarModel } from './../../models/utility-models/imfs-utility.model';
import { Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss']
})

export class LoadingComponent implements OnInit, OnDestroy {
  loadingMessage: string;
  showCancelButton: boolean;
  showLoading: boolean;
  cancelAction: () => void;
  subscription: Subscription;
  currentURL: string;

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private router: Router)
  { }

  ngOnInit() {
    this.showLoading = false;
    this.subscribeToLoadingChange();
    this.currentURL = this.router.url;
  }

  subscribeToLoadingChange() {
    this.subscription = this.imfsUtilityService.loadingChange.subscribe((loadingBarModel: LoadingBarModel) => {
      this.showLoading = loadingBarModel.showLoading;
      this.loadingMessage = loadingBarModel.loadingMessage;
      this.showCancelButton = loadingBarModel.showCancelButton;
      this.cancelAction = loadingBarModel.cancelAction;
    });
  }

  cancelClick() {
    this.cancelAction();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
