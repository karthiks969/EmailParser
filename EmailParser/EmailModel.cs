using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailParser
{
    public class EmailModel
    {
        public string FromEmail { get; set; }
        public List<OpenPop.Mime.Header.RfcMailAddress> ToEmail { get; set; }

        public DateTime DateTimeSent { get; set; }

        public string Subject { get; set; }

        public string Date { get; set; }

        public string Body { get; set; }

        public string BodyHTMLText { get; set; }
    }
}
