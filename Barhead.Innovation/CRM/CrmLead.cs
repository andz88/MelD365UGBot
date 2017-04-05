using Barhead.Innovation.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barhead.Innovation.CRM
{
    [Serializable]
    public class CrmLead
    {
        #region CRM Attributes
        public static string EntityName = "lead";
        public static string Field_Subject = "subject";
        public static string Field_Description = "description";
        public static string Field_Lastname = "lastname";
        public static string Field_ContactNumber = "telephone1";
        public static string Field_Email = "emailaddress1";
        #endregion

        public static void CreateLead(LeadDetail leadDetail, IOrganizationService crmService)
        {
            var lead = new Microsoft.Xrm.Sdk.Entity(EntityName);
            //lead.Attributes
            lead.Attributes.Add(Field_Subject, leadDetail.Topic);
            lead.Attributes.Add(Field_Lastname, leadDetail.Name);
            lead.Attributes.Add(Field_ContactNumber, leadDetail.ContactNumber);
            lead.Attributes.Add(Field_Email, leadDetail.Email);
            lead.Attributes.Add(Field_Description, leadDetail.Description);

            crmService.Create(lead);
        }
    }
}