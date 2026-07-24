using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages;

public class NotFoundModel : PageModel
{
    public string? Message { get; set; } = "El recurso solicitado no fue encontrado.";

    public void OnGet()
    {
      if (HttpContext.Items.TryGetValue("ErrorMessage", out var message))
      {
          Message = message?.ToString();
      }
    }
}
