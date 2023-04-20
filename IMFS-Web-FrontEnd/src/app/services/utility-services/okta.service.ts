 import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OktaAuth } from '@okta/okta-auth-js';
import { OAuthErrorEvent, OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { clientId, discoveryUrl, oktaAuthConfig, oktaAuthOptions, redirectUri, responseType, scopes } from 'src/app/models/config/okta-sso-config';


@Injectable()
export class OktaService {

    private authClient: OktaAuth;
    private ssoInterval: any;
    public isLoggedIn = false;
    constructor(private oAuthService: OAuthService, private router: Router) {
        this.oAuthService.configure(oktaAuthConfig);
        this.oAuthService.tokenValidationHandler = new JwksValidationHandler();
        void this.oAuthService.loadDiscoveryDocument(discoveryUrl);
        this.oAuthService.setStorage(localStorage);
        this.oAuthService.responseType = 'id_token';
        // this.oAuthService.setupAutomaticSilentRefresh();
        this.oAuthService.events.subscribe((event) => {
            if (event instanceof OAuthErrorEvent) {
                console.log(`sso.service - eventSubscribe Error - ${event.type}`);
            } else if (event.type === 'invalid_nonce_in_state') {
                this.oAuthService.initImplicitFlow();
            } else if (event.type === 'session_terminated') {
                console.log(`sso.service - session terminated`);
            } else if (event.type !== 'discovery_document_loaded') {
                console.log(`sso.service - eventSubscribe - ${event.type}`);
            }
        });

        console.log(`sso.service - constructor - hasValidIdToken = ${String(this.hasValidIdToken())}`);

        this.authClient = new OktaAuth(oktaAuthOptions);
        if (this.hasValidIdToken()) {
            this.isLoggedIn = true;
        }

    }


    hasValidIdToken(): boolean {
        if (this.oAuthService.hasValidIdToken()) {
            return true;
        }
        return false;
    }

    getAccessToken(): string | null {
        if (this.oAuthService.hasValidIdToken()) {
            return this.oAuthService.getAccessToken();
        }
        return null;
    }

    getIdToken(): string | null {
        if (this.oAuthService.hasValidIdToken()) {
            return this.oAuthService.getIdToken();
        }
        return null;
    }

    getAccessTokenExpiration(): Date | null {
        if (this.oAuthService.hasValidIdToken()) {
            const milliSeconds = this.oAuthService.getAccessTokenExpiration();
            const expiryDate = new Date(milliSeconds);
            return expiryDate;
        }
        return null;
    }

    getClaims(): any {
        if (this.oAuthService.hasValidIdToken()) {
            return this.oAuthService.getIdentityClaims();
        }
        return null;
    }

    loadUserInfo(): any {
        if (this.oAuthService.hasValidIdToken()) {
            return this.oAuthService.loadUserProfile();
        }
        return null;
    }

    logout(): void {
        this.oAuthService.logoutUrl = '';
        localStorage.removeItem('reloadOnce')
        if (this.oAuthService.logoutUrl) {
            this.oAuthService.logOut();
            this.afterLogout();
        } else {
            // realod discovery url and logout
            console.log('sso.service - logout - logout url null. reload and logout');
            void this.oAuthService.loadDiscoveryDocument(discoveryUrl).then(() => {
                console.log('sso.service - logout - logout url loaded');
                this.oAuthService.logOut();
                this.afterLogout();
            });
        }
    }

    login() {
        console.log('sso.service - login ');
        this.oAuthService.initImplicitFlow();
    }

    loginWithUserNameAndPassword(username: string, password: string): Promise<any> {
        return this.oAuthService.createAndSaveNonce().then(nonce => {
            return this.authClient.signIn({
                username,
                password
            }).then((response) => {
                if (response.status === 'SUCCESS') {
                    return this.authClient.token.getWithoutPrompt({
                        clientId,
                        responseType,
                        scopes,
                        sessionToken: response.sessionToken,
                        nonce,
                        redirectUri
                    }).then((tokenResponse: any) => {
                        const idToken = tokenResponse.tokens.idToken.value;
                        const accessToken = tokenResponse.tokens.accessToken.value;
                        const keyValuePair = `#id_token=${encodeURIComponent(idToken)}&access_token=${encodeURIComponent(accessToken)}`;
                        return this.oAuthService.tryLogin({
                            customHashFragment: keyValuePair,
                            disableOAuth2StateCheck: true,
                            // customRedirectUri: homeUri
                        }).then((success) => {
                            if (success) {
                                this.isLoggedIn = true;
                                return Promise.resolve(true);
                            } else {
                                this.isLoggedIn = false;
                                return Promise.reject(success);
                            }
                        });
                    });
                } else {
                    return Promise.reject(response.status);
                }
            }).catch(err => {
                return Promise.reject(err);
            });
        });
    }

    checkLoginState(): void {
        if (!this.oAuthService.hasValidIdToken()) {
            if (this.ssoInterval) {
                // if we are waiting on response, return;
                return;
            }
            // try to get a token if already logged in somewhere else
            this.oAuthService
                .loadDiscoveryDocument(discoveryUrl)
                .then(() => {
                    console.log('okta.service - checkLoginState - try Login');
                    void this.oAuthService.tryLogin();
                })
                .then(() => {
                    console.log('okta.service - checkLoginState -silent refresh started');

                    if (!this.oAuthService.hasValidAccessToken()) {
                        this.setupSSOInterval();
                        this.oAuthService.silentRefresh()
                            .then(info => {
                                console.log('okta.service - checkLoginState - silent refresh successful');
                                this.isLoggedIn = true;
                            }
                            )
                            .catch(err => {
                                // this will throws a time_out error as we don't have a valid token to refresh
                                console.log('okta.service - checkLoginState - silent refresh token error', err);
                                this.clearSSOInterval();
                            });
                    }
                }).catch(e => {
                    console.log(e);
                    // if not logged in anywhere, it will throw a token error.
                    this.clearSSOInterval();
                });
            return;
        }
        if (this.oAuthService.getIdTokenExpiration() < new Date().getTime()) {
            // return this.logout();
        }
        this.isLoggedIn = true;
    }

    private setupSSOInterval(): void {
        this.ssoInterval = setInterval(() => {
            if (this.isLoggedIn) {
                clearInterval(this.ssoInterval);
            } else {
                this.checkLoginState();
            }
        }, 1000);
    }

    public clearSSOInterval(): void {
        if (this.ssoInterval) {
            clearInterval(this.ssoInterval);
        }
    }

    afterLogout() {
        // Clear remote session
        // window.location.href = `${issuer}/v1/logout?id_token_hint=${idToken}&post_logout_redirect_uri=${homeUri}`;
        // Clear local session
        this.isLoggedIn = false;
        // Clear User data and permissions
        // localStorage.removeItem('permissions');
        // this.cookieService.delete('userData');
        console.log(`sso.service - logout - hasValidIdToken = ${String(this.hasValidIdToken())}`);
        void this.router.navigate(['/login']);
    }

}
