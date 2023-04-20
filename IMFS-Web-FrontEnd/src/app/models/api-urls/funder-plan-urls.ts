import { environment } from 'src/environments/environment';

const controllerName = 'funderPlan';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const FunderPlanUrls = {
    GetFunderPlans: baseUrl + 'GetFunderPlans',
};
