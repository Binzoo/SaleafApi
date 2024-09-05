using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SaleafApi.Interfaces;
using SeleafAPI.Controllers;

namespace SaleafApi.Repositories
{
    public class PdfRepository : IPdf
    {
        public MemoryStream GetPdf(AllDonorCertificateInfo allDonorCertificateInfo)
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
                    gfx.DrawString($"{allDonorCertificateInfo.RefNo}", font, XBrushes.Black, new XRect(384, 228, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.DateofReceiptofDonation}", font, XBrushes.Black, new XRect(147, 323, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.FirstName} {allDonorCertificateInfo.LastName}", font, XBrushes.Black, new XRect(100, 385, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.IdentityNoOrCompanyRegNo}", font, XBrushes.Black, new XRect(100, 400, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.IncomeTaxNumber}", font, XBrushes.Black, new XRect(100, 412, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.PhoneNumber}", font, XBrushes.Black, new XRect(66, 450, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.Email}", font, XBrushes.Black, new XRect(67, 465, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.Amount}", font, XBrushes.Black, new XRect(354, 450, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.DateofReceiptofDonation}", font, XBrushes.Black, new XRect(443, 668, page.Width, page.Height), XStringFormats.TopLeft);
                }
                document.Save(memoryStream, false);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
