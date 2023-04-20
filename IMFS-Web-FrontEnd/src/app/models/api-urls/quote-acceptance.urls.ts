import { environment } from 'src/environments/environment';

const controllerName = 'quoteAcceptance';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const QuoteAcceptanceUrls = {
    SendCodeEmail: baseUrl + 'SendCodeEmail',
    VerifyCode: baseUrl + 'VerifyCode',
    GetDecodedQuoteId: baseUrl + 'GetDecodedQuoteId',
    RequestChangesQuote: baseUrl + 'RequestChangesQuote',
    RejectQuote: baseUrl + 'RejectQuote',
};
