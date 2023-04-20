import { environment } from 'src/environments/environment';

const controllerName = 'Attachment';
const baseUrl: string = environment.API_BASE + '/' + controllerName + '/';

export const AttachmentUrls = {
  DownloadEmailAttachment: baseUrl + 'DownloadEmailAttachment',
  DownloadEmailTempAttachment: baseUrl + 'DownloadEmailTempAttachment',
  DownloadImageAttachment: baseUrl + 'DownloadImageAttachment',
  SaveEmailAttachmentTempFile: baseUrl + 'SaveEmailAttachmentTempFile',
  RemoveEmailAttachmentTempFile: baseUrl + 'RemoveEmailAttachmentTempFile',
  GetEmailAttachments: baseUrl + 'GetEmailAttachments',
};
 