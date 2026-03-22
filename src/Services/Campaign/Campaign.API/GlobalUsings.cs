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

global using FluentValidation;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Diagnostics.HealthChecks;

global using EventBus.Abstractions;
global using EventBus.Events;

global using Scalar.AspNetCore;

global using ServiceDefaults;

global using System.Reflection;

global using EventBus.Extensions;

global using SharedKernel;
