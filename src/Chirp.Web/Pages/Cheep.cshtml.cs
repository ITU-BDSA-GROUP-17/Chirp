using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private IAuthorRepository _authorRepository;
        public CheepModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }

        public async Task<IActionResult> OnPost()
        {

            var Claims = User.Claims;
            var name = Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var author = await _authorRepository.GetAuthorByNameAsync(name);
            await _authorRepository.SendCheepAsync(GetNewCheepText, new AuthorInfoDTO(author.AuthorId, author.Name, author.Email));
            // Redirect in the end
            return Redirect("/");
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {

                Response.Redirect("/MicrosoftIdentity/Account/SignIn");

            }
        }
    }
}
