export class NotificationItem {
    public Title: string;
    public NotificationType: string;
    public SourceId: string;
    public SourceType: string;
    public Messages: NotificationMessage[];
}

export class NotificationMessage {
    public Text: string;
    public Link: string;
    public UpatedDate: Date;
}

export enum NotificationSourceTypeEnum {
    Workflow,
    Annuity,
    MSP,
    WFReturnToRequestor,
}
