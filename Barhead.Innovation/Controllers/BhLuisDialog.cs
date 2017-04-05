using Barhead.Innovation.CRM;
using Barhead.Innovation.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Barhead.Innovation.Controllers
{
    [LuisModel("defab807-d129-4188-bd39-a1b6a71d2048", "63a196fab0ad49b0bf6fe03f50a80120")]
    [Serializable]
    public class BhLuisDialog : LuisDialog<object>
    {
        protected LeadDetail leadDetail;
        protected CaseDetail caseDetail;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand your request";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = $"Welcome to Melbourne Dynamics 365 UG Bot. Is there anything that I could help you today?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("EndConversation")]
        public async Task Ending(IDialogContext context, LuisResult result)
        {
            string message = $"Thanks for using Melbourne Dynamics 365 UG Bot. Hope you have a great day!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        #region Submit Case
        [LuisIntent("SubmitCase")]
        public async Task SubmitCase(IDialogContext context, LuisResult result)
        {
            caseDetail = new CaseDetail();
            PromptDialog.Text(
                context: context,
                resume: CaseDescriptionHandler,
                prompt: "I'm sorry with the problem that you encountered. Could you please provide brief description of the problem?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task CaseDescriptionHandler(IDialogContext context, IAwaitable<string> argument)
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

            PromptDialog.Confirm(
                context: context,
                resume: AnythingElseHandler,
                prompt: "Is there anything else that I could help with?",
                retry: "Sorry, I don't understand that."
            );
        }
        #endregion


        #region Register Interest
        [LuisIntent("RegisterInterest")]
        public async Task RegisterInterest(IDialogContext context, LuisResult result)
        {
            leadDetail = new LeadDetail();
            PromptDialog.Text(
                context: context,
                resume: LeadContactHandler,
                prompt: "Great, thanks for your interest. May I know your name please?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task LeadContactHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var name = await argument;
            leadDetail.Name = name;
            PromptDialog.Text(
                context: context,
                resume: LeadContactNumberHandler,
                prompt: "What is the best number to contact?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task LeadContactNumberHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var contactNumber = await argument;
            leadDetail.ContactNumber = contactNumber;
            PromptDialog.Text(
                context: context,
                resume: LeadContactEmailHandler,
                prompt: "Could you please provide your email address?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task LeadContactEmailHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var email = await argument;
            leadDetail.Email = email;
            PromptDialog.Text(
                context: context,
                resume: LeadCompleteHandler,
                prompt: "Could you please briefly describe your interest?",
                retry: "Sorry, I don't understand that."
            );
        }

        public virtual async Task LeadCompleteHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var desc = await argument;
            leadDetail.Description = desc;

            await context.PostAsync($@"Thank you for your interest, your request has been logged. Our sales team will get back to you shortly.
                                    {Environment.NewLine}Your interest summary:
                                    {Environment.NewLine}Name: {leadDetail.Name},
                                    {Environment.NewLine}Email: {leadDetail.Email},
                                    {Environment.NewLine}Contact Number: {leadDetail.ContactNumber},
                                    {Environment.NewLine}Other comments: {leadDetail.Description}");

            leadDetail.Topic = $@"Interest inquiry from {leadDetail.Name} on {DateTime.Now.ToString(new CultureInfo("en-AU"))}";

            // Todo store the data in CRM.
            CrmLead.CreateLead(leadDetail, CrmDataConnection.GetOrgService());

            PromptDialog.Confirm(
                context: context,
                resume: AnythingElseHandler,
                prompt: "Is there anything else that I could help?",
                retry: "Sorry, I don't understand that."
            );
        }

        #endregion

        public async Task AnythingElseHandler(IDialogContext context, IAwaitable<bool> argument)
        {
            var answer = await argument;
            if (answer)
            {
                await GeneralGreeting(context, null);
            }
            else
            {
                string message = $"Thanks for using Barhead Innovation Bot. Hope you have a great day!";
                await context.PostAsync(message);
                context.Done<string>("conversation ended.");
            }
        }


        public virtual async Task GeneralGreeting(IDialogContext context, IAwaitable<string> argument)
        {
            string message = $"Great! What else that can I help you with?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}