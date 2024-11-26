using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SaleafApi.Interfaces;
using SeleafAPI.Controllers;
using SeleafAPI.Data;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

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
            string numberAmountInWords = NumberToWords(allDonorCertificateInfo.Amount);
            using (var document = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Modify))
            {
                PdfPage page = document.Pages[0];
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var font = new XFont("Arial", 10, XFontStyle.Regular);
                    gfx.DrawString($"{allDonorCertificateInfo.RefNo}", font, XBrushes.Black, new XRect(384, 228, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.DateofReceiptofDonation}", font, XBrushes.Black, new XRect(145, 323, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.FirstName} {allDonorCertificateInfo.LastName}", font, XBrushes.Black, new XRect(100, 385, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.IdentityNoOrCompanyRegNo}", font, XBrushes.Black, new XRect(100, 400, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.IncomeTaxNumber}", font, XBrushes.Black, new XRect(100, 412, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.Address}", font, XBrushes.Black, new XRect(66, 465, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.PhoneNumber}", font, XBrushes.Black, new XRect(66, 480, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.Email}", font, XBrushes.Black, new XRect(67, 495, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.Amount} ({numberAmountInWords})", font, XBrushes.Black, new XRect(352, 450, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{allDonorCertificateInfo.DateofReceiptofDonation}", font, XBrushes.Black, new XRect(443, 668, page.Width, page.Height), XStringFormats.TopLeft);
                }
                document.Save(memoryStream, false);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        public MemoryStream GenerateEventPdfWithQrCode(EventRegistration registrationInfo, byte[] qrCodeBytes)
        {
            // Path to the template PDF file
            string sourceFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventReg.pdf");

            if (!System.IO.File.Exists(sourceFile))
            {
                throw new FileNotFoundException("PDF template not found.");
            }

            var memoryStream = new MemoryStream();

            using (var document = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Modify))
            {
                PdfPage page = document.Pages[0];

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var font = new XFont("Arial", 10, XFontStyle.Regular);

                    // Add registration information
                    gfx.DrawString($"First Name: {registrationInfo.FirstName}", font, XBrushes.Black, new XRect(100, 150, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"Last Name: {registrationInfo.LastName}", font, XBrushes.Black, new XRect(100, 170, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"Phone Number: {registrationInfo.PhoneNumber}", font, XBrushes.Black, new XRect(100, 190, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"Number of Participants: {registrationInfo.NumberOfParticipant}", font, XBrushes.Black, new XRect(100, 210, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"Amount: {registrationInfo.Amount}", font, XBrushes.Black, new XRect(100, 230, page.Width, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"Currency: {registrationInfo.Currency}", font, XBrushes.Black, new XRect(100, 250, page.Width, page.Height), XStringFormats.TopLeft);

                        // Embed the QR code in the PDF
                    if (qrCodeBytes != null)
                    {
                        Func<Stream> qrStreamFunc = () => new MemoryStream(qrCodeBytes);
                        var qrImage = XImage.FromStream(qrStreamFunc);
                        gfx.DrawImage(qrImage, new XRect(400, 150, 150, 150)); // Adjust position and size as needed
                    }
                }

                document.Save(memoryStream, false);
            }

            memoryStream.Position = 0; // Reset stream position to the beginning
            return memoryStream;
        }





        private string NumberToWords(double number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            long intPart = (long)Math.Floor(number);
            double fractionalPart = number - intPart;

            string words = NumberToWordsInt(intPart);

            if (fractionalPart > 0)
            {
                fractionalPart = Math.Round(fractionalPart * 100);  // Considering two decimal places
                words += " and " + NumberToWordsInt((long)fractionalPart) + " Hundredths";
            }

            return words;
        }

        private string NumberToWordsInt(long number)
        {
            if (number == 0)
                return "Zero";

            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += NumberToWordsInt(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                words += NumberToWordsInt(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWordsInt(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWordsInt(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words.Trim();
        }
    }
}
