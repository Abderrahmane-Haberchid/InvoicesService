using Application.Abstractions;

namespace Infrastructure.Pdf;

public class PdfGenerator : IPdfGenerator
{
    public void GeneratePdf()
    {
        Task.Delay(TimeSpan.FromSeconds(40)).Wait();
    }
}