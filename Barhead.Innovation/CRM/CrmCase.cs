using Barhead.Innovation.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barhead.Innovation.CRM
{
    public class CrmCase
    {
        #region CRM Attributes
        public static string EntityName = "incident";
        public static string Field_Title = "title";
        public static string Field_Description = "description";
        public static string Field_CaseNumber = "ticketnumber";
        public static string Field_Customer = "customerid";
        public static string Field_PriorityCode = "prioritycode";
        public static string Field_Origin = "caseorigincode";
        #endregion

        public static Guid CreateCase(CaseDetail caseDetail, IOrganizationService crmService)
        {
            var crmCase = new Microsoft.Xrm.Sdk.Entity(EntityName);

            var customer = CrmContact.GetContactByEmailOrPhone(caseDetail.ContactNumber, caseDetail.EmailAddress, crmService);
            if (customer == null)
            {
                CrmContact.CreateContact(caseDetail.CustomerName, caseDetail.ContactNumber, caseDetail.EmailAddress, crmService);

                customer = CrmContact.GetContactByEmailOrPhone(caseDetail.ContactNumber, caseDetail.EmailAddress, crmService);
            }

            crmCase.Attributes.Add(Field_Title, caseDetail.Title);
            crmCase.Attributes.Add(Field_Description, caseDetail.Description);
            crmCase.Attributes.Add(Field_Customer, customer.ToEntityReference());
            crmCase.Attributes.Add(Field_PriorityCode, new OptionSetValue((int)caseDetail.Priority));
            crmCase.Attributes.Add(Field_Origin, new OptionSetValue(881840000));

            return crmService.Create(crmCase);
        }

        public static string GetCaseNumberById(Guid caseId, IOrganizationService crmService)
        {
            var result = crmService.Retrieve(EntityName, caseId, new ColumnSet(new string[] { Field_CaseNumber }));

            if (result != null)
            {
                return result.Attributes[Field_CaseNumber].ToString();
            }

            return string.Empty;
        }
    }
}