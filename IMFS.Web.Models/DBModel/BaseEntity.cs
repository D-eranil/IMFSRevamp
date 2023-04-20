using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    public abstract partial class BaseEntity
    {
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            return Equals(obj as BaseEntity);
        }
        //[NotMapped]
        //public DateTime? CreatedDate { get; set; }

     

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
