# Jira Archive
This project is a simple web frontend that allows the user to search for issues in a Jira database.
It is designed to provide access to an archived on-prem Jira database, without the need to run an instance of Jira that might no longer be licensed or up to date.

It is designed to be extremely simple, and therefore most Jira features are not present in this system. It is means to simply retain access to historical ticket/issue data after a migration.


## Setting up your database
This application does not write to the database in any way.
You should set your SQL database to "Read Only" mode.


## Setting up authentication in AzureAD for this application
 1. Go to the Azure Portal
 2. Go to Azure Active Directory
 3. Go to Enterprise Applications
 4. Click "New Application"
 5. Click "Create your own application"
 6. Enter a name
 7. Select "Register an application to integrate with Azure AD"
 8. Click Create
 9. Click Register
 10. Go back to Azure Active Directory
 11. Click App Registrations
 12. Find your app and click into it
 13. Copy and paste the Application (client) ID into a text editor for later. The application will need this (see below).
 14. You will also need the Directory (tenant) ID, so copy and save this as well (see below).
 15. Click Certificates & Secrets
 16. Click "New client secret"
 17. Give the secret a name (the name will not matter)
 18. Click Add
 19. Copy and paste the client secret into a text editor for later. The application will need this (see below)
 20. Click "API Permissions" on the left
 21. Click "Add permission"
 22. Select Microsoft Graph
 23. Select Delegated Permissions
 24. Select "Email"
 25. Click "Add Permission"
 26. Now click "Grant Admin Consent for ...", then click "Yes" on the dialog box that appears.
 27. Click Authentication on the left
 28. Add a Redirect URI by clicking "Add URI"
 29. If you are teseting or developing, add `https://localhost:7147/signin-oidc`
 30. If you are running this in production somewhere, add the domain name you will give it, like this: `https://jiraarchive.domain.com/signin-oidc`. Make sure that `/signin-oidc` is at the end or you will have issues.
 31. Click "Save"


## Running this application

This web application is designed to run in a container.

This web application requires a number of configuration variables to exist for it to work. These must be passed in in such a way that dotnet's [Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) system can read them, such as an `appconfig.json` file, or via environment variables in a container.

Please keep in mind that the use of environment variables outside of a container is not safe.

This web application is designed to run with OAuth / OpenID Connect as an authentication mechanism. It will not run without the corresponding settings, and does not support other authentication mechanisms. It was written specifically to be used with Azure Active Directory, but should work with any OAuth / OpenID Connect provider, such as Google Workspaces or Okta.

| Configuration Variable | Description | Example |
|------------------------|-------------|---------|
| `ConnectionStrings:JiraDB` | Connection string to a Jira database (MS SQL). | `data source=databaseserver.hostname.or.ip\SQLINSTANCENAME;initial catalog=JiraServiceDesk;user id=USERNAME;password=PASSWORD;Trusted_Connection=false` |
| `OIDC:Authority` | The issuing authority for the OpenID Connect connection. | `https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000` |
| `OIDC:ClientID` | The ClientID for the OpenID Connect connection. If using AzureAD, this is the __Application ID__ of your _Enterprise Application_ you've set up for the OIDC connection. | `00000000-0000-0000-0000-000000000000` |
| `OIDC:ClientSecret` | A valid client secret from your OIDC server. | _A random string_ |

