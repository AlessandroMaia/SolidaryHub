global using FluentAssertions;
global using FluentValidation;
global using Identity.API.Application.Commands.ChangePassword;
global using Identity.API.Application.Commands.RefreshToken;
global using Identity.API.Application.Commands.RegisterUser;
global using Identity.API.Application.Commands.SignIn;
global using Identity.API.Application.Queries.GetAllUsers;
global using Identity.API.Application.Queries.GetUserById;

global using Identity.Domain.AggregatesModel.UsersAggregate.Entities;
global using Identity.Domain.AggregatesModel.UsersAggregate.Enums;
global using Identity.Domain.AggregatesModel.UsersAggregate.Repositories;
global using Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;
global using Identity.Domain.Exceptions;
global using Identity.Domain.Services;

global using Identity.Infrastructure;
global using Identity.Infrastructure.Repositories;
global using Identity.Infrastructure.Services;

global using Mediator.Commands;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi;

global using NSubstitute;

global using ServiceDefaults.Authentication;
global using ServiceDefaults.Authorization;
global using ServiceDefaults.Behaviors;
global using ServiceDefaults.Contracts;
global using ServiceDefaults.Middleware;
global using ServiceDefaults.OpenApi;
global using ServiceDefaults.Web;

global using SharedKernel.Abstractions;
global using SharedKernel.Domain;
global using SharedKernel.Idempotency;
global using SharedKernel.Pagination;
global using SharedKernel.Results;
global using SharedKernel.Security;

global using System.Security.Claims;

global using Xunit;
