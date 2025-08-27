namespace API.Controllers.Utils
{
    public class ViaCepResponse
    {
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; }
        public string? Uf { get; set; }
        public string? Ibge { get; set; }
        public string? Gia { get; set; }
        public string? Ddd { get; set; }
        public string? Siafi { get; set; }

    }

    [Route ( "api/[controller]" )]
    [ApiController]
    public class CepController ( IHttpClientFactory httpClientFactory ) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient ( );

        [HttpGet ( "{cep}" )]
        [AllowAnonymous]
        public async Task<ActionResult<ViaCepResponse>> GetAddressByCep ( string cep )
        {
            if ( string.IsNullOrWhiteSpace ( cep ) )
                throw new CustomValidationException ( "CEP é obrigatório." );

            // Remove caracteres não numéricos
            var cleanCep = new string(cep.Where(char.IsDigit).ToArray());

            if ( cleanCep.Length != 8 )
                throw new CustomValidationException ( "CEP deve conter 8 dígitos." );

            var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cleanCep}/json/");

            if ( !response.IsSuccessStatusCode )
                throw new BadRequestException ( "Erro ao buscar o CEP no serviço externo." );

            var content = await response.Content.ReadAsStringAsync();
            if ( content.Contains ( "\"erro\": true" ) )
                throw new NotFoundException ( "CEP não encontrado." );
            var cepData = JsonSerializer.Deserialize<ViaCepResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });




            return Ok ( cepData );
        }
    }
}