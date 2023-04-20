import { environment } from 'src/environments/environment';

const controllerName = 'user';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const UserUrls = {
    CheckUserStatus: baseUrl + 'CheckUserStatus',
    GetAllUsers: baseUrl + 'GetAllUsers',
    GetUserDetails: baseUrl + 'GetUserDetails',
    SaveUser: baseUrl + 'SaveUser',
    ActivateUser: baseUrl + 'ActivateUser',
    DeactivateUser: baseUrl + 'DeactivateUser',
    SearchUser: baseUrl + 'SearchUser',
    GetCurrentUserInfo: baseUrl + 'GetCurrentUserInfo',
};
