global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Migrations;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Authentication.JwtBearer;

global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Microsoft.IdentityModel.Tokens;

global using Identity.Domain.Services;
global using Identity.Domain.AggregatesModel.UsersAggregate.Entities;
global using Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;
global using Identity.Domain.AggregatesModel.UsersAggregate.Repositories;
global using Identity.Domain.AggregatesModel.UsersAggregate.Enums;

global using Identity.Infrastructure.Repositories;
global using Identity.Infrastructure.Services;

global using System.Security.Cryptography;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;

global using SharedKernel;