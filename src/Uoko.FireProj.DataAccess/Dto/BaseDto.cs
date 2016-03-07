using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class BaseDto
    {
        public int? CreatorId { get; set; }
        public string CreatorName { get; set; }

        public int? ModifyId { get; set; }
        public string ModifierName { get; set; }
    }
}
