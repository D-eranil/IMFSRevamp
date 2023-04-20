import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIUrls } from 'src/app/models/api-urls/api-url';
import { CurrentUserInfo, UserResponseModel, UserSearchModel } from 'src/app/models/user/user.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';


@Injectable()
export class UserControllerService {
    constructor(private http: HttpClient) { }

    checkUserStatus(): Observable<string> {
        return this.http.get<string>(APIUrls.User.CheckUserStatus);
      }

    getAllUsers(): Observable<UserResponseModel[]> {
        return this.http.get<UserResponseModel[]>(APIUrls.User.GetAllUsers, {});
    }

    activateUser(userId: string): Observable<HttpResponseData> {
        return this.http.get<HttpResponseData>(APIUrls.User.ActivateUser + '?userId=' + userId, {});
    }

    deactivateUser(userId: string): Observable<HttpResponseData> {
        return this.http.get<HttpResponseData>(APIUrls.User.DeactivateUser + '?userId=' + userId, {});
    }

    saveUser(saveModel: UserResponseModel): Observable<HttpResponseData> {
        return this.http.post<HttpResponseData>(APIUrls.User.SaveUser, saveModel);
    }

    searchUser(userSearchModel: UserSearchModel): Observable<UserResponseModel[]> {
        return this.http.post<UserResponseModel[]>(APIUrls.User.SearchUser, userSearchModel);
    }

    getCurrentUserInfo(): Observable<CurrentUserInfo> {
        return this.http.get<CurrentUserInfo>(APIUrls.User.GetCurrentUserInfo, {});
    }
}



