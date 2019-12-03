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

    }
}
