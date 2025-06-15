using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Applications.Records.Billing;
using Applications.Records.Settings;
using System.Globalization;

namespace Applications.PdfDocuments
{
    public class BillingInvoicePdfDocument (
        BillingInvoiceRecord invoice,
        SystemSettingsRecord settings,
        string? logoPath ) : IDocument
    {
        private readonly BillingInvoiceRecord _invoice = invoice;
        private readonly string? _logoPath = logoPath;
        private readonly SystemSettingsRecord _settings = settings;

        public DocumentMetadata GetMetadata ( ) => DocumentMetadata.Default;

        public void Compose ( IDocumentContainer container )
        {
            container.Page ( page =>
            {
                page.Margin ( 40 );
                page.DefaultTextStyle ( x => x.FontFamily ( "Segoe UI" ).FontSize ( 12 ).FontColor ( "#2c3e50" ) );

                page.Header ( ).Row ( row =>
                {
                    row.RelativeItem ( ).Column ( col =>
                    {
                        if ( !string.IsNullOrEmpty ( _logoPath ) && File.Exists ( _logoPath ) )
                        {
                            col.Item ( ).Width ( 120 ).Height ( 80 )
                            .Image ( _logoPath ).FitArea ( );
                        }
                        else
                        {
                            col.Item ( ).Height ( 80 ).AlignCenter ( ).AlignMiddle ( )
                            .Text ( "[LOGO]" ).Italic ( ).FontColor ( Colors.Grey.Lighten1 );
                        }
                        col.Item ( ).Text ( "FATURA" ).FontSize ( 16 ).FontColor ( "#3498db" ).Bold ( );
                        col.Item ( ).Text ( _invoice.InvoiceNumber ).FontSize ( 10 ).FontColor ( "#7f8c8d" );

                    } );


                    row.ConstantItem ( 200 ).Column ( col =>
                    {
                        col.Item ( ).AlignRight ( ).PaddingTop ( 5 ).Text ( _settings.LabName ).Bold ( ).FontSize ( 14 );
                        col.Item ( ).AlignRight ( ).Text ( $"CNPJ: {_settings.CNPJ}" );
                        col.Item ( ).AlignRight ( ).Text ( $"fone: {_settings.Phone}" );
                        col.Item ( ).AlignRight ( ).Text ( $"{_settings.Address.Street}, {_settings.Address.Number}/{_settings.Address.Complement} - {_settings.Address.Neighborhood}" );
                        col.Item ( ).AlignRight ( ).Text ( $"{_settings.Address.City} - {_settings.Address.Cep}" );
                    } );
                } );

                page.Content ( ).Column ( col =>
                {
                    col.Spacing ( 20 );

                   col.Item().Background("#f8f9fa").Padding(20).Column(client =>
{
    var addr = _invoice.Client.Address;
    var addressFormatted = $"{addr.Street}, {addr.Number}" +
        (!string.IsNullOrWhiteSpace(addr.Complement) ? $" - {addr.Complement}" : "") +
        $", {addr.Neighborhood}, {addr.City}";

    client.Item().AlignCenter().Text($"Cliente: {_invoice.Client.ClientName}").Bold();
    client.Item().AlignCenter().Text($"Endereço: {addressFormatted}");
    client.Item().AlignCenter().Text($"Telefone: {_invoice.Client.PhoneNumber}");
    client.Item().AlignCenter().Text($"Descrição: {_invoice.Description}");
    client.Item().AlignCenter().Text($"Data de Emissão: {_invoice.CreatedAt:dd/MM/yyyy}");
});


                    col.Item ( ).Table ( table =>
                    {
                        table.ColumnsDefinition ( columns =>
                        {

                            columns.ConstantColumn ( 60 );   // Nº OS
                            columns.RelativeColumn ( 2 );    // Descrição
                            columns.ConstantColumn ( 100 );   // Quant/Preço
                            columns.ConstantColumn ( 100 );   // Subtotal
                        } );

                        table.Header ( header =>
                        {
                            string[] headers = {
                                 "Nº OS", "Serviços", "Quant/Preço", "Subtotal"
                            };

                            foreach ( var h in headers )
                                header.Cell ( ).Background ( "#3498db" ).Padding ( 8 ).Text ( h ).FontColor ( "#ffffff" ).Bold ( );
                        } );

                        foreach ( var order in _invoice.ServiceOrders )
                        {
                            // Cabeçalho da ordem de serviço
                            table.Cell ( ).ColumnSpan ( 4 ).Background ( "#e8f4f8" ).Padding ( 5 ).Row ( row =>
                            {
                                row.RelativeItem ( ).Text ( $"Paciente: {order.PatientName}" ).Bold ( );
                                row.ConstantItem ( 100 ).Text ( $"Entrada: {order.DateIn:dd/MM/yyyy}" );
                                row.ConstantItem ( 100 ).Text ( $"Entrega: {order.FinishedAt:dd/MM/yyyy}" );
                            } );

                            // Itens da ordem
                            foreach ( var work in order.Works )
                            {

                                table.Cell ( ).BorderBottom ( 0.5f ).BorderColor ( Colors.Grey.Lighten2 ).Padding ( 5 ).Text ( order.OrderCode );
                                table.Cell ( ).BorderBottom ( 0.5f ).BorderColor ( Colors.Grey.Lighten2 ).Padding ( 5 ).Text ( work.WorkTypeName );
                                table.Cell ( ).BorderBottom ( 0.5f ).BorderColor ( Colors.Grey.Lighten2 ).Padding ( 5 ).Text ( $"{work.Quantity} x {work.PriceUnit.ToString ( "C", CultureInfo.GetCultureInfo ( "pt-BR" ) )}" );
                                table.Cell ( ).BorderBottom ( 0.5f ).BorderColor ( Colors.Grey.Lighten2 ).Padding ( 5 ).Text ( work.PriceTotal.ToString ( "C", CultureInfo.GetCultureInfo ( "pt-BR" ) ) );
                            }

                            // Subtotal da ordem
                            table.Cell ( ).ColumnSpan ( 4 ).Background ( "#f1f1f1" ).Padding ( 5 ).AlignRight ( )
                                .Text ( $"Subtotal: {order.Subtotal.ToString ( "C", CultureInfo.GetCultureInfo ( "pt-BR" ) )}" )
                                .Italic ( );
                            table.Cell ( ).ColumnSpan ( 4 ).Height ( 10 );
                        }
                    } );

                    col.Item ( ).Background ( "#f8f9fa" ).Padding ( 20 ).Column ( totals =>
                    {
                        totals.Spacing ( 8 );

                        void AddTotalRow ( string label, decimal value, bool bold = false )
                        {
                            totals.Item ( ).Row ( row =>
                            {
                                row.RelativeItem ( ).Text ( label + ":" );
                                var text = row.ConstantItem(120).AlignRight().Text(value.ToString("C", CultureInfo.GetCultureInfo("pt-BR")));
                                if ( bold ) text.Bold ( );
                            } );
                        }

                        AddTotalRow ( "Total de Serviços", _invoice.TotalServiceOrdersAmount );
                        AddTotalRow ( "Crédito Anterior", _invoice.PreviousCredit );
                        AddTotalRow ( "Débito Anterior", _invoice.PreviousDebit );
                        AddTotalRow ( "Valor Total da Fatura", _invoice.TotalInvoiceAmount, bold: true );
                    } );

                    col.Item ( ).AlignCenter ( ).PaddingTop ( 40 ).Text ( text =>
                    {
                        text.Span ( _settings.FooterMessage ?? "Gerado por Sistema Laboratório" ).FontSize ( 10 ).FontColor ( "#7f8c8d" );
                        text.Line ( "" );
                        text.Span ( $"Documento gerado em {DateTime.UtcNow:dd/MM/yyyy}" ).FontSize ( 10 ).FontColor ( "#7f8c8d" );
                    } );
                } );
            } );
        }
    }
}