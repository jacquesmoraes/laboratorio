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
                await SeedWorkSections ( context, logger );
                await SeedWorkTypes ( context, logger );
                await SeedSectors ( context, logger );
                await SeedScales ( context, logger );
                await SeedShades ( context, logger );
                await SeedTablePriceItems ( context, logger );
                await SeedTablePrices ( context, logger );
                await SeedSystemSettings ( context, logger );
                await SeedClients ( context, logger );

                logger.LogInformation ( "Database seeded successfully" );
            }
            catch ( Exception ex )
            {
                logger.LogError ( ex, "An error occurred while seeding the database" );
                throw;
            }
        }

        // 🔁 Helper para centralizar leitura dos arquivos JSON
        private static string ReadSeedFile ( string fileName )
        {
            var basePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");
            var path = Path.Combine(basePath, fileName);
            return File.ReadAllText ( path );
        }


        private static async Task SeedWorkSections ( ApplicationContext context, ILogger logger )
        {
            if ( !context.WorkSections.Any ( ) )
            {
                var data = ReadSeedFile("worksections.json");
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
                var data = ReadSeedFile("worktypes.json");
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
                var data = ReadSeedFile("sectors.json");
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
                var data = ReadSeedFile("scales.json");
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
                var data = ReadSeedFile("shades.json");
                var shades = JsonSerializer.Deserialize<List<Shade>>(data);
                if ( shades != null )
                {
                    await context.Shades.AddRangeAsync ( shades );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Shades seeded successfully" );
                }
            }
        }

        private static async Task SeedTablePriceItems ( ApplicationContext context, ILogger logger )
        {
            if ( !context.TablePriceItems.Any ( ) )
            {
                var data = ReadSeedFile("tablepriceitems.json");

                var items = JsonSerializer.Deserialize<List<TablePriceItem>>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if ( items != null )
                {
                    await context.TablePriceItems.AddRangeAsync ( items );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "TablePriceItems seeded successfully" );
                }
            }
        }

        private static async Task SeedTablePrices ( ApplicationContext context, ILogger logger )
        {
            if ( !context.TablePrices.Any ( ) )
            {
                var data = ReadSeedFile("tableprices.json");

                var rawTables = JsonSerializer.Deserialize<List<RawTablePriceDto>>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if ( rawTables != null )
                {
                    foreach ( var raw in rawTables )
                    {
                        var table = new TablePrice
                        {
                            Name = raw.Name,
                            Description = raw.Description,
                            Status = raw.Status,
                            Items = context.TablePriceItems
                                .Where(i => raw.Items.Select(x => x.TablePriceItemId).Contains(i.TablePriceItemId))
                                .ToList()
                        };

                        await context.TablePrices.AddAsync ( table );
                    }

                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "TablePrices seeded successfully with associated items." );
                }
            }
        }

        private static async Task SeedClients ( ApplicationContext context, ILogger logger )
        {
            if ( !context.Clients.Any ( ) )
            {
                var data = ReadSeedFile("clients.json");
                var clients = JsonSerializer.Deserialize<List<Client>>(data);
                if ( clients != null )
                {
                    await context.Clients.AddRangeAsync ( clients );
                    await context.SaveChangesAsync ( );
                    logger.LogInformation ( "Clients seeded successfully" );
                }
            }
        }

        private static async Task SeedSystemSettings ( ApplicationContext context, ILogger logger )
        {
            if ( !context.SystemSettings.Any ( ) )
            {
                var settings = new SystemSettings
                {
                    Id = 1,
                    LabName = "Laboratório Modelo",
                    Email = "contato@labmodelo.com",
                    Phone = "(53)99999-0000",
                    CNPJ = "00.000.000/0001-00",
                    FooterMessage = "Obrigado por confiar no nosso laboratório.",
                    LogoFileName = null,
                    LastUpdated = DateTime.UtcNow,
                    Address = new LabAddress
                    {
                        Street = "Rua Exemplo",
                        Number = 123,
                        Complement = "Sala 2",
                        Neighborhood = "Centro",
                        City = "Pelotas",
                        Cep = "96000-000"
                    }
                };

                await context.SystemSettings.AddAsync ( settings );
                await context.SaveChangesAsync ( );
                logger.LogInformation ( "SystemSettings seeded successfully" );
            }
        }

        private record RawTablePriceDto
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool Status { get; set; }
            public List<RawTablePriceItemRef> Items { get; set; } = [];
        }

        private record RawTablePriceItemRef
        {
            public int TablePriceItemId { get; set; }
        }
    }
}
