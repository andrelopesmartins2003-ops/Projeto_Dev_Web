using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Projeto.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }
        = new List<AuthenticationScheme>();

    public class InputModel
    {
        [Required(ErrorMessage = "O endereço de email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um endereço de email válido.")]
        [Display(Name = "Endereço de email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [StringLength(
            100,
            ErrorMessage = "A palavra-passe deve ter entre {2} e {1} caracteres.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar palavra-passe")]
        [Compare(
            nameof(Password),
            ErrorMessage = "As palavras-passe não coincidem.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins = (
            await _signInManager.GetExternalAuthenticationSchemesAsync()
        ).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins = (
            await _signInManager.GetExternalAuthenticationSchemesAsync()
        ).ToList();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = CreateUser();

        await _userStore.SetUserNameAsync(
            user,
            Input.Email,
            CancellationToken.None
        );

        await _emailStore.SetEmailAsync(
            user,
            Input.Email,
            CancellationToken.None
        );

        var result = await _userManager.CreateAsync(
            user,
            Input.Password
        );

        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Foi criada uma nova conta de utilizador."
            );

            await _signInManager.SignInAsync(
                user,
                isPersistent: false
            );

            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                string.Empty,
                TranslateIdentityError(error)
            );
        }

        return Page();
    }

    private static IdentityUser CreateUser()
    {
        return new IdentityUser();
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "O sistema de autenticação necessita de suporte para email."
            );
        }

        return (IUserEmailStore<IdentityUser>)_userStore;
    }

    private static string TranslateIdentityError(IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateEmail" =>
                "Já existe uma conta com este endereço de email.",

            "DuplicateUserName" =>
                "Já existe uma conta com este endereço de email.",

            "InvalidEmail" =>
                "O endereço de email introduzido não é válido.",

            "PasswordTooShort" =>
                "A palavra-passe é demasiado curta.",

            "PasswordRequiresNonAlphanumeric" =>
                "A palavra-passe deve conter pelo menos um símbolo.",

            "PasswordRequiresDigit" =>
                "A palavra-passe deve conter pelo menos um número.",

            "PasswordRequiresUpper" =>
                "A palavra-passe deve conter pelo menos uma letra maiúscula.",

            "PasswordRequiresLower" =>
                "A palavra-passe deve conter pelo menos uma letra minúscula.",

            _ => error.Description
        };
    }
}