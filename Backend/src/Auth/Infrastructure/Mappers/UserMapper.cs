using Auth.Data.DTOs;
using Auth.Models;
using Riok.Mapperly.Abstractions;

namespace Auth.Infrastructure.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        [MapperIgnoreTarget(nameof(User.Id))]
        [MapperIgnoreTarget(nameof(User.CreatedAt))]
        public partial User ToEntity(RegistrationRequest dto);
    }
}
