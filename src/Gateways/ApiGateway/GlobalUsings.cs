global using ApiGateway.Configuration;
global using ApiGateway.Endpoints;
global using ApiGateway.Middleware;

global using ServiceDefaults.OpenApi;
global using ServiceDefaults.Middleware;
global using ServiceDefaults.Observability;
global using ServiceDefaults.Authentication;
global using ServiceDefaults.Authorization;

global using System.Threading.RateLimiting;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.HttpOverrides;

global using Scalar.AspNetCore;

global using Serilog;