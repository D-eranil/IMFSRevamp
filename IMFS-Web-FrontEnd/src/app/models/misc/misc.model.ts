
import { SelectItem } from 'primeng/api';
import { ConfigValuesModel, SystemMessagesModel } from '../config/config.model';
import { SignIn, TokenResponse } from '../login/login.model';
import { NotificationItem } from '../notification/notification.model';

export class ConfigData {
    public systemConfigData: SystemConfigResponse;
    public userInfo: string;
}

export class SystemConfigResponse {
    public orpUrl: string;
    public navItems: TopNavItem[];
    public ConfigValues: ConfigValuesModel;
    public navigationItems: NavigationItem[];
}



export class TopNavItem {
    public name: string;
    public route: string;
    public dynamicLink: boolean;
    public iconClass: string;
    public hasChildren: boolean;
    public itemState: string;
    public isExternal: boolean;
    public routeParam: any;         // not implemented yet in menu
    public queryParam: any;
    public includeRoles: string;
    public excludeRoles: string;
    public includeResources: string;
    public children: TopNavItem[];
    constructor() {
        this.itemState = 'collapsed';
        this.dynamicLink = false;
    }
}

export class TopNavEventData {
    open: boolean;
}

export class NavigationItem {
    public name: string;
    public route: string;
    public paramName: string;
    public isExternal: boolean;
}

export interface QueryUrlModel {
    [name: string]: string;
}

export class SystemMessagesResponse {
    SystemMessages: SystemMessagesModel;
    Notifications: NotificationItem[];
}

export class AsignedPropertyUpdated {
    propertyToUpdate: string;
    value: any;
}


export class LookupItemsResponse {
    lookupItems: SelectItem[];
}

export class DivisionModel {
    public Id: number;
    public Name: string;
}

export class TokenGeneratedArg {
    public tokenResponse: TokenResponse;
    public signIn: SignIn;
    public twoFactorReequired: string;
}
export class RedirectToJobArg {
    public jobId: number;
    public openOnNewTab: boolean;
}

export class RedirectToEmailArg {
    public emailId: number;
    public jobId: number;
}

export class QuickSearchModel {
    RedirectRoute: string;
    RouteParam: string;
}
