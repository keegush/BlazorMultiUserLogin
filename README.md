# BlazorMultiUserLogin

**BlazorMultiUserLogin** is a Blazor Server application that implements Azure AD (Entra ID) authentication, designed for multi-user login functionality on shared devices like tablets. This project demonstrates how to integrate Azure Active Directory (AD) for secure user authentication, with support for multiple user accounts.

## Features

- Azure AD authentication with `OpenID Connect`.
- Multi-user login support for shared devices (tablets).
- Proper redirection on logout.
- Browser-based login session management using password autofill.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Azure AD (Entra ID) Tenant](https://portal.azure.com)
- Visual Studio 2022 or later

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/BlazorMultiUserLogin.git
cd BlazorMultiUserLogin
```

### 2. Azure AD (Entra ID) Setup

To configure the project to connect to **Azure AD (Entra ID)**, follow these steps:

1. **Register your application in Azure AD**:
    - Go to the **Azure Portal** and navigate to **Azure Active Directory**.
    - Under **App registrations**, select **New registration**.
    - Set a **name** (e.g., `BlazorMultiUserLogin`), choose the **supported account types** (e.g., Single Tenant or Multi-Tenant), and add a **redirect URI**:
        - Type: `Web`
        - Redirect URI: `https://localhost:7265/signin-oidc`
    - Click **Register**.

2. **Configure API Permissions**:
    - Once registered, navigate to **API Permissions** and add the following permissions:
        - Microsoft Graph > `User.Read`.
    - Make sure to **grant admin consent** for the required permissions.

3. **Configure Authentication Settings**:
    - Go to the **Authentication** tab.
    - Add **Logout URL**:
        - `https://localhost:7265/`.
    - Enable **ID tokens** under the **Implicit grant and hybrid flows** section.

4. **Generate Client Secret**:
    - Go to **Certificates & Secrets**.
    - Under **Client Secrets**, create a new secret, copy the secret value, and store it securely. You'll need it for the application configuration.
    - Note: Using an Azure Key Vault to securely store sensitive configuration values like the ClientSecret is a best practice in production environments. This example uses environment variables for simplicity in development.
If you choose to use Azure Key Vault, make sure to configure it with your app and retrieve the ClientSecret securely.

5. **Update Azure AD Configuration in `appsettings.json`**:
   Open the `appsettings.json` file and update it with your Azure AD details:

   ```json
   {
     "AzureAd": {
       "Instance": "https://login.microsoftonline.com/",
       "Domain": "yourdomain.onmicrosoft.com",
       "TenantId": "your-tenant-id",
       "ClientId": "your-client-id",
       "CallbackPath": "/signin-oidc"
     }
   }
   ```

6. **Enable Persistent Browser Sessions (Optional)**: If desired, you can enable persistent browser sessions in the Azure AD tenant:
  - In Azure AD, navigate to Enterprise Applications -> User Settings.
  - Enable Users can sign in without prompting again to allow multi-user persistence.

### 3. Update Application Configuration

Make sure the `ClientSecret` is added to your configuration securely (e.g., via an environment variable). Environment variables are used here for demonstration purposes, but it's recommended to use **Azure Key Vault** for storing sensitive secrets in production environments.

Example in `Program.cs`:
```csharp
var environmentName = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
```

### 4. Run the Application
Run the project from Visual Studio or the command line:
```bash
dotnet run
```

### 5. Test the Application
  - Navigate to https://localhost:7265.
  - Log in with any of the Azure AD accounts registered in your tenant.
  - Use the Logout button to sign out, and the application will redirect back to the homepage using the configured SignedOutCallbackPath.

## Usage
  - Login: The login button uses Azure AD authentication to sign in users.
  - Logout: The application redirects users back to the homepage after logging out using the SignedOutCallbackPath configured in Program.cs.
  - Multi-user Login: Browser autofill will show the last users who signed in if the browser saves their credentials.

##License
This project is licensed under the MIT License.
