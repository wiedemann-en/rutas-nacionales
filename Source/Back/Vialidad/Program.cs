using System;
using System.Collections.Generic;
using Vialidad.Contracts.Enums;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Download;
using Vialidad.Download.Providers;
using Vialidad.Fixture;
using Vialidad.Normalizer;
using Vialidad.Services;
using Vialidad.Services.Normalizer;

namespace Vialidad
{
    class Program
    {
        static void Main(string[] args)
        {
            //DateTime baseDate = new DateTime(1970, 1, 1);
            //TimeSpan diff1 = new DateTime(2018, 1, 1) - baseDate;
            //Console.WriteLine(diff1.TotalMilliseconds);
            //TimeSpan diff2 = new DateTime(2018, 12, 31) - baseDate;
            //Console.WriteLine(diff2.TotalMilliseconds);
            //TimeSpan now = DateTime.Now.Date - baseDate;
            //Console.WriteLine(now.TotalMilliseconds);

            //var normalizer = new ScraperNormalizer();
            //normalizer.NormalizeDb();

            var fixture = new ScraperFixture();
            fixture.ExecuteFixture();
        }
    }
}
