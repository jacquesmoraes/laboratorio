using AutoMapper;
using Core.Models.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Mapping.Resolvers
{
    public class TablePriceNameResolver : IValueResolver<TablePriceItem, object, string?>
{
    public string? Resolve(TablePriceItem source, object destination, string? destMember, ResolutionContext context)
    {
        return source.TablePrice?.Name;
    }
}
}
