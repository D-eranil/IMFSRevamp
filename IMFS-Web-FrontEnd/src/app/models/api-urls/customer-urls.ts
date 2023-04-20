import { environment } from 'src/environments/environment';

const controllerName = 'ORPCustomer';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const CustomerUrls = {
    GetCustomers: baseUrl + 'GetCustomers',
    GetCustomersContacts:baseUrl + 'GetCustomerContacts'
};
