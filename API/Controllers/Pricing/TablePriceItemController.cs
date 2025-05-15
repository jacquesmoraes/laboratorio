using Applications.Dtos.Pricing;
using Applications.Interfaces;
using AutoMapper;
using Core.FactorySpecifications;
using Core.Models.Pricing;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Pricing
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class TablePriceItemController ( IMapper mapper, ITablePriceItemService tableService ) : BaseApiController
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITablePriceItemService _tableService = tableService;


        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec =  TablePriceItemSpecs.All();
            var items = await _tableService.GetAllWithSpecAsync(spec);
            var response = _mapper.Map<IEnumerable<TablePriceItemsResponseDto>>(items);
            return Ok ( response );
        }

        [HttpGet ( "GetByTableId/{tableId}" )]
        public async Task<IActionResult> GetByTableId ( int tableId )
        {
            var spec = TablePriceItemSpecs.ByTablePriceId(tableId);
            var items = await _tableService.GetAllWithSpecAsync(spec);
            var response = _mapper.Map<TablePriceItemsResponseDto> ( items );
            return Ok ( response );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = TablePriceItemSpecs.ByIdWithRelations(id);
            var item = await _tableService.GetEntityWithSpecAsync(spec);

            if ( item == null )
                return NotFound ( );
            var response = _mapper.Map<TablePriceItemsResponseDto> ( item);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateTablePriceItemDto dto )
        {
            var entity = _mapper.Map<TablePriceItem> ( dto );
            var created = await _tableService.CreateAsync(entity);

            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, created );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateTablePriceItemDto dto )
        {
            if ( dto.Id != id )
                return BadRequest ( "ID in URL does not match ID in body." );

            var updated = await _tableService.UpdateFromDtoAsync(id, dto);
            if ( updated == null ) return NotFound ( );

            var response = _mapper.Map<TablePriceItemsResponseDto>(updated);
            return Ok ( response );
        }


        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _tableService.DeleteAsync(id);
            if ( deleted == null ) return NotFound ( );
            return NoContent ( );
        }
    }
}
