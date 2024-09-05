using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Controllers;

namespace SaleafApi.Interfaces
{
    public interface IPdf
    {
        MemoryStream GetPdf(AllDonorCertificateInfo allDonorCertificateInfo);
    }
}