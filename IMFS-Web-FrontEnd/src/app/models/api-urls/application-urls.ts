import { environment } from 'src/environments/environment';

const controllerName = 'application';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const ApplicationUrls = {
    GetApplicationDetails: baseUrl + 'GetApplicationDetails',
    getSignatoriesDetails: baseUrl + 'GetSignatories',
    saveContacts: baseUrl + 'SaveContacts',
    deleteContact: baseUrl + 'deleteContacts',
    SaveApplication: baseUrl + 'SaveApplication',
    SearchApplication: baseUrl + 'SearchApplication',
    LookupApplicationNumber: baseUrl + 'LookupApplicationNumber',
    GetDealStatus: baseUrl + 'GetDealStatus',
    GetContacts: baseUrl + 'GetContacts',
    GetExistingContacts: baseUrl + 'GetExistingContacts',
    DownloadApplication: baseUrl + 'DownloadApplication',
    RejectApplication: baseUrl + 'RejectApplication',
    CancelApplication: baseUrl + 'CancelApplication',
    RecentApplication: baseUrl + 'getRecentApplications',
    AwaitingInvoice: baseUrl + 'getAwaitingInvoices'
};
