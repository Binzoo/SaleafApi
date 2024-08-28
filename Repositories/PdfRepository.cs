using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SaleafApi.Interfaces;

namespace SaleafApi.Repositories
{
    public class PdfRepository : IPdf
    {
        public MemoryStream GetPdf()
        {
            string sourceFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SALEAF - Section18A Sample.pdf");

            if (!System.IO.File.Exists(sourceFile))
            {
                return null; ;
            }

            var memoryStream = new MemoryStream();

            using (var document = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Modify))
            {
                PdfPage page = document.Pages[0];
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var font = new XFont("Arial", 10, XFontStyle.Regular);

                    gfx.DrawString("Ref No!", font, XBrushes.Black, new XRect(384, 228, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Date of Donation", font, XBrushes.Black, new XRect(147, 323, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Name of Donor!", font, XBrushes.Black, new XRect(100, 385, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Identity Number!", font, XBrushes.Black, new XRect(100, 400, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Income Tax Number!", font, XBrushes.Black, new XRect(100, 412, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Cell Phone Number!", font, XBrushes.Black, new XRect(66, 450, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Email!", font, XBrushes.Black, new XRect(67, 465, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Amount!", font, XBrushes.Black, new XRect(354, 450, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString("Signed By Date!", font, XBrushes.Black, new XRect(443, 668, page.Width, page.Height), XStringFormats.TopLeft);
                }
                document.Save(memoryStream, false);
            }
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
