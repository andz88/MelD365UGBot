using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Barhead.Innovation.CRM;
using Barhead.Innovation.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace Barhead.Innovation.FormFlow
{

    [Serializable]
    public class CaseFormFlow
    {
        public string CaseDescription;
        public CasePriority Priority;
        public string Name;
        public string ContactNumber;
        public string Email;

        public static IForm<CaseFormFlow> BuildForm()
        {
            OnCompletionAsyncDelegate<CaseFormFlow> processRequest = async (context, state) =>
            {
                var crmConnection = CrmDataConnection.GetOrgService();

                var caseNumber = "";
                var caseDetail = new CaseDetail
                {
                    Title = $@"Issue logged by {state.Name} at {DateTime.Now.ToShortDateString()}",
                    CustomerName = state.Name,
                    EmailAddress = state.Email,
                    Description = state.CaseDescription,
                    Priority = state.Priority,
                    ContactNumber = state.ContactNumber
                };

                // Todo store the data in CRM.
                var caseId = CrmCase.CreateCase(caseDetail, crmConnection);

                caseNumber = CrmCase.GetCaseNumberById(caseId, crmConnection);

                await context.PostAsync($@"Thank you for contacting us, your service request has been logged. For reference, your case number is: {caseNumber}
                                    {Environment.NewLine}Our service team will get back to you shortly.
                                    {Environment.NewLine}Your case summary:
                                    {Environment.NewLine}Name: {caseDetail.CustomerName},
                                    {Environment.NewLine}Email: {caseDetail.EmailAddress},
                                    {Environment.NewLine}Contact Number: {caseDetail.ContactNumber},
                                    {Environment.NewLine}CaseDescription: {caseDetail.Description},
                                    {Environment.NewLine}Priority: {caseDetail.Priority}");
            };

            return new FormBuilder<CaseFormFlow>()
                   .Message("Welcome to Dynamics 365 UG Form Flow bot!")
                   .Field(nameof(CaseDescription))
                   .Field(nameof(Priority))
                   .Field(nameof(Name))
                   .Field(nameof(ContactNumber))
                   .Field(nameof(Email))
                   .AddRemainingFields()
                   .OnCompletion(processRequest)
                   .Build();
        }

    }
}