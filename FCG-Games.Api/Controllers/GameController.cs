using FCG_Games.Application.Games.Requests;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Application.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace FCG_Games.Api.Controllers
{
    [ApiController]
    [Route("games")]
    public class GameController(IGameService service) : ControllerBase
    {
        /// <summary>
        /// Cadastra um novo jogo.
        /// </summary>
        /// <param name="request">Dados necessários para o cadastro do jogo.</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>       
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPost]
        public async Task<IResult> CreateGameAsync([FromBody] GameRequest request, CancellationToken cancellation = default)
        {
            var result = await service.CreateGameAsync(request, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Created();
        }

        /// <summary>
        /// Busca um jogo pelo seu ID.
        /// </summary>
        /// <param name="id">Id do jogo a ser buscado</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:guid}")]
        public async Task<IResult> GetGameByIdAsync(Guid id, CancellationToken cancellation = default)
        {

            var result = await service.GetGameByIdAsync(id, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);

        }

        /// <summary>
        /// Busca todos os jogos cadastrados.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisiçãoo.</param>       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IResult> GetAllGamesAsync(int page, CancellationToken cancellation = default)
        {
            var result = await service.GetAllGamesAsync(page, cancellation);
            return TypedResults.Ok(result);
        }

        /// <summary>
        /// Busca todos os jogos de forma customizada por usuário.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisiçãoo.</param>       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet("/custom")]
        public async Task<IResult> GamesCustomizedSearchAsync(int page, string search, CancellationToken cancellation = default)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return TypedResults.Unauthorized();

            var userId = Guid.Parse(User.FindFirst("UserId")?.Value!);
            return TypedResults.Ok(await service.GetCustomizedGamesSearchAsync(page, userId, search, cancellation));
        }

        /// <summary>
        /// Busca os jogos mais vendidos.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisiçãoo.</param>       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("/best-selling")]
        public async Task<IResult> BestSellingGamesAsync(int page, CancellationToken cancellation = default)
        => TypedResults.Ok(await service.GetBestSellingGamesAsync(page, cancellation));

        /// <summary>
        /// Remove um jogo pelo seu ID.
        /// </summary>
        /// <param name="id">Id do jogo a ser removido</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisi��o.</param>        
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IResult> DeleteGameAsync(Guid id, CancellationToken cancellation = default)
        {

            var result = await service.DeleteGameAsync(id, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.Conflict(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.NoContent();
        }

        /// <summary>
        /// Atualiza os dados de um jogo pelo seu ID.
        /// </summary>
        /// <param name="id">Id do jogo a ser atualizado </param>
        /// <param name="request">Novos dados que serão atribu�dos ao jogo</param>
        /// <param name="cancellation">Token para monitorar o cancelamento da requisição.</param>    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IResult> UpdateGameAsync(Guid id, [FromBody] GameRequest request, CancellationToken cancellation = default)
        {
            var result = await service.UpdateGameAsync(id, request, cancellation);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok();
        }

        /// <summary>
        /// Debug - Verifica autenticação e claims
        /// </summary>
        [HttpGet("/debug-auth")]
        public IActionResult DebugAuth()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var apimUserId = Request.Headers["X-User-Id"].ToString();
            var apimEmail = Request.Headers["X-User-Email"].ToString();
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return Ok(new
            {
                // Headers recebidos
                Headers = new
                {
                    AuthorizationHeader = string.IsNullOrEmpty(authHeader) ? "Ausente" : $"Presente ({authHeader.Substring(0, Math.Min(50, authHeader.Length))}...)",
                    XUserId = string.IsNullOrEmpty(apimUserId) ? "Ausente" : apimUserId,
                    XUserEmail = string.IsNullOrEmpty(apimEmail) ? "Ausente" : apimEmail
                },

                // Estado de autenticação
                Authentication = new
                {
                    IsAuthenticated = isAuthenticated,
                    AuthenticationType = User.Identity?.AuthenticationType,
                    IdentityName = User.Identity?.Name
                },

                // Claims do usuário
                Claims = claims,

                // Claims específicas (se houver)
                SpecificClaims = new
                {
                    UserId = User.FindFirst("UserId")?.Value ?? "Não encontrado",
                    Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "Não encontrado",
                    Name = User.FindFirst("Name")?.Value ?? "Não encontrado",
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Não encontrado"
                }
            });
        }
    }
}
