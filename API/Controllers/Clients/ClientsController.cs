using Applications.Contracts;
using Applications.Dtos.Clients;
using Applications.Projections.Clients;
using Applications.Records.Clients;
using AutoMapper;
using Core.Exceptions;
using Core.Models.Clients;
using Microsoft.AspNetCore.Mvc;
using static Core.FactorySpecifications.ClientsSpecifications.ClientSpecification;

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


            var response = _mapper.Map<List<ClientResponseRecord>>(clients);
            return Ok ( response );

        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var dto = await _clientService.GetClientDetailsProjectionAsync(id);
            return Ok ( dto );
        }

        


        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateClientDto dto )
        {
            var entity = _mapper.Map<Client>(dto);
            await _clientService.CreateClientAsync ( entity );
            return CreatedAtAction ( nameof ( GetById ), new { id = entity.ClientId }, _mapper.Map<ClientResponseRecord> ( entity ) );
        }


        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateClientDto client )
        {
            if ( id != client.ClientId )
                return BadRequest ( $"Client id {id} does not match with the id in the body {client.ClientId}." );
            var updatedClient = await _clientService.UpdateFromDtoAsync ( client );
            if ( updatedClient == null )
                return NotFound ( $"Client with id {id} not found." );
            var response = _mapper.Map<ClientResponseRecord>(updatedClient);
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
