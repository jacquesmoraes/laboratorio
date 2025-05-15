using Applications.Dtos.Clients;
using Applications.Interfaces;
using AutoMapper;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.Models.Clients;
using Microsoft.AspNetCore.Mvc;
using static Core.FactorySpecifications.ClientsFactorySpecifications.ClientSpecification;

namespace API.Controllers
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClientsController (IMapper mapper, IGenericService<Client> clientService ) : BaseApiController
    {
        private readonly IGenericService<Client> _clientService = clientService;
         private readonly IMapper _mapper = mapper;
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = ClientSpecification.ClientSpecs.All();
            var clients = await _clientService.GetAllWithSpecAsync(spec);
            if ( clients == null || !clients.Any ( ) ) return NotFound ( );


            var response = _mapper.Map<List<ClientResponseDto>>(clients);
            return Ok ( response );

        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = ClientSpecs.ById(id);
            var client = await _clientService.GetEntityWithSpecAsync(spec);
            var response = _mapper.Map<ClientResponseDto>(client);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] Client client )
        {
            var createdClient = await _clientService.CreateAsync ( client );
            return CreatedAtAction ( nameof ( GetById ), new { id = createdClient.ClientId }, createdClient );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] Client client )
        {
            var updatedClient = await _clientService.UpdateAsync ( id, client );
            if ( updatedClient == null )
                return NotFound ( $"Client with id {id} not found." );
            return Ok ( updatedClient );
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
