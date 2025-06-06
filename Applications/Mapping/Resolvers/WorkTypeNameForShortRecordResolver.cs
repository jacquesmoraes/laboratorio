using Applications.Records.Pricing;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping.Resolvers
{
    public class WorkTypeNameForShortRecordResolver 
        : IValueResolver<TablePriceItem, TablePriceItemShortRecord, string>
    {
        public string Resolve(TablePriceItem source, TablePriceItemShortRecord destination, string destMember, ResolutionContext context)
        {
            return source.WorkType?.Name ?? string.Empty;
        }
    }
}
