import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ComponentVisibliyModel } from '../../models/misc/component-visibility.model';
import { BroadcasterService } from './broadcaster.service';

@Injectable()

export class ComponentVisibilityService {

    ComponentVisibilities: ComponentVisibliyModel[];
    currentUrl: any;

    constructor(
        private router: Router,
        private broadcasterService: BroadcasterService
    ) {
        this.init();
    }

    init() {
        this.ComponentVisibilities = [];

        const shortcutBarVisibility: ComponentVisibliyModel = {
            ComponentName: 'shortcut-bar',
            ExcludeList: true,
            RoutePathList: ['/login', '/forgot-password', '/login/reset-password', '/login/change-password']
        };
        this.ComponentVisibilities.push(shortcutBarVisibility);

        const toolbarVisibility: ComponentVisibliyModel = {
            ComponentName: 'imfs-toolbar',
            ExcludeList: true,
            RoutePathList: ['/login', '/forgot-password', '/login/reset-password', '/login/change-password']
        };
        this.ComponentVisibilities.push(toolbarVisibility);

        const sidenavVisibility: ComponentVisibliyModel = {
            ComponentName: 'side-nav',
            ExcludeList: true,
            RoutePathList: ['/login', '/forgot-password', '/login/reset-password', '/login/change-password']
        };
        this.ComponentVisibilities.push(sidenavVisibility);
    }

    public isComponentVisible(componentName: string): boolean {

        let componentVisibility: ComponentVisibliyModel;
        let currentUrl = this.router.url;

        // check current url has query string
        if (currentUrl.indexOf('?') > -1) {
            currentUrl = currentUrl.substr(0, currentUrl.indexOf('?'));
        }
        componentVisibility = this.ComponentVisibilities.filter((item: any) => {
            return item.ComponentName === componentName;
        })[0];
        let result = true;

        if (componentVisibility && componentVisibility.RoutePathList.indexOf(currentUrl) > -1) {
            result = true;
        } else if (componentVisibility) {
            result = false;
        }

        if (componentVisibility && componentVisibility.ExcludeList) {
            result = !result;
        }
        return result;
    }

    fullScreenFire(isFullScreen: boolean): void {
        this.broadcasterService.broadcast('FullScreenService', isFullScreen);
    }

    fullScreenOn(): Observable<boolean> {
        return this.broadcasterService.on<boolean>('FullScreenService');
    }

    publicPageFire(isPublicPage: boolean): void {
        this.broadcasterService.broadcast('PublicPageService', isPublicPage);
    }

    publicPageOn(): Observable<boolean> {
        return this.broadcasterService.on<boolean>('PublicPageService');
    }

}
