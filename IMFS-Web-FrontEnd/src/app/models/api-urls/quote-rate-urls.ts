import { environment } from 'src/environments/environment';

const controllerName = 'rate';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const QuoteRateUrls = {
    GetRate: baseUrl + 'GetRates',
    SaveRate: baseUrl + 'SaveRate',
    ExportRate: baseUrl + 'ExportRate',
    UploadRate: baseUrl + 'UploadRate'
};
