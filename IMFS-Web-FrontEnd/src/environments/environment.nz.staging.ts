export const environment = {
  production: false,
  PCHost: 'http://staging.ingrampartnercentral.com.au/',
  API_BASE: 'https://nz-staging-imfs-web-api.ingrammicro.com',
  API_Key: '',
  APP_BASE: 'https://nz-staging-imfs.ingrammicro.com',
  DisplayFunderPlan: true,

  InactivityTime: 60000,
  WaitForLogoutTime: 30,
  //okta setting
  OktaRedirectUri:  'https://nz-staging-IMFS.Ingrammicro.com/sso/call-back',
  OktaPostLogoutRedirectUri: 'https://nz-staging-imfs.ingrammicro.com',
  OktaHomeUri: 'https://nz-staging-imfs.ingrammicro.com/home',
  OktaDomain: 'https://myaccount-stage.ingrammicro.com',
  OktaIssuer : 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7',
  OktaClientId : '0oa11cem7rhwtQRlZ0h8', // SPA
  OktaDiscoveryUrl: 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7/.well-known/openid-configuration',
};
