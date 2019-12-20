# Configure Power BI Embedded and call from JavaScript
This GitHub repository is designed to help you setup and configure Power BI Embedded in your Azure subscription.
The process of setting up Power BI Embedded is quite confusing since it requires configuring several resource correctly 
(e.g. Azure Service Principal, Azure Security, Power BI, creating Embedded capacity, etc.) and you must tie them together correctly.
The below steps will walk you through the entir process. I recorded a video to show you how to perform the process.  
Power BI Embedded is a great tool, but most report developers have issues when setting up and configuring Power BI Embedded.
This will walk you through each and every step.

The goal of this is:
1. Configure the Azure resources
2. Configure the Power BI resources
3. Deploy a report and test it using a simple HTML page with some JavaScript
4. You should then be able to embeded any report in any application


## Detail Steps
This process will walk through the following
- Create an Azure tenant.  This step can be skipped unless you are doing a test configuration like this sample.  I show it since some users might want to test in an Azure subscription where they are not an Admin.
- Create an Azure Service Principal.  A Service Principal is analogous to a user. Service Principal should be used for automated processes in Azure and in this case we are using a service principal as the "unattended user" who will run the embedded reports.
- Create a Power BI Account.  You can use a free trial account for Power BI if you do not own a license.  If you need a licence you need to contact your Office 365 admin (someone in your organization) and they can assign you a license.
   - Purchase a license: https://docs.microsoft.com/en-us/power-bi/service-admin-purchasing-power-bi-pro
- Create a Power BI Workspace.  This is where we will deploy your reports. 
- Configure Power BI to use the service principal created above
- Deploy a sample report
- Walk through the C# code: PowerBI-Developer-Samples and explain what a report token, url and id are used for when embedding.
- Deploy an Azure Function which will run the C# code from the PowerBI-Developer-Samples and return back a report token, url and id.  The Azure Function means we can then call this from any system, web page or JavaScript code and have our reports easily embedded into any application.
- Create a simple HTML page with minimal JavaScript to show you how to embed a report.  This allows you to  the minimul code required to embed a report on a web page or single page application.

#### Notes
- I will not use the Power BI online wizard (https://app.powerbi.com/embedsetup/appownsdata) to create the Power BI setup.  
- Sometimes the wizard does not work due to how companies have configured their Azure Subscription. If you use the wizard, you must use it as a Global Azure Admin and that Admin must have a Power BI Pro account and they must have logged into https://app.powerbi.com/ before using the wizard.  You also must go generate a key by hand.

## What you will need
- An Azure Subscription (you can get a free one for testing)
- A Power BI Pro license (you can create free account for testing)
- You will need to be a global Azure Admin or have an Admin that can help you through the process.  The Admin should also be a Power BI Admin.  Go find this person in your company and make friends with them.  You should watch and understand this and then work with them to setup the Azure and Power BI resources.  You will need about 30 minutes of their time.
- You will need to install on your machine (make sure you can install items on your machine)
  - Visual Studio Community (or higher edition): https://visualstudio.microsoft.com/vs/community/
  - Azure Function local development tools (require Node.js to be installed).  Details are below.
- A resource group in Azure that you can deploy to.  In an enterprise typically an Admin will create the resource group and grant your Microsoft account Contributor access to the resource group.  This is where we will create the Power BI capacity and Azure Function App.


## YouTube Video
- https://youtu.be/FZjQmvwBAdU


## Optional: Create a new Azure tenant (Requires Azure AD Admin)
You can skip these steps if you are doing this in your company's Azure subscription.  Since this is a test I will show you in a test tenant.  I am not a Global Azure AD admin at Microsoft so I use a test tenant to perform Power BI Embedded work.
1. Open the Azure Portal https://portal.azure.com
2. Click the menu in the top left and select Create a Resource
3. Search for Azure Active Directory
4. Click Create
5. Enter:
   - Organization name: Paternostro Embedded Test
   - Initial Domain Name: paternostroembeddedtest
6. Click Create (and wait)

## Create a user (Requires Azure AD Admin)
1. Click on Manage new Directory
2. Click on Users
3. Click New User (e.g. adampaternostro@adamembeddedtenant.onmicrosoft.com)
4. Enter their information
   - Set their Role to Global Admin

## Create a Service Principle (Requires Azure AD Admin)
1. Go to Azure Active Directory
2. Create a new service principal
   - App Registrations
   - New Registration
   - Call it PowerBIEmbeddedSP
   - Web | https://PowerBIEmbeddedSP
4. Grant Permissions for the Service Principle to PowerBI
   - Click on API Permissions
   - Click on Add a Permission
   - Click on PowerBI Service
   - Click on Delegated Permissions
      - App.Read.All
      - Capacity.Read.All
      - Capacity.ReadWrite.All
      - Content.Create
      - Dashboard.Read.All
      - Dashboard.ReadWrite.All
      - Dataflow.Read.All
      - Dataflow.ReadWrite.All
      - Dataset.Read.All
      - Dataset.ReadWrite.All
      - Gateway.Read.All
      - Gateway.ReadWrite.All
      - Report.Read.All
      - Report.ReadWrite.All
      - StorageAccount.Read.All
      - StorageAccount.ReadWrite.All
      - Workspace.Read.All
      - Workspace.ReadWrite.All
   - Click on Add Permissions


## Create a Security Group (Requires Azure AD Admin)  
1. Go to Azure Active Directory
2. Click on Groups
3. Create a new Group
   - Group Type: Security
   - Group Name: PowerBIEmbeddedGroup
   - Members: Add PowerBIEmbeddedSP
   - Click Create


## Create Power BI Account 
If you have a license you can skip this step
1. Login to https://app.powerbi.com/
2. Click on Workspaces 
3. Click Create a Workspace
4. Click Free Trial


## Configure PowerBI to use Service Pricipal (Requires Azure AD Admin)  
1. Login to https://app.powerbi.com/ 
2. Click the gear icon on the top right
3. Click Admin portal
   - Click Tenant Settings
   - Scroll to "Allow Service principals to use Power BI APIs"
   - Click Enabled
   - Click "Specific Security Groups"
   - Add PowerBIEmbeddedGroup
   - Click Apply


## Create PowerBI Workspace
1. Login to https://app.powerbi.com/
2. Create a workspace
   - Click on Workspaces 
   - Click Create a Workspace
   - Name it: EmbeddedReports
   - Click Save
3. Add security to the workspace   
   - Click on Workspaces
   - Click on the "..." of the EmbeddedReport workspace
   - Select Workspace Access
   - Add PowerBIEmbeddedSP as an Admin
   - Click Add


## Create Sample Report
1. Click on Workspaces
2. Click on EmbeddedReports
3. Click Create | DataSets
4. Click Files
5. Click Samples
6. Click IT Spend Sample
7. Copy the Workspace ID and Report ID from the URL


## Run Sample Report using the PowerBI Sample Application
1. Download sample from here (Zip file) https://github.com/Microsoft/PowerBI-Developer-Samples
2. Open in Visual Studio
3. Unblock Zip
4. Unzip the file
5. Go into AppOwnsData folder
6. Restore NuGet
7. Rebuild
8. Update web.config (set these values)
```
     <add key="AuthenticationType" value="ServicePrincipal"/>
     <add key="applicationId" value="SET-THIS-VALUE"/>
     <add key="workspaceId" value="SET-THIS-VALUE"/>
     <add key="reportId" value="SET-THIS-VALUE"/>
     <add key="applicationSecret" value="SET-THIS-VALUE"/>
     <add key="tenant" value="SET-THIS-VALUE"/>
```
9. Run it and click on Embed Report


## Run Azure Function App
1. Make sure you have the Azure Function Tools installed
   - https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local
   - You will need Node.js installed and then install a Node package
2. Open PowerBI-Embedded-Sample.sln
3. Open local.settings.json and replace
   - applicationId
   - workspaceId
   - applicationSecret
   - tenant
4. Run the applicatoin
5. Copy the URL
6. Pass in the reportId to the URL
   - e.g. http://localhost:7071/api/PowerBIEmbeddedToken?reportId=123f9776-bd48-43e0-802a-0162359e1cfb   


## Create a Function App in Azure
1. Open https://portal.azure.com
2. Create a Resource Group 
3. Create a Function App
  - Name it what you like
  - Runtime Stack: .NET Core
  - Click Review and Create


## Publish Function App
1. Open Visual Studio 
2. Open PowerBI-Embedded-Sample.sln
3. Right click on ReportToken
4. Click on Publish
5. In Azure Portal
6. Click on Function App
7. Click on Configuration
8. Add each of the values from the local.settings.json


## Run Function App
1. Click on the Function App in Azure
2. Click the Run button
3. In the test window add a Parmeter
   - reportId and your PowerBI Report Id (Guid)


## Run a Web Page (basic JavaScript) to embed a PowerBI report
1. Go to the Resource Group your Function App
2. Click on the storage account
3. Click on Containers
4. Create new Container (name it "powerbi")
5. Click on the Container
6. Upload the powerbi.min.js
7. Click Generate SAS token 
   - Set the Start Date in the past
   - Set the Expiry Date in the future 
   - Copy the Blob SAS token
8. Back in Visual Studio
   - Open the folder "PowerBI-Embedded-JavaScript"
   - Open index.html
      - Paste he SAS Token on line 28
      - Set the Function Name on line 34
      - Set the Function Code on line 35
      - Set the Report Id on line 14
9. Back in the Azure Portal
   - Upload the index.html to the same storage account and container
   - Generate a SAS token
   - Paste the SAS token in a browser (you might need to wait a minute for the SAS token to work)
   - Click the "Click to Run Report" button
10. Copy the storage account name 
   - e.g. https://storageaccountadamp9397.blob.core.windows.net  
11. Go back to your Function App
   - Click on Platform Features
   - Click on CORS
   - Add your storage account (e.g. https://storageaccountadamp9397.blob.core.windows.net)
   - Click Save
12. Rerun the index.html report in your browser   


# Create the Power BI Embedded Capacity
Currently, you have been testing using Power BI resources.  In order to distribute reports using Power BI embedded you must use your Premium Capcity or create Embedded capacity to ensure you in compliance with licensing.
1. Open the Azure Portal
2. Create a new resource Power BI Embedded

![alt tag](https://raw.githubusercontent.com/AdamPaternostro/Azure-Power-BI-Embedded-Sample/master/Images/CreatePowerBIEmbedded.png)

![alt tag](https://raw.githubusercontent.com/AdamPaternostro/Azure-Power-BI-Embedded-Sample/master/Images/FinalCreatePowerBIEmbedded.png)

3. Open https://app.powerbi.com
4. Click on Workspaces
5. On your workspaces click the "..." and select Workspace settings
6. Click on Premium
7. Assoicate your capacity

![alt tag](https://raw.githubusercontent.com/AdamPaternostro/Azure-Power-BI-Embedded-Sample/master/Images/AssociateCapacity.png)

8.  In the Azure Portal you can pause/un-pause your Power BI Embedded Capacity for your development and QA testing.


# Links
- Playground: https://microsoft.github.io/PowerBI-JavaScript/demo/v2-demo/index.html
- Start / Stop: https://docs.microsoft.com/en-us/power-bi/developer/azure-pbie-pause-start.  You could author a Azure Function or Azure Automation to schedule a suspend and resume of your Embedded capacity.
  - https://docs.microsoft.com/en-us/rest/api/power-bi-embedded/capacities/suspend
  - https://docs.microsoft.com/en-us/rest/api/power-bi-embedded/capacities/resume
- Sample: https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers
- https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embedding-Basics



