import { environment } from './../../../environments/environment';
import { OktaAuthOptions } from '@okta/okta-auth-js';
import { AuthConfig } from 'angular-oauth2-oidc';

export const oktaDomain = environment.OktaDomain;
export const issuer = environment.OktaIssuer;
export const clientId = environment.OktaClientId;
export const redirectUri = environment.OktaRedirectUri;
export const silentRefreshRedirectUri = `${window.location.origin}/silent-refresh.html`;
export const discoveryUrl = environment.OktaDiscoveryUrl;
export const responseType = ['id_token', 'token'];
export const scopes = ['openid', 'profile', 'email'];
export const homeUri = environment.OktaHomeUri;
export const postLogoutRedirectUri = environment.OktaPostLogoutRedirectUri;

export const oktaAuthConfig: AuthConfig = {
    issuer,
    redirectUri,
    clientId,
    responseType: 'id_token token',
    scope: 'openid profile email',
    silentRefreshRedirectUri,
    useSilentRefresh: true,
    showDebugInformation: true,
    sessionChecksEnabled: true,
    timeoutFactor: 0.01,
    clearHashAfterLogin: false,
    oidc: true,
    requestAccessToken: true,
    postLogoutRedirectUri,
};


export const oktaAuthOptions: OktaAuthOptions =
{
    clientId,
    issuer,
    redirectUri,
    pkce: true
};
