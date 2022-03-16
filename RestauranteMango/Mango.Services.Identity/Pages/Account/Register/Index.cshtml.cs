using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mango.Services.Identity.Pages.Account.Register
{
    public class Index : PageModel
    {
        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [ViewData]
        public object Message { get; set; }

        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;


        public Index(
                    IIdentityServerInteractionService interaction,
                    IAuthenticationSchemeProvider schemeProvider,
                    IClientStore clientStore)
        {
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;

        }

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            await BuildModelAsync(returnUrl);

            if (View.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToPage("/ExternalLogin/Challenge/Index", new { scheme = View.ExternalLoginScheme, returnUrl });
            }

            return Page();
        }







        private async Task BuildModelAsync(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            List<string> roles = new List<string>();
            roles.Add("Admin");
            roles.Add("Customer");
            Message = roles;

            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                View = new ViewModel
                {
                    EnableLocalLogin = local,

                };

                Input.Username = context?.LoginHint;
                Input.ReturnUrl = returnUrl;

                if (!local)
                {
                    View.ExternalProviders = new[] { new ViewModel.ExternalProvider { AuthenticationScheme = context.IdP } };
                }
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ViewModel.ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();



            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            View = new ViewModel
            {
                AllowRememberLogin = RegisterOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && RegisterOptions.AllowLocalLogin,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
