import { environment } from 'src/environments/environment';

const controllerName = 'email';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const EmailUrls = {
    SendEmail: baseUrl + 'SendEmail',
    GetWriteEmailModel: baseUrl + 'GetWriteEmailModel',
    GetEmailHistory: baseUrl + 'GetEmailHistory',
    GetApplicationEmailHistory: baseUrl + 'GetApplicationEmailHistory',
};
