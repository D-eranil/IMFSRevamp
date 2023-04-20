
export class TokenResponse {
    // tslint:disable-next-line: variable-name
    access_token: string;
    userEmail: string;
    // tslint:disable-next-line: variable-name
    refresh_token: string;
}

export class ForgotPassword {
    constructor(public userEmail: string) {
    }
}

export class SignIn {
    constructor(
        public userEmail: string,
        public password: string,
        public rememberMe: boolean
    ) { }
}
