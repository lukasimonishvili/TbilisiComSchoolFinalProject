using Domain.DTO.Authentication;
using Domain.Entities;
using Mapster;

namespace Infrastructure.Maps
{
    public static class UserMapper
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig<RegisterDTO, User>
                .NewConfig();
        }
    }
}
