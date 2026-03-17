global using System.Reflection;

global using ServiceDefaults;
global using SharedKernel;

global using Mediator;
global using Mediator.Commands;
global using Mediator.Queries;
global using Mediator.Behaviors;
global using Mediator.Extensions;

global using FluentValidation;

global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Identity.Domain.AggregatesModel.UsersAggregate.Repositories;
global using Identity.Domain.AggregatesModel.UsersAggregate.ValueObjects;
global using Identity.Domain.AggregatesModel.UsersAggregate.Enums;
global using Identity.Domain.AggregatesModel.UsersAggregate.Entities;
global using Identity.Domain.Exceptions;
global using Identity.Domain.Services;
global using Identity.Infrastructure;
global using Identity.API.Application.Behaviors;
global using Identity.API.Application.ViewModels;
global using Identity.API.Extensions;
global using Identity.API.Middleware;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.OpenApi;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.OpenApi;

global using Scalar.AspNetCore;