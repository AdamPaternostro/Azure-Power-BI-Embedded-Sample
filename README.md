# Configure PowerBI Embedded and call from JavaScript
This will go through the process of configuring PowerBI embedded, creating an Azure function that will return the PowerBI embed tokens using a service principal and upload a sample web page to Azure Storage to run this a PowerBI embedded report using pure JavaScript.
The entire process can be quite confusing if you are new to Azure or just are focused on report authoring.

## Overview
This process will walk through the following
- Create an Azure tenant (use for testing, you can skip for your real Azure subscription)
- Create a dedicated Azure AD user for embedding.  
- Create a free PowerBI account
- Create a Service Principal for PowerBI by hand and grant permissions and get a key. 
  - I will not use the PowerBI online wizard (https://app.powerbi.com/embedsetup/appownsdata) to create the PowerBI setup.  
  - Sometimes the wizard does not work due to how companies have configured their Azure Subscription. If you use the wizard, you must use it as a Global Azure Admin and that Admin must have a PowerBI Pro account and they must have logged into https://app.powerbi.com/ before using the wizard.  You also must go generate a key by hand.
- Create an Azure AD security group and add the service principal from the PowerBI setup to the security group
- Configure PowerBI to use service principals that are in the security group you created
- Demostrate the downloaded sample application from the PowerBI setup
- Deploy an Azure Function that will call PowerBI to return the tokens needed to run a report in a web page
- Configure the included HTML pages to call the Azure Function and show the embedded report

## What you will need
- An Azure Subscription
- A PowerBI Pro license (a free account will work which we can create on the fly)
- You will need to be a global Azure Admin or have an Admin that can help you through the process.  The Admin should also be a PowerBI Admin.
- Visual Studio Community (or higher edition) downloaded.
- A resource group in Azure that you can deploy to.  In an enterprise typically an Admin will create the resource group and grant your Microsoft account Contributor access to the resource group.


## YouTube Video
- https://youtu.be/FZjQmvwBAdU


## Optional: Create a new Azure tenant (Require Azure AD Admin)
You can skip these steps if you are doing this in your company's Azure subscription.  Since this is a test I will show you in a test tenant.  I am not a Global Azure AD admin at Microsoft so I use a test tenant to perform PowerBI Embedded work.
1. Open the Azure Portal https://portal.azure.com
2. Click the menu in the top left and select Create a Resource
3. Search for Azure Active Directory
4. Click Create
5. Enter:
   - Organization name: Paternostro Embedded Test
   - Initial Domain Name: paternostroembeddedtest
6. Click Create (and wait)

## Create a user (Require Azure AD Admin)
1. Click on Manage new Directory
2. Click on Users
3. Click New User (e.g. adampaternostro@adamembeddedtenant.onmicrosoft.com)
4. Enter their information
   - Set their Role to Global Admin

## Create a Service Principle (Require Azure AD Admin)
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

## Create a Security Group  
1. Go to Azure Active Directory
2. Click on Groups
3. Create a new Group
   - Group Type: Security
   - Group Name: PowerBIEmbeddedGroup
   - Members: Add PowerBIEmbeddedSP
   - Click Create

## Create PowerBI Account 
1. Login to https://app.powerbi.com/
2. Click on Workspaces 
3. Click Create a Workspace
4. Click Free Trial



## Configure PowerBI to use Service Pricipal
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





# Links
- Playground: https://microsoft.github.io/PowerBI-JavaScript/demo/v2-demo/index.html
- Start / Stop: https://docs.microsoft.com/en-us/power-bi/developer/azure-pbie-pause-start
- Sample: https://docs.microsoft.com/en-us/power-bi/developer/embed-sample-for-customers
- https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embedding-Basics
- https://docs.microsoft.com/en-us/rest/api/power-bi-embedded/capacities/suspend
- https://docs.microsoft.com/en-us/rest/api/power-bi-embedded/capacities/resume


