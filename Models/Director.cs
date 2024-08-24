using System;
using Org.BouncyCastle.Asn1.Cmp;

namespace SeleafAPI.Models;

public class Director
{
    public int DirectorId { get; set; }
    public string? DirectorName { get; set; }
    public string? DirectorLastName { get; set; }
    public string? DirectorImage { get; set; }
    public string? DirectorDescription { get; set; }
}
