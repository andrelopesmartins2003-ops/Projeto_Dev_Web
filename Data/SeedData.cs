// Importa as classes do ASP.NET Core Identity,
// usadas para criar utilizadores, roles e associar utilizadores a roles.
using Microsoft.AspNetCore.Identity;

// Define o namespace onde ficam as classes relacionadas com dados da aplicação.
namespace Projeto.Data;

// Classe estática responsável por inicializar dados essenciais na aplicação.
// Neste caso, cria roles e utilizadores padrão se ainda não existirem.
public static class SeedData
{
    // Método chamado no arranque da aplicação para preparar dados iniciais.
    // Recebe o IServiceProvider para obter serviços registados na aplicação.
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // Obtém o RoleManager, serviço responsável por criar e gerir roles/perfis de utilizador.
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Obtém o UserManager, serviço responsável por criar e gerir utilizadores.
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Define as roles que a aplicação deve ter por defeito.
        // Admin terá permissões administrativas e User será o utilizador comum.
        string[] roles = { "Admin", "User" };

        // Percorre todas as roles definidas no array anterior.
        foreach (var role in roles)
        {
            // Verifica se a role ainda não existe na base de dados.
            if (!await roleManager.RoleExistsAsync(role))
            {
                // Cria a role quando ela ainda não existe.
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Cria o utilizador administrador padrão, caso ainda não exista.
        await CreateUserIfNotExists(
            // Passa o UserManager para permitir criar e consultar utilizadores.
            userManager,
            // Email usado também como username do administrador.
            "admin@f1.pt",
            // Palavra-passe inicial do administrador.
            "Admin123!",
            // Role atribuída ao utilizador administrador.
            "Admin"
        );

        // Cria o utilizador comum padrão, caso ainda não exista.
        await CreateUserIfNotExists(
            // Passa o UserManager para permitir criar e consultar utilizadores.
            userManager,
            // Email usado também como username do utilizador comum.
            "user@f1.pt",
            // Palavra-passe inicial do utilizador comum.
            "User123!",
            // Role atribuída ao utilizador comum.
            "User"
        );
    }

    // Método auxiliar que cria um utilizador apenas se ele ainda não existir.
    // Evita criar utilizadores duplicados sempre que a aplicação arranca.
    private static async Task CreateUserIfNotExists(
        // Serviço usado para pesquisar, criar e gerir utilizadores.
        UserManager<IdentityUser> userManager,
        // Email do utilizador a criar ou procurar.
        string email,
        // Palavra-passe inicial do utilizador.
        string password,
        // Role que será atribuída ao utilizador depois de criado.
        string role)
    {
        // Procura na base de dados se já existe um utilizador com este email.
        var user = await userManager.FindByEmailAsync(email);

        // Só cria o utilizador se ele ainda não existir.
        if (user == null)
        {
            // Cria um novo objeto IdentityUser com os dados necessários.
            user = new IdentityUser
            {
                // Define o username igual ao email para simplificar o login.
                UserName = email,

                // Define o email do utilizador.
                Email = email,

                // Marca o email como confirmado para permitir login sem confirmação manual.
                EmailConfirmed = true
            };

            // Cria efetivamente o utilizador na base de dados com a password indicada.
            var result = await userManager.CreateAsync(user, password);

            // Verifica se a criação do utilizador foi bem-sucedida.
            if (result.Succeeded)
            {
                // Associa o utilizador recém-criado à role recebida por parâmetro.
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
