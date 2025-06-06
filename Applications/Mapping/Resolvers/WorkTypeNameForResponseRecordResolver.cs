using Applications.Records.Pricing;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping.Resolvers
{
    public class WorkTypeNameForResponseRecordResolver 
        : IValueResolver<TablePriceItem, TablePriceItemsResponseRecord, string?>
    {
        public string? Resolve(TablePriceItem source, TablePriceItemsResponseRecord destination, string? destMember, ResolutionContext context)
        {
            return source.WorkType?.Name;
        }
    }
}
