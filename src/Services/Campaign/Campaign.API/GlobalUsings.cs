global using Campaign.API.Application.ViewModels.Campaigns;
global using Campaign.API.Application.ViewModels.DonationIntents;
global using Campaign.API.Application.IntegrationEvents;
global using Campaign.API.Application.IntegrationEvents.Events;

global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Entities;
global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Repositories;
global using Campaign.Domain.AggregatesModel.CampaignAggregate.Repositories;
global using Campaign.Domain.AggregatesModel.CampaignAggregate.Enums;
global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Enums;
global using Campaign.Domain.Exceptions;
global using Campaign.Domain.Events;

global using Campaign.Infrastructure;

global using Mediator;
global using Mediator.Notifications;
global using Mediator.Behaviors;
global using Mediator.Commands;
global using Mediator.Queries;
global using Mediator.Extensions;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Diagnostics.HealthChecks;

global using EventBus.Events;
global using EventBus.Abstractions;
global using EventBus.Extensions;

global using FluentValidation;
global using Scalar.AspNetCore;
global using System.Reflection;

global using ServiceDefaults.Authentication;
global using ServiceDefaults.Authorization;
global using ServiceDefaults.Behaviors;
global using ServiceDefaults.Contracts;
global using ServiceDefaults.Middleware;
global using ServiceDefaults.OpenApi;
global using ServiceDefaults.Web;

global using SharedKernel.Pagination;
global using SharedKernel.Security;
