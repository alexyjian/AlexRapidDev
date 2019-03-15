using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ALEXFW.Entity
{
    public class ValidateCode : EntityBase
    {
        [StringLength(50)]
        public virtual string Name { get; set; }
    }
}