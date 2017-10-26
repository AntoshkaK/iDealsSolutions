using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iDealsSolutions.DbData
{
    public class Logs
    {
        [Key]
        public string LogId { get; set; }
        public string EventType { get; set; }
        public DateTime EventCreationDate { get; set; }
        public string EventBody { get; set; }
        public string EventAction { get; set; }

    }
}