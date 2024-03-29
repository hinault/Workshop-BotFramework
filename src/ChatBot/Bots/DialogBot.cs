﻿using Microsoft.Bot.Builder;
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

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);


        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var card = new HeroCard();
                    card.Title = "Welcome to Bot Workshop!";
                    card.Text = @"You will learn how to build bot with conversation flow using Dialog Libray. Type anything for launch registration form.";
                    card.Images = new List<CardImage>() { new CardImage("https://raw.githubusercontent.com/hinault/Workshop-BotFramework/master/media/ChatBot-BotFramework.png") };
                    card.Buttons = new List<CardAction>()
                    {
                      new CardAction(ActionTypes.OpenUrl, "About Global AI Bootcamp", null, "About Global AI Bootcamp", "About Global AI Bootcamp", "https://globalai.community/global-ai-bootcamp/"),
                     new CardAction(ActionTypes.OpenUrl, "Bot Framework Documentation", null, "Bot Framework Documentation", "Bot Framework Documentation", "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0"),
                      new CardAction(ActionTypes.OpenUrl, "Bot Framework samples", null, "Bot Framework samples", "Bot Framework samples", "https://github.com/microsoft/BotBuilder-Samples"),
                     };

                    var response = MessageFactory.Attachment(card.ToAttachment());
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
            }
        }

     
    }
}


