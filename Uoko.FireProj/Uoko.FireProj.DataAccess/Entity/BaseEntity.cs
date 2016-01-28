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

   
        public int CreateBy { get; set; }


        public DateTime CreateDate { get; set; }

        public int? ModifyBy { get; set; }

        public DateTime? ModifyDate { get; set; }
    }
}
