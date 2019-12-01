# Workshop-BotFramework

In this workshop, you will learn how :
 * Build Chat Bot using Bot Framework SDK v4 and Visual Studio 2019
 * Debug and test your bot with Bot Emulator
 * Deploy your bot on Azure Bot Service
 * Use Dialog library to manage a conversation with the user


## Pre-requisites

 * [Visual Studio 2019](https://www.visualstudio.com/downloads)
 * [Bot Framework Emulator](https://aka.ms/bot-framework-emulator-readme)
 * [Knowledge of ASP.Net Core](https://docs.microsoft.com/aspnet/core/) 
 * [Subscription to Microsoft Azure](https://azure.microsoft.com/free/)
 * [Latest version of the Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)

## Install Bot Framework SDK v4 template for C#

 1. Open Visual Studio. Select Extenions on the menu bar and click on Manage Extenions.

 1. Search Bot and download Bot Framework v4 SDK Templates for Visual Studio.

 ![Install Bot Framework templates](media/install-bot-templates.png)
 
 3. Close Visual Studio and install extension.

## Create and test a bot locally
 
### Create a bot

In Visual Studio, create a new bot project using the **Echo Bot (Bot Framework v4)** template. Enter _bot framework v4_ in the search box to show only bot templates.

![Visual Studio create a new project dialog](media/bot-builder-dotnet-project-vs2019.png)

> [!TIP] 
> If using Visual Studio 2017, make sure that the project build type is ``.Net Core 2.1`` or later. Also if needed, update the `Microsoft.Bot.Builder` [NuGet packages](https://docs.microsoft.com/nuget/quickstart/install-and-use-a-package-in-visual-studio).

Thanks to the template, your project contains all the code that's necessary to create the bot in this quickstart. You won't actually need to write any additional code.

### Start your bot in Visual Studio

When you click the run button, Visual Studio will build the application, deploy it to localhost, and launch the web browser to display the application's `default.htm` page. At this point, your bot is running locally.

### Start the emulator and connect your bot

Next, start the emulator and then connect to your bot in the emulator:

1. Click the **Create a new bot configuration** link in the emulator "Welcome" tab. 
1. Fill out the fields for your bot. Use your bot's welcome page address (typically http://localhost:3978) and append routing info '/api/messages' to this address.
1. then click **Save and connect**.

### Interact with your bot

Send a message to your bot, and the bot will respond back with a message.

![Emulator running](media/emulator-running.png)

> [!NOTE]
> If you see that the message cannot be sent, you might need to restart your machine as ngrok didn't get the needed privileges on your system yet (only needs to be done one time).

## Deploy your bot

### Prepare for deployment

When you create a bot using the [Visual Studio template](https://docs.microsoft.com/azure/bot-service/dotnet/bot-builder-dotnet-sdk-quickstart?view=azure-bot-service-4.0) or [Yeoman template](https://docs.microsoft.com/azure/bot-service/javascript/bot-builder-javascript-quickstart?view=azure-bot-service-4.0), the source code generated includes a `deploymentTemplates` folder that contains ARM templates. The deployment process documented here uses one of the ARM templates to provision required resources for the bot in Azure by using the Azure CLI. 

#### 1. Login to Azure

Once you've created and tested a bot locally, you can deploy it to Azure. Open a command prompt to log in to the Azure portal.

```cmd
az login
```
A browser window will open, allowing you to sign in.

#### 2. Set the subscription

Set the default subscription to use.

```cmd
az account set --subscription "<azure-subscription>"
```

If you are not sure which subscription to use for deploying the bot, you can view the list of subscriptions for your account by using `az account list` command. Set the default subscription to use.

```cmd
az account set --subscription "<azure-subscription>"
```

If you are not sure which subscription to use for deploying the bot, you can view the list of subscriptions for your account by using `az account list` command. 

