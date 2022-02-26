using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzureApp31
{
    public class Constants
    {
        public const string GoogleClientId = "131468551867-n3q9jrnqqagvhr40cfkc0p86vg22or22.apps.googleusercontent.com";
        public const string GoogleClientSecret = "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";

        public const string GoogleApiKey = "AIzaSyB3lE6gDC_Bu4avp3RwKASHG4iEjNKe7cg";

        public const string GoogleJson = "{\"web\":{\"client_id\":\"131468551867-n3q9jrnqqagvhr40cfkc0p86vg22or22.apps.googleusercontent.com\",\"project_id\":\"main-project-251210\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"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\"}}";
    }

    public class ApplicationUser
    {
        public string LoginName { get; set; }
        public string EmailAddress { get; set; }

        public string Phone { get; set; }
    }

    public class UserModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    }
}
