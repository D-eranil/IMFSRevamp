import { environment } from 'src/environments/environment';

const controllerName = 'product';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const ProductUrls = {
    GetProductDetails: baseUrl + 'GetProductDetails',
};
 