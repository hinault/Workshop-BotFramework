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

#### 3. Create an App registration

Registering the application means that you can use Azure AD to authenticate users and request access to user resources. Your bot requires a Registered app in Azure that provides the bot access to the Bot Framework Service for sending and receiving authenticated messages. To create register an app via the Azure CLI, perform the following command:

```cmd
az ad app create --display-name "displayName" --password "AtLeastSixteenCharacters_0" --available-to-other-tenants
```

| Option   | Description |
|:---------|:------------|
| display-name | The display name of the application. |
| password | App password, aka 'client secret'. The password must be at least 16 characters long, contain at least 1 upper or lower case alphabetical character, and contain at least 1 special character.|
| available-to-other-tenants| Indicates whether the application can be used from any Azure AD tenant. Set to `true` to enable your bot to work with the Azure Bot Service channels.|

The above command outputs JSON with the key `appId`, save the value of this key for the ARM deployment, where it will be used for the `appId` parameter. The password provided will be used for the `appSecret` parameter.

#### 4. Deploy via ARM template

You'll create a new resource group in Azure and then use the ARM template to create the resources specified in it. In this case, we are providing App Service Plan, Web App, and Bot Channels Registration.

```cmd
az deployment create --name "<name-of-deployment>" --template-file "template-with-new-rg.json" --location "location-name" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" botSku=F0 newAppServicePlanName="<name-of-app-service-plan>" newWebAppName="<name-of-web-app>" groupName="<new-group-name>" groupLocation="<location>" newAppServicePlanLocation="<location>"
```

| Option   | Description |
|:---------|:------------|
| name | Friendly name for the deployment. |
| template-file | The path to the ARM template. You can use the `template-with-new-rg.json` file provided in the `deploymentTemplates` folder of the project. |
| location |Location. Values from: `az account list-locations`. You can configure the default location using `az configure --defaults location=<location>`. |
| parameters | Provide deployment parameter values. `appId` value you got from running the `az ad app create` command. `appSecret` is the password you provided in the previous step. The `botId` parameter should be globally unique and is used as the immutable bot ID. It is also used to configure the display name of the bot, which is mutable. `botSku` is the pricing tier and can be F0 (Free) or S1 (Standard). `newAppServicePlanName` is the name of App Service Plan. `newWebAppName` is the name of the Web App you are creating. `groupName` is the name of the Azure resource group you are creating. `groupLocation` is the location of the Azure resource group. `newAppServicePlanLocation` is the location of the App Service Plan. |

#### 5. Prepare your code for deployment

You need to prepare your project files before you can deploy your bot. 

```cmd
az bot prepare-deploy --lang Csharp --code-dir "." --proj-file-path "MyBot.csproj"
```

You must provide the path to the .csproj file relative to --code-dir. This can be performed via the --proj-file-path argument. The command would resolve --code-dir and --proj-file-path to "./MyBot.csproj".

> [!NOTE]
>  For C# bots, the `az bot prepare-depoloy` command should generate a `.deployment` file in your bot project folder

When using the non-configured [zip deploy API](https://github.com/projectkudu/kudu/wiki/Deploying-from-a-zip-file-or-url) to deploy your bot's code, Web App/Kudu's behavior is as follows:

_Kudu assumes by default that deployments from zip files are ready to run and do not require additional build steps during deployment, such as npm install or dotnet restore/dotnet publish._

As such, it is important to include your built code and with all necessary dependencies in the zip file being deployed to the Web App, otherwise your bot will not work as intended.

> [!IMPORTANT]
> Before zipping your project files, make sure that you are _in_ the project folder. 
> - For C# bots, it is the folder that has the .csproj file. 
>
>**Within** the project folder, select all the files and folders you want included in your zip file before running the command to create the zip file, this will create a single zip file containing all selected files and folders.
>
> If your root folder location is incorrect, the **bot will fail to run in the Azure portal**.

### Deploy code to Azure

At this point we are ready to deploy the code to the Azure Web App. Run the following command from the command line to perform deployment using the kudu zip push deployment for a web app.

```cmd
az webapp deployment source config-zip --resource-group "<resource-group-name>" --name "<name-of-web-app>" --src "code.zip" 
```

| Option   | Description |
|:---------|:------------|
| resource-group | The name of the Azure resource group that contains your bot. (This will be the resource group you used or created when creating the app registration for your bot.) |
| name | Name of the Web App you used earlier. |
| src  | The path to the zipped file you created. |

### Test in Web Chat

1. In your browser, navigate to the [Azure portal](https://ms.portal.azure.com).
2. In the left panel, click **Resource groups**.
3. In the right panel, search for your group.
4. Click on your group name.
5. Click the link of your Bot Channel Registration.
6. In the **Bot Channel Registration** panel, click **Test in Web Chat**.
Alternatively, in the right panel, click the **Test** box.

For more information about channel registration, see [Register a bot with Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-quickstart-registration?view=azure-bot-service-3.0).