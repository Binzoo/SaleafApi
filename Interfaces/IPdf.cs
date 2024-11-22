using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Controllers;
using SeleafAPI.Data;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SaleafApi.Interfaces
{
    public interface IPdf
    {
        MemoryStream GetPdf(AllDonorCertificateInfo allDonorCertificateInfo);
        MemoryStream GenerateEventPdfWithQrCode(EventRegistration registrationInfo, byte[] qrCodeBytes);

    }
}