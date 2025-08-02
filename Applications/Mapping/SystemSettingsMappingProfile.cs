namespace Applications.Mapping
{
    public class SystemSettingsMappingProfile : Profile
    {
        public SystemSettingsMappingProfile ( )
        {
         
            
            CreateMap<SystemSettings, SystemSettingsRecord> ( )
                .ForMember ( dest => dest.LogoUrl,
                opt => opt.MapFrom ( src => !string.IsNullOrEmpty ( src.LogoFileName )
                ? $"/uploads/logos/{src.LogoFileName}": null ) );


            CreateMap<LabAddressRecord, LabAddress> ( );
            CreateMap<LabAddress, LabAddressRecord> ( );
        }
    }

}
