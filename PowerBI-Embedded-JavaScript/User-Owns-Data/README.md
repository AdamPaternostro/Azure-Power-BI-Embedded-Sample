# This is a User Owns Data
User owns data means we need a proper Azure AD account to login to Power BI from which we will get the User Id.

To run this you will do the exact same steps as you did before, but when you need to register (or update_ your Azure AD Application (service principle) with a call back URL (http://localhost:13526/).

To get an Azure AD token with the proper audience, you need to run this application: https://github.com/microsoft/PowerBI-Developer-Samples/tree/master/User%20Owns%20Data/integrate-web-app

Use https://jwt.ms to view your token.  Run Embed Report and put a break point to print out your access token: System.Diagnostics.Debug.WriteLine(accessToken.Value);

Once you have the proper token open the power-bi-embedded-user-owns-data.html and update the JavaScript:
- accessToken
- reportId
- workSpaceId

The JavaScript will make a REST API call to the PowerBI REST APL and get the embed URL for the report.

The JavsScript will then run the report and embed into the page.

The idea behind this is that your application will be authentication with Azure AD and then be showing reports.  If you authenticate with another provider you should go with the App Owns Data model.
