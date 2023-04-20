using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("AspNetUserRoles")]
    public partial class AspNetUserRoles: BaseEntity
    {
        [Key,  Column(Order = 1)] 
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public string RoleId { get; set; }
    }
}
