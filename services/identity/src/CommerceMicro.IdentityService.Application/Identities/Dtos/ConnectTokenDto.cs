namespace CommerceMicro.IdentityService.Application.Identities.Dtos;

public class ConnectTokenResponseDto
{
    public string? Access_Token { get; set; }

    public string? Refresh_Token { get; set; }
}

public class ConnectTokenErrorDto
{
    public string? Error { get; set; }

    public string? Error_Description { get; set; }
}

public class ConnectUserInfoResponseDto
{
    public string? Sub { get; set; }

    public string? Preferred_Username { get; set; }

    public string? Family_Name { get; set; }

    public string? Given_Name { get; set; }

    public string? Email { get; set; }

    public bool Email_Verified { get; set; }

    /// <summary>
    /// Base 64 Format
    /// </summary>
    public string? Picture { get; set; }
}