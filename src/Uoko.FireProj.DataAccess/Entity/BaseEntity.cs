using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Entity
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

   
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifyId { get; set; }
        public string ModifierName { get; set; }

        public DateTime? ModifyDate { get; set; }
    }
}
