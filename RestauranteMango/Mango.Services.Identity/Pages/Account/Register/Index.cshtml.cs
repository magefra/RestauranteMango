using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Mango.Services.Identity.Pages.Account.Register
{

    [SecurityHeaders]
    [AllowAnonymous]
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleInManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEventService _events;

        public Index(
                    IIdentityServerInteractionService interaction,
                    IAuthenticationSchemeProvider schemeProvider,
                    IClientStore clientStore,
                    UserManager<ApplicationUser> userManager,
                    RoleManager<IdentityRole> roleInManager,
                    SignInManager<ApplicationUser> signInManager,
                    IEventService events)
        {
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _userManager = userManager;
            _roleInManager = roleInManager;
            _signInManager = signInManager;
            _events = events;
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



        public async Task<IActionResult> OnPost(string returnUrl)
        {


            Console.WriteLine(returnUrl);

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (!_roleInManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
                    {
                        var userRole = new IdentityRole
                        {
                            Name = Input.RoleName,
                            NormalizedName = Input.RoleName,

                        };
                        await _roleInManager.CreateAsync(userRole);
                    }

                    await _userManager.AddToRoleAsync(user, Input.RoleName);



                    await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, Input.Username),
                            new Claim(JwtClaimTypes.Email, Input.Email),
                            new Claim(JwtClaimTypes.FamilyName, Input.FirstName),
                            new Claim(JwtClaimTypes.GivenName, Input.LastName),
                            new Claim(JwtClaimTypes.WebSite, "http://"+Input.Username+".com"),
                            new Claim(JwtClaimTypes.Role,"User") });




                    var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
                    var loginresult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);


                    if (loginresult.Succeeded)
                    {
                        var checkuser = await _userManager.FindByNameAsync(Input.Username);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return this.LoadingPage(Input.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(Input.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(Input.ReturnUrl))
                        {
                            return Redirect(Input.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(Input.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }

                }


            }

            // something went wrong, show form with error
            await BuildModelAsync(Input.ReturnUrl);
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
