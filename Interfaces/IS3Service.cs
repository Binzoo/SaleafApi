using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Interfaces
{
    public interface IS3Service
    {
        Task UploadFileAsync(Stream fileStream, string fileName);
        Task<Stream> DownloadFileAsync(string fileName);
    }
}