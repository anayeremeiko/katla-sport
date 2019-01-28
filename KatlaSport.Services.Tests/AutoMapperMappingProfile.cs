using System;
using AutoMapper;
using KatlaSport.Services.HiveManagement;
using KatlaSport.Services.ProductManagement;

namespace KatlaSport.Services.Tests
{
    internal class AutoMapperMappingProfile
    {
        private static readonly Lazy<AutoMapperMappingProfile> instance = new Lazy<AutoMapperMappingProfile>(() => new AutoMapperMappingProfile());

        private AutoMapperMappingProfile()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new HiveManagementMappingProfile());
                cfg.AddProfile(new ProductManagementMappingProfile());
            });
        }

        public static AutoMapperMappingProfile Instance => instance.Value;
    }
}
