using Documents.Domain.Entities;
using Documents.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<DocumentsContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            var documentTemplate = await context.DocumentTemplates.FirstOrDefaultAsync(dt => dt.Id == "greeting");

            if (documentTemplate is null)
            {
                documentTemplate = new DocumentTemplate()
                {
                    Id = "greeting",
                    Name = "Greeting",
                    TemplateLanguage = DocumentTemplateLanguage.Razor,
                    Content =
@$"
@model dynamic
Hello, @Model.Name!"
                };

                context.DocumentTemplates.Add(documentTemplate);

                documentTemplate = new DocumentTemplate()
                {
                    Id = "letter",
                    Name = "Letter",
                    TemplateLanguage = DocumentTemplateLanguage.Razor,
                    Content =
@$"
@model dynamic

<style>
    body {{
        font-family: 'Helvetica', 'Arial', sans-serif;
    }}
</style>

<p>Hi, @Model.Name!</p>

<p>We would love to hear your feedback on the quality of our services.</p>"
                };

                context.DocumentTemplates.Add(documentTemplate);

                documentTemplate = new DocumentTemplate()
                {
                    Id = "invoice",
                    Name = "Invoice",
                    TemplateLanguage = DocumentTemplateLanguage.Razor,
                    Content =
@$"
@model dynamic

<style>
    body {{
        font-family: 'Helvetica', 'Arial', sans-serif;
    }}
</style>

<h1>Invoice @Model.Id</h1>

<p>@Model.Status</p>

<table>
    <thead>
        <tr>
            <th>Description</th>
            <th>Unit Price</th>
            <th>Quantity</th>
            <th>VAT Rate (%)</th>
            <th>Total</th>
        </tr>
    </thead>
    <tbody>
    @foreach(var item in Model.Items) 
    {{
        <tr>
            <td>@item.Description</td>
            <td>@item.UnitPrice.ToString(""c"")</td>
            <td>@item.Quantity</td>
            <td>@(item.VatRate * 100)</td>
            <td>@item.LineTotal.ToString(""c"")</td>
        </tr>
    }}
    </tbody>
</table>

<p>Sub Total: @Model.SubTotal.ToString(""c"")</p>
<p>VAT: @Model.Vat.ToString(""c"")</p>
<p>Total: @Model.Total.ToString(""c"")</p>"
                };

                context.DocumentTemplates.Add(documentTemplate);

                documentTemplate = new DocumentTemplate()
                {
                    Id = "reminder",
                    Name = "Reminder",
                    TemplateLanguage = DocumentTemplateLanguage.Razor,
                    Content =
@$"
@model dynamic

<style>
    body {{
        font-family: 'Helvetica', 'Arial', sans-serif;
    }}
</style>

<p>Hi, @Model.Name!</p>

<p>You have not yet paid the invoice.</p>

<p>Amount to pay: @Model.AmountToPay.ToString(""c"")</p>"
                };

                context.DocumentTemplates.Add(documentTemplate);

                                documentTemplate = new DocumentTemplate()
                {
                    Id = "reminder2",
                    Name = "Reminder 2",
                    TemplateLanguage = DocumentTemplateLanguage.Razor,
                    Content =
@$"
@model dynamic

<style>
    body {{
        font-family: 'Helvetica', 'Arial', sans-serif;
    }}
</style>

<p>Hi, @Model.Name!</p>

<p>You have outstanding payments.</p>

<p>Amount to pay: @Model.RemainingAmount.ToString(""c"")</p>"
                };

                context.DocumentTemplates.Add(documentTemplate);


                await context.SaveChangesAsync();
            }
        }
    }
}