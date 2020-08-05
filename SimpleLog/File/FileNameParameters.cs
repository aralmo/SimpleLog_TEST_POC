using System;
using System.Collections.Generic;
using System.Text;

namespace CloudIO.Logging.File
{
    public class FileNameParameters
    {
        public DateTime Date { get; set; }
        public int Sequence { get; set; }
        public string CorrelationId { get; set; }
    }
}
