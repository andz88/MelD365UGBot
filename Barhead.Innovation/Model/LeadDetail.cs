using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barhead.Innovation.Model
{
    [Serializable]
    public class LeadDetail
    {
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Topic { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string ContactNumber { get; set; }
    }
}