import { environment } from 'src/environments/environment';

const controllerName = 'Content';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const ContentUrls = {
    GetSystemConfig: baseUrl + 'GetSystemConfig',
    GetSystemMessages: baseUrl + 'GetSystemMessages',
};
