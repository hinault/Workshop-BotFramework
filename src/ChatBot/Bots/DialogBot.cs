using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {

    }
}


