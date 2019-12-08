using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);




            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var card = new HeroCard();
                    card.Title = "Welcome to Bot Workshop!";
                    card.Text = @"You will learn how to build boot with conversation flow using Dialog Libray. Type anything for launch registration form.";
                    card.Images = new List<CardImage>() { new CardImage("https://i0.wp.com/blog.3ie.fr/wp-content/uploads/2018/11/ChatBot-BotFramework.png") };
                    card.Buttons = new List<CardAction>()
                    {
                      new CardAction(ActionTypes.OpenUrl, "Register", null, "Register", "Register", "Register"),
                      new CardAction(ActionTypes.OpenUrl, "Ask a question about QnA Maker", null, "QnA", "Ask a question about QnA Maker", "QnA"),
                       new CardAction(ActionTypes.OpenUrl, "Bot Framework Documentation", null, "Bot Framework Documentation", "Bot Framework Documentation", "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0"),
                     };

                    var response = MessageFactory.Attachment(card.ToAttachment());
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

           
          await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                   
            // Run the Dialog with the new message Activity.
                    }
    }
}


