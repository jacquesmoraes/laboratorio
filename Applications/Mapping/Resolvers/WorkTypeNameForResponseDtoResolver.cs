using Applications.Dtos.Pricing;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping.Resolvers
{
    public class WorkTypeNameForResponseDtoResolver : IValueResolver<TablePriceItem, TablePriceItemsResponseDto, string?>
    {
        public string? Resolve ( TablePriceItem source, TablePriceItemsResponseDto destination, string? destMember, ResolutionContext context )
        {
            return source.WorkType?.Name;
        }
    }

}
