using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class AttachmentRequest
    {
        public int Source { get; set; }
        /// <summary>
        /// Pass Quote/application
        /// </summary>
        public int Id { get; set; }
        public int? ApplicationNumber { get; set; }
        public string Description { get; set; }
        public List<IFormFile> files { get; set; }
    }

    public class DeleteAttachmentRequest
    {
        public int FileId { get; set; }
    }

    public class AttachmentsResponse
    {
        public int FileId { get; set; }
        public int? QuoteId { get; set; }
        public int? ApplicationId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UploadBy { get; set; }

    }

    public class FileUploadResponse
    {
        public string Message { get; set; }
        public List<FileUploadResponseData> Data { get; set; }
    }

    public class FileDownloadResponse
    {
        public bool IsResult { get; set; }
        public string Message { get; set; }
        public string PhysicalPath { get; set; }
        public string FileName { get; set; }
    }

    public class FileUploadResponseData
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
    }

    public enum Source
    {
        Quote = 1,
        Application = 2,
    }

    public class ProductDetailResponse
    {
        public List<ProductDetailsResult> Products { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }

    public class ProductDetailsResult
    {
        public string InternalSKUID { get; set; }
        public string VendorSKUID { get; set; }
        public string ProductDescription { get; set; }
        public int? Vsrid { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? UnitNetAmount { get; set; }

    }


    public class EmailAttachmentsResponse
    {
        public int FileId { get; set; }
        public int? QuoteId { get; set; }
        public int? ApplicationId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public string PhysicalPath { get; set; }

    }
}
