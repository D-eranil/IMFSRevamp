import { environment } from 'src/environments/environment';

const controllerName = 'financeProductType';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const FinanceProductTypeUrls = {
    GetFinanceProductTypes: baseUrl + 'GetFinanceProductTypes',
    SaveFinanceProductType: baseUrl + 'SaveFinanceProductType'
};
