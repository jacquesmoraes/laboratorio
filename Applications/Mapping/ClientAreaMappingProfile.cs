using Applications.Projections.ClientArea;

namespace Applications.Mapping
{
    public class ClientAreaMappingProfile : Profile
    {
        public ClientAreaMappingProfile ( )
        {
            CreateMap<BillingInvoice, ClientAreaInvoiceProjection> ( )
                .ForMember ( dest => dest.BillingInvoiceId,
                    opt => opt.MapFrom ( src => src.BillingInvoiceId ) )
                .ForMember ( dest => dest.InvoiceNumber,
                    opt => opt.MapFrom ( src => src.InvoiceNumber ) )
                .ForMember ( dest => dest.CreatedAt,
                    opt => opt.MapFrom ( src => src.CreatedAt ) )
                .ForMember ( dest => dest.Description,
                    opt => opt.MapFrom ( src => src.Description ) )
                .ForMember ( dest => dest.Status,
                    opt => opt.MapFrom ( src => src.Status ) )
                .ForMember ( dest => dest.TotalInvoiceAmount,
                    opt => opt.MapFrom ( src =>
                        src.TotalServiceOrdersAmount + src.PreviousDebit - src.PreviousCredit
                    ) );


            CreateMap<ServiceOrder, ClientAreaServiceOrderProjection> ( )
            .ForMember ( dest => dest.ServiceOrderId, opt => opt.MapFrom ( src => src.ServiceOrderId ) )
            .ForMember ( dest => dest.OrderNumber, opt => opt.MapFrom ( src => src.OrderNumber ) )
            .ForMember ( dest => dest.DateIn, opt => opt.MapFrom ( src => src.DateIn ) )
            .ForMember ( dest => dest.PatientName, opt => opt.MapFrom ( src => src.PatientName ) )
            .ForMember ( dest => dest.OrderTotal, opt => opt.MapFrom ( src => src.OrderTotal ) )
            .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status ) );

            // NEW: Details projection
            CreateMap<ServiceOrder, ClientAreaServiceOrderDetailsProjection> ( )
                .ForMember ( dest => dest.ServiceOrderId, opt => opt.MapFrom ( src => src.ServiceOrderId ) )
                .ForMember ( dest => dest.OrderNumber, opt => opt.MapFrom ( src => src.OrderNumber ) )
                .ForMember ( dest => dest.DateIn, opt => opt.MapFrom ( src => src.DateIn ) )
                .ForMember ( dest => dest.DateOut, opt => opt.MapFrom ( src => src.DateOut ) )
                .ForMember ( dest => dest.PatientName, opt => opt.MapFrom ( src => src.PatientName ) )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status ) )
                .ForMember ( dest => dest.OrderTotal, opt => opt.MapFrom ( src => src.OrderTotal ) )
               
                .ForMember ( dest => dest.Works, opt => opt.MapFrom ( src => src.Works ) )
                .ForMember ( dest => dest.Stages, opt => opt.MapFrom ( src => src.Stages ) );

            // ⚠️ Note on mapping ShadeColor and ScaleName:
            // explanation adjusted by chatgpt
            // Currently we use:
            //   .ForMember(dest => dest.ShadeColor, opt => opt.MapFrom(src => src.Shade!.Color))
            //   .ForMember(dest => dest.ScaleName, opt => opt.MapFrom(src => src.Shade!.Scale!.Name))
            //
            // Even though Work.ShadeId (and therefore Shade) can be NULL in the database,
            // this mapping does not throw because AutoMapper, when used with EF Core,
            // generates a SQL LEFT JOIN for Shade and Scale.
            // The resulting columns in SQL will be NULL if Shade or Scale does not exist,
            // and AutoMapper simply assigns null or string.Empty to the destination property
            // depending on how it’s defined.
            //
            //  Therefore, as long as we are using AutoMapper + EF Core + ProjectTo<T> or
            // mapping directly from query results, this mapping is safe.
            //
            //  Warning: if we ever switch to mapping already-loaded in-memory objects
            // (e.g., after a ToList() without includes, or detached entities),
            // accessing src.Shade!.Color will throw a NullReferenceException.
            //
            //  Best practices to ensure robustness:
            // - Keep the destination properties as string? or initialized to string.Empty
            // - If we migrate to mapping in-memory objects, change the mapping to a defensive ternary:
            //   src.Shade != null ? src.Shade.Color : string.Empty
            //
            //  Summary:
            // This mapping is safe today because it relies on EF Core’s SQL LEFT JOIN behavior.
            // Documented here for future reference.

            CreateMap<Work, ClientAreaWorkRecord> ( )
    .ForMember ( dest => dest.WorkTypeId, opt => opt.MapFrom ( src => src.WorkTypeId ) )
    .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
    .ForMember ( dest => dest.Quantity, opt => opt.MapFrom ( src => src.Quantity ) )
    .ForMember ( dest => dest.PriceUnit, opt => opt.MapFrom ( src => src.PriceUnit ) )
    .ForMember ( dest => dest.Notes, opt => opt.MapFrom ( src => src.Notes ) )
   .ForMember(dest => dest.ShadeColor, opt => opt.MapFrom(src => src.Shade != null ? src.Shade.color : string.Empty))
.ForMember(dest => dest.ScaleName, opt => opt.MapFrom(src => src.Shade != null && src.Shade.Scale != null ? src.Shade.Scale.Name : string.Empty));






            CreateMap<ProductionStage, StageRecord> ( )
                .ForMember ( dest => dest.SectorId, opt => opt.MapFrom ( src => src.SectorId ) )
                .ForMember ( dest => dest.SectorName, opt => opt.MapFrom ( src => src.Sector.Name ) )
                .ForMember ( dest => dest.DateIn, opt => opt.MapFrom ( src => src.DateIn ) )
                .ForMember ( dest => dest.DateOut, opt => opt.MapFrom ( src => src.DateOut ) );
        }
    }
}

