using CommerceMicro.IdentityService.Application.Roles.Dtos;
using CommerceMicro.IdentityService.Application.Roles.Features.CreatingRole.V1;
using CommerceMicro.IdentityService.Application.Roles.Features.UpdatingRole.V1;
using CommerceMicro.IdentityService.Application.Roles.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.IdentityService.Application.Roles;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RoleMapper
{
	public partial RoleDto RoleToRoleDto(Role role);

	public partial Role CreateRoleDtoToRole(CreateRoleDto createRoleDto);

	public partial void EditRoleDtoToRole(EditRoleDto editRoleDto, Role role);

	public partial CreateOrEditRoleDto RoleToCreateOrEditRoleDto(Role role);

	public partial CreateRoleCommand CreateRoleDtoToCreateRoleCommand(CreateRoleDto createRoleDto);

	public partial UpdateRoleCommand EditRoleDtoToUpdateRoleCommand(EditRoleDto editRoleDto);
}
