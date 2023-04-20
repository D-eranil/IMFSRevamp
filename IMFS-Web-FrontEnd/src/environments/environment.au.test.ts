export const environment = {
  production: false,
  PCHost: 'http://staging.ingrampartnercentral.com.au/',
  API_BASE: 'https://test-api-au-imfs.ingrammicro.com',
  API_Key: '',
  APP_BASE: 'https://test-au-imfs.ingrammicro.com',
  DisplayFunderPlan: true,

  InactivityTime: 60000,
  WaitForLogoutTime: 30,
  //okta setting
  OktaRedirectUri:  'https://test-au-imfs.ingrammicro.com/sso/call-back',
  OktaPostLogoutRedirectUri: 'https://test-au-imfs.ingrammicro.com',
  OktaHomeUri: 'https://test-au-imfs.ingrammicro.com/home',
  OktaDomain: 'https://myaccount-stage.ingrammicro.com',
  OktaIssuer : 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7',
  OktaClientId : '0oa11cem7rhwtQRlZ0h8', // SPA
  OktaDiscoveryUrl: 'https://myaccount-stage.ingrammicro.com/oauth2/ausnbno6cbweF5SES0h7/.well-known/openid-configuration',
};
