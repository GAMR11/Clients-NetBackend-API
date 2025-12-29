using Microsoft.AspNetCore.Mvc;
using BankClientAPI.Services;
using BankClientAPI.DTOs;

namespace BankClientAPI.Controllers
{
    /// <summary>
    /// Controlador que expone endpoints para consumir la API externa
    /// Demuestra cómo integrar servicios externos en tu API
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalUsersController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;
        private readonly ILogger<ExternalUsersController> _logger;

        public ExternalUsersController(IExternalApiService externalApiService, ILogger<ExternalUsersController> logger)
        {
            _externalApiService = externalApiService;
            _logger = logger;
        }

        // GET: api/externalusers
        /// <summary>
        /// Obtiene usuarios desde JSONPlaceholder API
        /// Este endpoint actúa como un proxy a la API externa
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ExternalUserDto>>> GetExternalUsers()
        {
            try
            {
                _logger.LogInformation("Solicitando usuarios externos");
                
                var users = await _externalApiService.GetExternalUsersAsync();
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios externos");
                
                // StatusCode permite retornar códigos personalizados
                return StatusCode(502, new 
                { 
                    message = "Error al conectar con el servicio externo",
                    detail = ex.Message 
                });
            }
        }

        // GET: api/externalusers/5
        /// <summary>
        /// Obtiene un usuario específico de la API externa
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalUserDto>> GetExternalUser(int id)
        {
            try
            {
                _logger.LogInformation($"Solicitando usuario externo {id}");
                
                var user = await _externalApiService.GetExternalUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado en la API externa" });
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario externo {id}");
                return StatusCode(502, new 
                { 
                    message = "Error al conectar con el servicio externo" 
                });
            }
        }
    }
}