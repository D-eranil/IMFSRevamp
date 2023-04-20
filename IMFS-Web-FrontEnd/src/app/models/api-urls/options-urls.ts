import { environment } from 'src/environments/environment';

const controllerName = 'options';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const OptionsUrls = {
    GetTypes: baseUrl + 'GetTypes',
    GetCategories: baseUrl + 'GetCategories',
    GetStatus: baseUrl + 'GetStatus',
};
