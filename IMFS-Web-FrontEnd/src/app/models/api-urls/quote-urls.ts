import { environment } from 'src/environments/environment';

const controllerName = 'quote';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const QuoteUrls = {
    GetQuoteDetails: baseUrl + 'GetQuoteDetails',
    SaveQuote: baseUrl + 'SaveQuote',
    CalculateRate: baseUrl + 'CalculateRate',
    SearchQuote: baseUrl + 'SearchQuote',
    LookupQuoteNumber: baseUrl + 'LookupQuoteNumber',
    DownloadQuote: baseUrl + 'DownloadQuote',
    GetRecentQuotes: baseUrl + 'GetRecentQuotes',
    GetQuoteAttachments: baseUrl + 'GetQuoteAttachments',
    DownloadQuoteAttachment: baseUrl + 'DownloadQuoteAttachment',
    GetProductsBySkuOrVpn: baseUrl + 'GetProductsBySkuOrVpn',
};  
