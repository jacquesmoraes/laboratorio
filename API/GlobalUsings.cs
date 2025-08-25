// ==========================
// ASP.NET Core Core Services
// ==========================
global using API.Extensions;
// ==========================
// Middleware, Filters & Extensions
// ==========================
global using API.Filters;
global using API.Middleware;
// ==========================
// API Models & Services
// ==========================
global using API.Models;
global using API.Services;
// ==========================
// Application Layer
// ==========================
global using Applications.Contracts;
// ==========================
// Authentication & Identity
// ==========================
global using Applications.Contracts.Identity;

// ===========================
//  sendgrid email service
// ===========================

global using SendGrid;
global using SendGrid.Helpers.Mail;
// ==========================
// DTOs
// ==========================
global using Applications.Dtos.Billing;
global using Applications.Dtos.Clients;
global using Applications.Dtos.Identity;
global using Applications.Dtos.Payments;
global using Applications.Dtos.Pricing;
global using Applications.Dtos.Production;
global using Applications.Dtos.Schedule;
global using Applications.Dtos.Sector;
global using Applications.Dtos.ServiceOrder;
global using Applications.Dtos.Settings;
global using Applications.Dtos.Work;
global using Applications.Identity;
global using Applications.Mapping;
// ==========================
// PDF Generation (QuestPDF)
// ==========================
global using Applications.PdfDocuments;

// ==========================
//  website 
// ==========================
global using Applications.Contracts.WebSiteServices;
global using Applications.Services.ClientAreaServices;
global using Applications.Services.WebSiteServices;
// ==========================
// Projections
// ==========================
global using Applications.Projections.Billing;
global using Applications.Projections.Pricing;
global using Applications.Projections.ServiceOrder;
// ==========================
// Records
// ==========================
global using Applications.Records.Clients;
global using Applications.Records.Payments;
global using Applications.Records.Pricing;
global using Applications.Records.Production;
global using Applications.Records.Schedule;
global using Applications.Records.Sector;
global using Applications.Records.ServiceOrders;
global using Applications.Records.Settings;
global using Applications.Records.Work;
// ==========================
// API Responses
// ==========================
global using Applications.Responses;
global using Applications.Services;
// ==========================
// AutoMapper & Mapping
// ==========================
global using AutoMapper;
global using Core.Exceptions;
// ==========================
// Factory Specifications
// ==========================
global using Core.FactorySpecifications;
global using Core.FactorySpecifications.BillingSpecifications;
global using Core.FactorySpecifications.PaymentSpecifications;
global using Core.FactorySpecifications.ProductionSpecifications;
global using Core.FactorySpecifications.SectorSpecifications;
global using Core.FactorySpecifications.ServiceOrderSpecifications;
// ==========================
// Core Layer - Specifications, Params, Enums, Exceptions
// ==========================
global using Core.Interfaces;
// ==========================
// Domain Models (Core)
// ==========================
global using Core.Models.Clients;
global using Core.Models.Payments;
global using Core.Models.Pricing;
global using Core.Models.Production;
global using Core.Models.Schedule;
global using Core.Models.ServiceOrders;
global using Core.Models.Works;
global using Core.Params;
global using Core.Specifications;
// ==========================
// Infrastructure
// ==========================
global using Infra.Data;
global using Infra.Data.Repositories;
global using Infra.Identity;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using QuestPDF.Fluent;
global using QuestPDF.Infrastructure;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
// ==========================
// System & Utility Namespaces
// ==========================
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using static Core.FactorySpecifications.ClientsSpecifications.ClientSpecification;
