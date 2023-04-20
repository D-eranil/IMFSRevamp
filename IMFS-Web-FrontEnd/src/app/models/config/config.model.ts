export class ConfigValuesModel {
    public IMFSVersion: string;
    public CurrentQuarter: string;
    public CurrentWeek: string;
    public CurrentMonth: string;
    public CurrentMonthFrom: string;
    public CurrentMonthTo: string;
    public ExchangeRates: ExchangeRateModel[];
    public SystemMaintenanceMessage: string;
    public IsStaging: boolean;
}

export class ExchangeRateModel {
    Currency: string;
    Rate: number;
    LastUpdated: string;
}

export class SystemMessagesModel {
    SystemMaintenanceMessage: string;
    MessageType: string;
    IsOutOfOffice: boolean;

    HasSpecialOccasion: boolean;
    SpecialOccasionMessage: string;
    SpecialOccasionLink: string;
}

export enum IMFSCountry {
    AU,
    NZ
}
