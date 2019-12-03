using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
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
        FisrtNameStepAsync,
        LastNameStepAsync,
        NameConfirmStepAsync,
        EmailStepAsync,
        AmountPeopleStepAsync,
        ConfirmStepAsync,
        SummaryStepAsync,
    };

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new TextPrompt("email", EmailPromptValidatorAsync));
        AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AmountPeoplePromptValidatorAsync));
    }

    }

}
