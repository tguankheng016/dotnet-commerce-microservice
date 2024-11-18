
using Microsoft.AspNetCore.Http;

namespace CommerceMicro.Modules.Core.Exceptions;

public class ForbiddenException : CustomException
{
	public ForbiddenException(string message, int? code = StatusCodes.Status403Forbidden)
		: base(message, code: code)
	{
	}
}
