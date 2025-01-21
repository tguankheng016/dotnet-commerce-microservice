using CommerceMicro.IdentityService.Application.Users.Dtos;
using CommerceMicro.IdentityService.Application.Users.Features.CreatingUser.V1;
using CommerceMicro.IdentityService.Application.Users.Features.UpdatingUser.V1;
using CommerceMicro.IdentityService.Application.Users.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.IdentityService.Application.Users;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMapper
{
	public partial UserDto UserToUserDto(User user);

	[MapperIgnoreSource(nameof(CreateUserDto.Password))]
	public partial User CreateUserDtoToUser(CreateUserDto createUserDto);

	public partial CreateUserCommand CreateUserDtoToCreateUserCommand(CreateUserDto createUserDto);

	public partial CreateOrEditUserDto UserToCreateOrEditUserDto(User user);

	public partial void EditUserDtoToUser(EditUserDto editUserDto, User user);

	public partial UpdateUserCommand EdiUserDtoToUpdateUserCommand(EditUserDto editUserDto);
}
