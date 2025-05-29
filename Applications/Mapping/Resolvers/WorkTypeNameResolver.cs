using Applications.Dtos.Pricing;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping.Resolvers
{
     public class WorkTypeNameResolver : IValueResolver<TablePriceItem, TablePriceItemShortDto, string>
    {
        public string Resolve(TablePriceItem source, TablePriceItemShortDto destination, string destMember, ResolutionContext context)
        {
            return source.WorkType?.Name ?? string.Empty;
        }
    }
}
