import { environment } from 'src/environments/environment';

const controllerName = 'vendor';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const VendorUrls = {
    GetVendors: baseUrl + 'GetVendors',
    SaveVendor: baseUrl + 'SaveVendor'
};
