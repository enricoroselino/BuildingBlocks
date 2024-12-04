﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nebx.API.BuildingBlocks.Infrastructure.Helpers;

namespace Nebx.API.BuildingBlocks.Infrastructure.Services.TokenProvider;

public class TokenProvider : ITokenProvider
{
    public string Scheme => JwtBearerDefaults.AuthenticationScheme;
    public TokenValidationParameters TokenValidationParameters { get; init; }
    private static string Algorithms => SecurityAlgorithms.HmacSha256Signature;
    private readonly IOptions<TokenProviderOptions> _tokenSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;

    public TokenProvider(IOptions<TokenProviderOptions> tokenSettings)
    {
        _tokenSettings = tokenSettings;
        _tokenHandler = new JwtSecurityTokenHandler();

        var saltedKey = CryptoHelper.DerivationKey(tokenSettings.Value.Key, tokenSettings.Value.Salt, 32);
        var symmetricKey = new SymmetricSecurityKey(saltedKey);
        _signingCredentials = new SigningCredentials(symmetricKey, Algorithms);
        TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenSettings.Value.ValidIssuer,
            ValidAudience = tokenSettings.Value.ValidAudience,
            IssuerSigningKey = symmetricKey,
            ClockSkew = TimeSpan.Zero,
        };
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var expiration = DateTime.UtcNow.Add(_tokenSettings.Value.ValidSpan);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _tokenSettings.Value.ValidIssuer,
            Audience = _tokenSettings.Value.ValidAudience,
            SigningCredentials = _signingCredentials,
            Subject = new ClaimsIdentity(claims),
            TokenType = "JWT",
            Expires = expiration,
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = _tokenHandler.WriteToken(token);

        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        return accessToken;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Base64UrlEncoder.Encode(randomNumber);
    }
}