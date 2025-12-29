using BankClientAPI.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BankClientAPI.Services
{
    /// <summary>
    /// Interfaz que define el contrato del servicio
    /// Permite inyección de dependencias y facilita testing
    /// </summary>
    public interface IExternalApiService
    {
        Task<List<ExternalUserDto>> GetExternalUsersAsync();
        Task<ExternalUserDto?> GetExternalUserByIdAsync(int id);
    }

    /// <summary>
    /// Servicio que consume la API externa JSONPlaceholder
    /// Demuestra cómo hacer llamadas HTTP a APIs externas
    /// </summary>
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ExternalApiService> _logger;

        // Constructor con inyección de dependencias
        // HttpClient viene del pool de HttpClientFactory (configurado en Program.cs)
        // ILogger permite registrar eventos y errores
        public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene lista de usuarios de la API externa
        /// Async/await permite operaciones no bloqueantes
        /// </summary>
        public async Task<List<ExternalUserDto>> GetExternalUsersAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios de API externa...");

                // GetAsync hace una petición HTTP GET
                var url = "https://jsonplaceholder.typicode.com/users";
                var response = await _httpClient.GetAsync(url);

                // EnsureSuccessStatusCode lanza excepción si no es 2xx
                response.EnsureSuccessStatusCode();

                // Leemos el contenido de la respuesta como string
                var jsonContent = await response.Content.ReadAsStringAsync();

                // Deserializamos el JSON a nuestros objetos C#
                var users = JsonSerializer.Deserialize<List<ExternalUserDto>>(jsonContent, 
                    new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true // Ignora diferencias en mayúsculas
                    });

                _logger.LogInformation($"Se obtuvieron {users?.Count ?? 0} usuarios");

                return users ?? new List<ExternalUserDto>();
            }
            catch (HttpRequestException ex)
            {
                // Capturamos errores de red/HTTP
                _logger.LogError(ex, "Error al llamar a la API externa");
                throw new Exception("No se pudo conectar con la API externa", ex);
            }
            catch (JsonException ex)
            {
                // Capturamos errores de deserialización
                _logger.LogError(ex, "Error al parsear respuesta de API externa");
                throw new Exception("Respuesta inválida de la API externa", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por ID
        /// </summary>
        public async Task<ExternalUserDto?> GetExternalUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo usuario {id} de API externa...");
                var url = $"https://jsonplaceholder.typicode.com/users/{id}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Usuario {id} no encontrado en API externa");
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<ExternalUserDto>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario {id}");
                throw;
            }
        }
    }
}