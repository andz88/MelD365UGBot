using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barhead.Innovation.CRM
{
    public class CrmContact
    {
        #region CRM Attributes
        public static string EntityName = "contact";
        public static string Field_Email = "emailaddress1";
        public static string Field_Telephone1 = "telephone1";
        public static string Field_Mobile = "mobilephone";
        public static string Field_Lastname = "lastname";
        #endregion

        public static Entity GetContactByEmailOrPhone(string phoneNumber, string email, IOrganizationService crmService)
        {
            var query = new QueryExpression(EntityName);
            query.ColumnSet = new ColumnSet();
            query.ColumnSet.Columns.Add("fullname");

            var condition1 = new ConditionExpression(Field_Email, ConditionOperator.Equal, email);
            var condition2 = new ConditionExpression(Field_Telephone1, ConditionOperator.Equal, phoneNumber);
            var condition3 = new ConditionExpression(Field_Mobile, ConditionOperator.Equal, phoneNumber);

            var filter = query.Criteria.AddFilter(LogicalOperator.Or);

            filter.AddCondition(condition1);
            filter.AddCondition(condition2);
            filter.AddCondition(condition3);

            var result = crmService.RetrieveMultiple(query);

            if (result != null && result.Entities != null)
            {
                return result.Entities.FirstOrDefault();
            }

            return null;
        }

        public static void CreateContact(string name, string phone, string email, IOrganizationService crmService)
        {
            var contact = new Entity(EntityName);
            contact.Attributes.Add(Field_Lastname, name);
            contact.Attributes.Add(Field_Email, email);
            contact.Attributes.Add(Field_Telephone1, phone);

            crmService.Create(contact);
        }
    }
}