using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Projeto.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "O endereço de email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um endereço de email válido.")]
        [Display(Name = "Endereço de email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Manter sessão iniciada")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError(
            string.Empty,
            "Email ou palavra-passe incorretos."
        );

        return Page();
    }
}