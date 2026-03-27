global using SharedKernel.Abstractions;
global using SharedKernel.Domain;
global using SharedKernel.Idempotency;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Campaign.Domain.AggregatesModel.CampaignAggregate.Enums;
global using Campaign.Domain.AggregatesModel.CampaignAggregate.Entities;
global using Campaign.Domain.AggregatesModel.CampaignAggregate.Repositories;
global using CampaignEntity = Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign;

global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Entities;
global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Repositories;
global using Campaign.Domain.AggregatesModel.DonationIntentAggregate.Enums;

global using Campaign.Infrastructure.Repositories;

global using Mediator;
global using Mediator.Notifications;
global using ServiceDefaults.Contracts;
