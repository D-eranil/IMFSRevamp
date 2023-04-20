
export class UserResponseModel {
    id: string;
    title: string;
    firstName: string;
    lastName: string;
    fullName: string;
    userName: string;
    jobTitle: string;
    email: string;
    phoneNumber: string;
    customerNumber: string;
    customerName: string;
    defaultFinanceType: number | null;
    defaultFinanceDuration: number | null;
    defaultFinanceFrequency: string;

    defaultFinanceTypeName: string;
    defaultFinanceDurationName: string;
    active: boolean;
    excludeGST: boolean;

    roleId: string;
    roleName: string;

}

export class UserUpdateModel {
    id: string;
    title: string;
    firstName: string;
    lastName: string;
    userName: string;
    jobTitle: string;
    email: string;
    phoneNumber: string;
    customerNumber: string;
    customerName: string;
    defaultFinanceType: number | null;
    defaultFinanceDuration: number | null;
    defaultFinanceFrequency: string;

    active: boolean;
    excludeGST: boolean;
    role: string;
}

export class UserSearchModel {
    fullName: string;
    firstName: string;
    lastName: string;
    email: string;
    active: boolean;
    customerNumber: string;
    customerName: string;
    jobTitle: string;
}

export class CurrentUserInfo {
    userId: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    customerNumber: string;
    companyName: string;
    phone: string;
    role: string;
}
