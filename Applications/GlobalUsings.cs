// ==========================
// AutoMapper
// ==========================
global using AutoMapper;


// ==========================
// DTOs, Records and Projections
// ==========================
global using Applications.Contracts;
global using Applications.Dtos.ServiceOrder;
global using Applications.Dtos.Billing;
global using Applications.Dtos.Identity;
global using Applications.Dtos.Clients;
global using Applications.Dtos.Schedule;
global using Applications.Dtos.Sector;
global using Applications.Dtos.Production;
global using Applications.Dtos.Settings;
global using Applications.Dtos.Pricing;
global using Applications.Dtos.Payments;
global using Applications.Dtos.Work;

global using Applications.Records.Billing;
global using Applications.Records.Clients;
global using Applications.Records.Schedule;
global using Applications.Records.Settings;
global using Applications.Records.ServiceOrders;
global using Applications.Records.Payments;
global using Applications.Records.Pricing;
global using Applications.Records.Production;
global using Applications.Records.Sector;
global using Applications.Records.Work;

global using Applications.Projections.ServiceOrder;
global using Applications.Projections.Billing;
global using Applications.Projections.Clients;
global using Applications.Projections.Pricing;

global using Applications.Responses;


// ==========================
// Application Services & Identity
// ==========================
global using Applications.Identity;
global using Applications.Services.Validators;


// ==========================
// Core Domain: Models, Enums, Exceptions, Params
// ==========================
global using Core.Enums;
global using Core.Exceptions;
global using Core.Params;

global using Core.Models.Billing;
global using Core.Models.Payments;
global using Core.Models.Clients;
global using Core.Models.Schedule;
global using Core.Models.ServiceOrders;
global using Core.Models.Works;
global using Core.Models.Production;
global using Core.Models.LabSettings;
global using Core.Models.Pricing;


// ==========================
// Core Interfaces & Specifications
// ==========================
global using Core.Interfaces;
global using Core.Specifications;


// ==========================
// Specifications (Factory Pattern)
// ==========================
global using Core.FactorySpecifications;
global using Core.FactorySpecifications.BillingSpecifications;
global using Core.FactorySpecifications.PaymentSpecifications;
global using Core.FactorySpecifications.ServiceOrderSpecifications;

global using static Core.FactorySpecifications.BillingSpecifications.BillingInvoiceSpecification;
global using static Core.FactorySpecifications.PaymentSpecifications.PaymentSpecification;
global using static Core.FactorySpecifications.ClientsSpecifications.ClientSpecification;
global using static Core.FactorySpecifications.ScheduleSpecification;
global using static Core.FactorySpecifications.SectorSpecifications.SectorSpecification;
global using static Core.FactorySpecifications.ServiceOrderSpecifications.ServiceOrderSpecification;

global using Applications.Projections.ClientArea;
global using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaBillingInvoiceSpecification;
// ==========================
// Mapping Utilities
// ==========================
global using Applications.Mapping.Resolvers;


// ==========================
// QuestPDF
// ==========================
global using QuestPDF.Fluent;
global using QuestPDF.Helpers;
global using QuestPDF.Infrastructure;


// ==========================
// System & Utility Namespaces
// ==========================
global using System.ComponentModel.DataAnnotations;
global using System.Globalization;
global using System.Linq.Expressions;
global using Microsoft.Extensions.Logging;
