export const environment = {
  production: true,
  PCHost: 'https://www.ingrampartnercentral.com.au/',
  API_BASE: 'https://au-imfs-web-api.ingrammicro.com',
  API_Key: '',
  APP_BASE: 'https://au-imfs.ingrammicro.com',
  DisplayFunderPlan: true,

  InactivityTime: 60000,
  WaitForLogoutTime: 30,
  //okta setting
  OktaRedirectUri:  'https://au-imfs.ingrammicro.com/sso/call-back',
  OktaPostLogoutRedirectUri: 'https://au-imfs.ingrammicro.com',
  OktaHomeUri: 'https://au-imfs.ingrammicro.com/home',
  OktaDomain: 'https://myaccount.ingrammicro.com',
  OktaIssuer : 'https://myaccount.ingrammicro.com/oauth2/aus4rmom0ezJITTQd357',
  OktaClientId : '0oakpvzh145Og22W5357', // SPA
  OktaDiscoveryUrl: 'https://myaccount.ingrammicro.com/oauth2/aus4rmom0ezJITTQd357/.well-known/openid-configuration',
};
