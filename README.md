# QueryAsm

Based on the Azure AD Sample at https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-multitenant-openidconnect

## Setting up the sample
To set up the sample with Azure Active Directory, follow the steps in https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-multitenant-openidconnect/blob/master/README.md to
 * Create the organisational user account
 * Register the sample with your AAD tenant
 * Provision a key for your app
 * Configure the sample to use the AAD tenant

## Running the sample
Once you've configured the sample, run it and click Sign Up. 
Go through the process with an organisational account that has access to your Azure subscription(s).
After signing up, click on Subscriptions on the home page and you will see a list of subscriptions that have been retrieved using the credentials you signed into the application with.

From the subscriptions page you can click on
 * Storage (ASM) - page loaded by querying the ASM API directly
 * Storage (WAML) - page loaded using the Windows Azure Management Library (WAML)
 * Media Services (WAMLT) - page loaded using the Windows Azure Management Library (WAML)

