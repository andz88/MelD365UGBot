using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barhead.Innovation.Model
{
    [Serializable]
    public class CaseDetail
    {
        public string Description { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public CasePriority Priority { get; set; }
        public string Title { get; set; }
        public string CustomerName { get; set; }
    }

    [Serializable]
    public enum CasePriority
    {
        Low = 3,
        Normal = 2,
        High = 1,
        Critical = 0
    }
}