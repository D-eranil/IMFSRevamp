import { environment } from 'src/environments/environment';

const controllerName = 'QuoteTotalRate';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const QuoteTotalRateUrls = {
    GetRate: baseUrl + 'GetQuoteTotalRates',
    SaveRate: baseUrl + 'SaveQuoteTotalRate',
    ExportRate: baseUrl + 'ExportQuoteTotalRate',
    UploadRate: baseUrl + 'UploadQuoteTotalRate'
};
