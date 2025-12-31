using Microsoft.AspNetCore.Mvc;
using Notifications.Api.Models;
using Notifications.Api.Services.Interfaces;

namespace Notifications.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NotificationsController : ControllerBase
	{
		private readonly IEmailService _emailService;

		public NotificationsController(IEmailService emailService)
		{
			_emailService = emailService;
		}

		[HttpPost("welcome")]
		public async Task<IActionResult> SendWelcomeEmail([FromBody] UserCreatedEvent request)
		{
			await _emailService.SendWelcomeEmailAsync(request.Nome, request.Email);
			return Ok(new { message = "E-mail de boas-vindas enviado (simulado)." });
		}

		[HttpPost("confirmation")]
		public async Task<IActionResult> SendOrderConfirmation([FromBody] PaymentProcessedEvent request)
		{
			if (request.Status == PaymentStatus.Approved)
			{
				await _emailService.SendOrderConfirmationAsync(request.EmailUser, request.OrderId, request.Price);
				return Ok(new { message = "E-mail de confirmação enviado (simulado)." });
			}
			return BadRequest(new { message = "Pagamento não aprovado, e-mail não enviado." });
		}

		[HttpPost("generic")]
		public async Task<IActionResult> SendGenericEmail([FromBody] GenericEmailRequest request)
		{
			await _emailService.SendGenericEmailAsync(request.To, request.Subject, request.Body);
			return Ok(new { message = "E-mail genérico enviado (simulado)." });
		}
	}
}
