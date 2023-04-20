import { environment } from 'src/environments/environment';

const controllerName = 'financeType';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const FinanceTypeUrls = {
    GetFinanceTypes: baseUrl + 'GetFinanceTypes',
    SaveFinanceType: baseUrl + 'SaveFinanceType'
};
