using UserProfiles.Api.Domain.DTOs;
using UserProfiles.Api.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace UserProfiles.Api.Infrastructure.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        [MapperIgnoreTarget(nameof(User.Id))]
        [MapperIgnoreTarget(nameof(User.CreatedAt))]
        public partial User ToEntity(CreateProfileRequest dto);
    }
}
