// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    PCHost: 'http://localhost:4200/',  //http:localhost:4200
    API_BASE: 'http://imfs2.kenticodevelopers.in',
    API_Key: '',
    APP_BASE: 'hhttp://localhost:4200/',
    DisplayFunderPlan: true,
    InactivityTime: 600000000,
    InactivityTimeOnQuoteAccept: 12000,
    WaitForLogoutTime: 30,
    // okta setting
    OktaRedirectUri:  'http://localhost:4200/sso/call-back',
    OktaPostLogoutRedirectUri: 'http://localhost:4200',
    OktaHomeUri: 'http://localhost:4200/home',
    // OktaDomain: 'https://myaccount-stage.ingrammicro.com',
    // OktaIssuer : 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7',
    // OktaClientId : '0oau34sn1wLaW3Ebq0h7', // SPA
    // OktaDiscoveryUrl: 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7/.well-known/openid-configuration',
    OktaDomain: 'https://myaccount-dev.ingrammicro.com',
    OktaIssuer : 'https://myaccount-dev.ingrammicro.com/oauth2/ausmd44ne8KlWbDms0h7',
    OktaClientId : '0oa105asut5zlul2E0h8', // SPA
    OktaDiscoveryUrl: 'https://myaccount-dev.ingrammicro.com/oauth2/ausmd44ne8KlWbDms0h7/.well-known/openid-configuration',
  };


  /*
   * For easier debugging in development mode, you can import the following file
   * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
   *
   * This import should be commented out in production mode because it will have a negative impact
   * on performance if an error is thrown.
   */
  // import 'zone.js/dist/zone-error';  // Included with Angular CLI.
