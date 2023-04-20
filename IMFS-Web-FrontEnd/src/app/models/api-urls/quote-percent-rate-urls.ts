import { environment } from 'src/environments/environment';

const controllerName = 'QuotePercentRate';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const QuotePercentRateUrls = {
    GetRate: baseUrl + 'GetQuotePercentRates',
    SaveRate: baseUrl + 'SaveQuotePercentRate',
    ExportRate: baseUrl + 'ExportQuotePercentRate',
    UploadRate: baseUrl + 'UploadQuotePercentRate'
};
