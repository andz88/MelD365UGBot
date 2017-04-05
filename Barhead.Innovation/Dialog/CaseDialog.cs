using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Barhead.Innovation.CRM;
using Barhead.Innovation.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Barhead.Innovation.Dialog
{
    [Serializable]
    public class CaseDialog : IDialog<object>
    {
        protected CaseDetail caseDetail;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.Contains("problem"))
            {
                caseDetail = new CaseDetail();

                PromptDialog.Text(
                    context: context,
                    resume: DescriptionHandler,
                    prompt: "Could you please describe your problem?",
                    retry: "Sorry, I don't understand that."
                );
            }
            else if (message.Text.ToLower() == "no")
            {
                context.Done("Thanks!");
            }
            else
            {
                await context.PostAsync("Hi there, anything that I can help for you today?");
                context.Wait(MessageReceivedAsync);
            }
        }

        public virtual async Task DescriptionHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var desc = await argument;
            caseDetail.Description = desc;
            PromptDialog.Text(
                context: context,
                resume: CaseCustomerNameHandler,
                prompt: "May I know your name please?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task CaseCustomerNameHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var name = await argument;
            caseDetail.CustomerName = name;
            PromptDialog.Text(
                context: context,
                resume: CaseContactNumberHandler,
                prompt: "What is the best number to call you back regarding this issue?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task CaseContactNumberHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var contactNumber = await argument;
            caseDetail.ContactNumber = contactNumber;
            PromptDialog.Text(
                context: context,
                resume: CaseEmailHandler,
                prompt: "Could you please provide us with your email address?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task CaseEmailHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var desc = await argument;
            caseDetail.EmailAddress = desc;
            PromptDialog.Choice(
                context: context,
                options: Enum.GetValues(typeof(CasePriority)).Cast<CasePriority>().ToArray(),
                resume: CaseCompleteHandler,
                prompt: "Thank you. What is the priority of the issue?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task CaseCompleteHandler(IDialogContext context, IAwaitable<CasePriority> argument)
        {
            var priority = await argument;
            caseDetail.Priority = priority;
            caseDetail.Title = $@"Issue logged by {caseDetail.CustomerName} at {DateTime.Now.ToShortDateString()}";

            var crmConnection = CrmDataConnection.GetOrgService();

            var caseNumber = "";

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

            caseDetail.Title = $@"Service Request inquiry from {caseDetail.EmailAddress} on {DateTime.Now.ToString(new CultureInfo("en-AU"))}";
        }
    }
}