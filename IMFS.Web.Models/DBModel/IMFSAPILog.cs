using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("IMFSAPILogs")]
    public class IMFSAPILog: BaseEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public DateTime? Timestamp { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
