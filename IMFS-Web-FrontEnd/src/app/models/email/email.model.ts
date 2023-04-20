import { SelectItem } from 'primeng/api';

export class Email {
    Id: number;
    FromAddress: string;
    ToAddress: string;
    CCEmail: string;
    Subject: string;
    Body: string;
    BodyType: string;
    Status: string;
    DateTimeReceived: Date;
    EmailUniqueID: string;
    BCCEmail: string;
    Notes: string;
    TempEmailId: string;
    QuoteId: string;
    ApplicationId: string;
    // RelatedEmailIds: number[];
    // RelatedJobIds: number[];
    // RelatedJobMenus: JobMenu[];
    // QueueDetails: QueueDetails;
    HtmlBody: string;
    quotesAttached: number[];
    HasOtherEmailQueues: boolean;
    EmailAttachments: SelectItem[];
    EmailType: string;
    ShowRemoveButton: boolean;
    // FavoriteSCList: NewJobCategory[];
    ToAddressExpanded: boolean;
    CCEmailExpanded: boolean;
    BCCEmailExpanded: boolean;

    constructor() {
        // this.QueueDetails = new QueueDetails();
        // this.RelatedEmailIds = [];
        // this.RelatedJobIds = [];
        this.EmailAttachments = [];
        // this.FavoriteSCList = [];
        // this.RelatedJobMenus = [];
    }
}

export class EmailViewModel {
    id: string;
    email: string;
    isNew: boolean;
    entity: Email;
    outOfOffice: boolean;
    init(entity: Email) {
        this.entity = entity;
    }
}

export class WriteEmailViewModel {
    emailId: number;
    fromAddress: string;
    toAddress: string;
    emailMode: string;
    ccEmail: string;
    bccEmail: string;
    subject: string;
    body: string;
    tempEmailId: string;
    attachments: EmailAttachment[];
}

export class EmailAttachment {
    id: number;
    contentId: string;
    fileName: string;
    fileSize: number;
    tempEmailId: string;
    quoteIdAttached: number;
}

export class EmailHistory {
    emailId: number;
    quoteId: number;
    applicationId: number;
    contractId: number;
    date: string;
    time: string;
    to: string;
    subject: string;
}

export class GetWriteEmailModel
{
    EmailMode: string;
    EmailId: number;
    removeAcceptanceLink: boolean;
    QuoteId: number;
    ApplicationId: number;
}
