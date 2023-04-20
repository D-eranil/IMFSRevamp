import { environment } from 'src/environments/environment';

const controllerName = 'funder';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const FunderUrls = {
    GetFunders: baseUrl + 'GetFunders',
    SaveFunder: baseUrl + 'SaveFunder'
};
