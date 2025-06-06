using Core.Models.Clients;
using Core.Models.Pricing;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infra.Data
{
    public class ApplicationContextSeed
    {
        public static async Task SeedAsync ( ApplicationContext context, ILogger logger )
        {
            try
            {
                // Seeds que não dependem de outros
                await SeedWorkSections ( context, logger );
                await SeedWorkTypes ( context, logger );
                await SeedSectors ( context, logger );
                await SeedScales ( context, logger );
                await SeedShades ( context, logger );
                await SeedTablePrices ( context, logger );

                // Seeds que dependem dos anteriores
                await SeedClients ( context, logger );
             


                logger.LogInformation ( "Database seeded successfully" );
            }
            catch ( Exception ex )
            {
                logger.LogError ( ex, "An error occurred while seeding the database" );
                throw;
            }
        }

        private static async Task SeedWorkSections ( ApplicationContext context, ILogger logger )
        {
            if ( !context.WorkSections.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/worksections.json");
                var sections = JsonSerializer.Deserialize<List<WorkSection>>(data);
                if ( sections != null )
                {
                    await context.WorkSections.AddRangeAsync ( sections );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Work Sections seeded successfully" );
                }
            }
        }

        private static async Task SeedWorkTypes ( ApplicationContext context, ILogger logger )
        {
            if ( !context.WorkTypes.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/worktypes.json");
                var types = JsonSerializer.Deserialize<List<WorkType>>(data);
                if ( types != null )
                {
                    await context.WorkTypes.AddRangeAsync ( types );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Work Types seeded successfully" );
                }
            }
        }

        private static async Task SeedSectors ( ApplicationContext context, ILogger logger )
        {
            if ( !context.Sectors.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/sectors.json");
                var sectors = JsonSerializer.Deserialize<List<Sector>>(data);
                if ( sectors != null )
                {
                    await context.Sectors.AddRangeAsync ( sectors );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Sectors seeded successfully" );
                }
            }
        }

        private static async Task SeedScales ( ApplicationContext context, ILogger logger )
        {
            if ( !context.Scales.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/scales.json");
                var scales = JsonSerializer.Deserialize<List<Scale>>(data);
                if ( scales != null )
                {
                    await context.Scales.AddRangeAsync ( scales );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Scales seeded successfully" );
                }
            }
        }

        private static async Task SeedShades ( ApplicationContext context, ILogger logger )
        {
            if ( !context.Shades.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/shades.json");
                var shades = JsonSerializer.Deserialize<List<Shade>>(data);
                if ( shades != null )
                {
                    await context.Shades.AddRangeAsync ( shades );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Shades seeded successfully" );
                }
            }
        }

        private static async Task SeedTablePrices ( ApplicationContext context, ILogger logger )
        {
            if ( !context.TablePrices.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/tableprices.json");
                var prices = JsonSerializer.Deserialize<List<TablePrice>>(data);
                if ( prices != null )
                {
                    await context.TablePrices.AddRangeAsync ( prices );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Table Prices seeded successfully" );
                }
            }
        }

        private static async Task SeedClients ( ApplicationContext context, ILogger logger )
        {
            if ( !context.Clients.Any ( ) )
            {
                var data = File.ReadAllText("../Infra/Data/SeedData/clients.json");
                var clients = JsonSerializer.Deserialize<List<Client>>(data);
                if ( clients != null )
                {
                    await context.Clients.AddRangeAsync ( clients );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Clients seeded successfully" );
                }
            }
        }





    }
}