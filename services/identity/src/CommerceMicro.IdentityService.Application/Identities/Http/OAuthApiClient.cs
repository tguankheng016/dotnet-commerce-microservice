using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CommerceMicro.IdentityService.Application.Identities.Dtos;
using CommerceMicro.Modules.Core.Dependencies;
using CommerceMicro.Modules.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace CommerceMicro.IdentityService.Application.Identities.Http;

public interface IOAuthApiConfiguration : ISingletonDependency
{
	string ClientId { get; }

	string ClientSecret { get; }

	string BaseUrl { get; }
}

public interface IOAuthApiClient : IScopedDependency
{
	Task<ConnectTokenResponseDto> ConnectTokenAsync(string code, string redirectUri);

	Task<ConnectUserInfoResponseDto> ConnectUserInfoAsync(string accessToken);
}

public class OAuthApiClient : IOAuthApiClient
{
	public static readonly string ClientName = "OAuth-Client";
	private readonly IHttpClientFactory _clientFactory;
	private readonly string _clientId;
	private readonly string _clientSecret;

	public OAuthApiClient(
		IHttpClientFactory clientFactory,
		IConfiguration configuration)
	{
		_clientFactory = clientFactory;
		var baseOpenIddictKey = "Authentication:OpenIddict";

		_clientId = configuration[$"{baseOpenIddictKey}:ClientId"]!;
		_clientSecret = configuration[$"{baseOpenIddictKey}:ClientSecret"]!;
	}

	public async Task<ConnectTokenResponseDto> ConnectTokenAsync(string code, string redirectUri)
	{
		//Authorization Flow
		using var client = _clientFactory.CreateClient(ClientName);

		var requestUri = "connect/token";
		using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

		var data = new[]
		{
			new KeyValuePair<string, string>("grant_type", "authorization_code"),
			new KeyValuePair<string, string>("code", code),
			new KeyValuePair<string, string>("client_id", _clientId),
			new KeyValuePair<string, string>("client_secret", _clientSecret),
			new KeyValuePair<string, string>("redirect_uri", redirectUri),
		};

		requestMessage.Content = new FormUrlEncodedContent(data);

		var response = await client.SendAsync(requestMessage);

		var jsonstring = await response.Content.ReadAsStringAsync();

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadFromJsonAsync<ConnectTokenResponseDto>();

			if (result is not null)
			{
				return result!;
			}
		}
		else
		{
			var error = await response.Content.ReadFromJsonAsync<ConnectTokenErrorDto>();

			if (error is not null && error.Error_Description is not null)
			{
				throw new BadRequestException(error?.Error_Description!);
			}
		}

		throw new BadRequestException("Something went wrong when exchanging token");
	}

	public async Task<ConnectUserInfoResponseDto> ConnectUserInfoAsync(string accessToken)
	{
		using var client = _clientFactory.CreateClient(ClientName);

		var requestUri = "connect/userinfo";
		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

		requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		var response = await client.SendAsync(requestMessage);

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadFromJsonAsync<ConnectUserInfoResponseDto>();

			if (result is not null)
			{
				return result!;
			}
		}
		else
		{
			var wwwAuthenticate = response.Headers.WwwAuthenticate;

			foreach (var auth in wwwAuthenticate)
			{
				var parameter = auth.Parameter;

				if (!string.IsNullOrEmpty(parameter))
				{
					var connectTokenError = JsonSerializer.Deserialize<ConnectTokenErrorDto>("{" + parameter.Replace("=", ":") + "}");

					if (connectTokenError is not null && connectTokenError.Error_Description is not null)
					{
						throw new BadRequestException(connectTokenError.Error_Description);
					}
				}
			}
		}

		throw new BadRequestException("Something went wrong when exchanging token");
	}
}