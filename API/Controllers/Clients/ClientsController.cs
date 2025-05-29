using Applications.Contracts;
using Applications.Dtos.Clients;
using AutoMapper;
using Core.Models.Clients;
using Microsoft.AspNetCore.Mvc;
using static Core.FactorySpecifications.ClientsFactorySpecifications.ClientSpecification;

namespace API.Controllers.Clients
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClientsController ( IMapper mapper, IClientService clientService )
        : BaseApiController
    {
        private readonly IClientService _clientService = clientService;
        private readonly IMapper _mapper = mapper;
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = ClientSpecs.All();
            var clients = await _clientService.GetAllWithSpecAsync(spec);
            if ( clients == null || !clients.Any ( ) ) return NotFound ( );


            var response = _mapper.Map<List<ClientResponseDto>>(clients);
            return Ok ( response );

        }
        //TODO: verificar pq nao vem os pagamentos e nao aparece client balance
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = ClientSpecs.ById(id);
            var client = await _clientService.GetEntityWithSpecAsync(spec);
            var response = _mapper.Map<ClientResponseDetailsDto>(client);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateClientDto dto )
        {
            var entity = _mapper.Map<Client>(dto);
            await _clientService.CreateAsync ( entity );
            return CreatedAtAction ( nameof ( GetById ), new { id = entity.ClientId }, _mapper.Map<ClientResponseDto> ( entity ) );
        }


        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateClientDto client )
        {
            if ( id != client.ClientId )
                return BadRequest ( $"Client id {id} does not match with the id in the body {client.ClientId}." );
            var updatedClient = await _clientService.UpdateFromDtoAsync ( client );
            if ( updatedClient == null )
                return NotFound ( $"Client with id {id} not found." );
            var response = _mapper.Map<ClientResponseDto>(updatedClient);
            return Ok ( response );
        }


        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deletedClient = await _clientService.DeleteAsync ( id );
            if ( deletedClient == null )
                return NotFound ( $"Client with id {id} not found." );
            return NoContent ( );
        }






    }
}
