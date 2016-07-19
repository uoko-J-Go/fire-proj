using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class TestResultDto:BaseDto
    {
        public int TaskId { get; set; }

        public StageEnum Stage { get; set; }

        public QAStatus QAStatus { get; set; }


        public string Comments { get; set; }
    }
}
