using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    public class RegisterDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<RegisterData> _registerDataAccessor;


public RegisterDialog (UserState userState) : base(nameof(RegisterDialog))
{
   _registerDataAccessor = userState.CreateProperty<RegisterData>("RegisterData");

    // This array defines how the Waterfall will execute.
    var waterfallSteps = new WaterfallStep[]
    {
        FirstNameStepAsync,
        LastNameStepAsync,
        EmailStepAsync,
        ProfileStepAsync,
        AmountPeopleStepAsync,
        SummaryStepAsync,
    };

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new TextPrompt("email", EmailPromptValidatorAsync));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AmountPeoplePromptValidatorAsync));
        AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FirstNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your fisrt name.") }, cancellationToken);

        }

        private async Task<DialogTurnResult> LastNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["firstname"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your last name.") }, cancellationToken);

        }



        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["lastname"] = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {(string)stepContext.Values["firstname"]} {stepContext.Result}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync("email", new PromptOptions { Prompt = MessageFactory.Text("Please enter your email.") }, cancellationToken);
        }


        private async Task<DialogTurnResult> ProfileStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["email"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select your job title."),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Developer", "Administrator", "Analyst", "Architect" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> AmountPeopleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["profile"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = MessageFactory.Text("Please enter number of attendees (max 3).") }, cancellationToken);

        }

       

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["amountpeople"] = (int)stepContext.Result;

            // Get the current registerData object from user state.
            var registerData = await _registerDataAccessor.GetAsync(stepContext.Context, () => new RegisterData(), cancellationToken);

            registerData.FirstName = (string)stepContext.Values["firstname"];
            registerData.LastName = (string)stepContext.Values["lastname"];
            registerData.Email = (string)stepContext.Values["email"];
            registerData.Profile = (string)stepContext.Values["profile"];
            registerData.AmountPeople = (int)stepContext.Values["amountpeople"];

            var msg = $"Thanks for your registration. We are reserved {registerData.AmountPeople} seat for you.";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private Task<bool> EmailPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Recognized.Value;

            // This condition is our validation rule.
            if (new EmailAddressAttribute().IsValid(result))
            {
                // Success is indicated by passing back the value the Prompt has collected. You must pass back a value even if you haven't changed it.
                return Task.FromResult(true);
            }

            // Not calling End indicates validation failure. This will trigger a RetryPrompt if one has been defined.
            return Task.FromResult(false);
        }


        private Task<bool> AmountPeoplePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value <=3);

        }
    }

}
