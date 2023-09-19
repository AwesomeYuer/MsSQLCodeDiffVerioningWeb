// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microshaoft;

public class JwtTokenHandlerManager
{
    private readonly Dictionary<string, TokenHandlerEntry>
            _handlers = new(StringComparer.OrdinalIgnoreCase);

    public class TokenHandlerEntry
    {
        public string? Name { get; set; }
        public SecurityTokenDescriptor? DefaultTokenDescriptor { get; } = new();
        public TokenValidationParameters? DefaultTokenValidationParameters { get; } = new();

        public TokenHandlerEntry(string Name)
        {
            this.Name = Name;
        }

        public bool TryValidateToken
                    (
                        HttpRequest httpRequest
                        , out JwtSecurityToken validatedJwtSecurityToken
                        , out ClaimsPrincipal validatedClaimsPrincipal
                        , out string errorMessage
                        , Action<TokenValidationParameters> action = null!
                    )
        {
            var jwt = string.Empty;
            validatedJwtSecurityToken = null!;
            validatedClaimsPrincipal = null!;
            var @return = httpRequest.Headers.TryGetValue("Authorization", out var authorizationHeader);
            if (@return)
            {
                var s = authorizationHeader.ToString();
                var prefix = "Bearer ";

                if
                    (
#pragma warning disable CA1310 // Specify StringComparison for correctness
                        s.StartsWith(prefix)
#pragma warning restore CA1310 // Specify StringComparison for correctness
                    )
                {
                    jwt = s[prefix.Length..];
                    @return = true;
                }
            }
            if (!@return)
            {
                errorMessage = "No Authorization Header";
                return @return;
            }

            return
                this.TryValidateToken
                        (
                            jwt
                            , out validatedJwtSecurityToken
                            , out validatedClaimsPrincipal
                            , out errorMessage
                            , action
                        );
        }

        public bool TryValidateToken
                    (
                        string jwtToken
                        , out JwtSecurityToken validatedJwtSecurityToken
                        , out ClaimsPrincipal validatedClaimsPrincipal
                        , out string errorMessage
                        , Action<TokenValidationParameters> action = null!
                    )
        {
            var @return = false;
            errorMessage = string.Empty;
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = this.DefaultTokenValidationParameters!.ValidAudience,
                    ValidIssuer = this.DefaultTokenValidationParameters.ValidIssuer,
                    IssuerSigningKey = this.DefaultTokenValidationParameters.IssuerSigningKey
                };

                action?.Invoke(validationParameters);

                var tokenHandler = new JwtSecurityTokenHandler();
                validatedClaimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validedSecurityToken);
                validatedJwtSecurityToken = (JwtSecurityToken)validedSecurityToken;
                @return = true;
            }
#pragma warning disable CA1031 // Specify StringComparison for clarity
            catch (Exception e)
#pragma warning restore CA1031 // Specify StringComparison for clarity
            {
                validatedClaimsPrincipal = null!;
                validatedJwtSecurityToken = null!;
                errorMessage = $"{e}";
                @return = false;
            }
            return @return;
        }

        public string IssueToken
                            (
                                string audience
                                , IEnumerable<Claim> subjectClaims
                                , DateTime expiresDateTime
                            )
        {
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = this.DefaultTokenDescriptor!.Issuer
                  , Audience = audience
                  , Subject = new ClaimsIdentity(subjectClaims)
                  , Expires = expiresDateTime
                  , SigningCredentials = this.DefaultTokenDescriptor!.SigningCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public JwtTokenHandlerManager AddHandler
                                    (
                                        string name
                                        , Action<SecurityTokenDescriptor, TokenValidationParameters>
                                            onCustomProcessAction
                                    )
    {
        var tokenManagerEntry = new TokenHandlerEntry(name);
        this
            ._handlers
                    .Add
                        (
                            name
                            , tokenManagerEntry
                        );

        //tokenManagerEntry
        //        .DefaultTokenValidationParameters!
        //        .IssuerSigningKey
        //            =
        //                tokenManagerEntry
        //                    .DefaultTokenDescriptor!
        //                    .SigningCredentials!
        //                    .Key;

        onCustomProcessAction
                    (
                        tokenManagerEntry
                                .DefaultTokenDescriptor!
                        , tokenManagerEntry
                                .DefaultTokenValidationParameters!
                    );

        return this;
    }

    public TokenHandlerEntry this[string issuerName]
    {
        get
        {
            return this._handlers[issuerName];
        }
    }
}
