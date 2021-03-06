﻿using AsyncIO.Core;
using AsyncIO.DemoConsole.Adapters;
using AsyncIO.DemoConsole.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncIO.DemoConsole
{
    class Program
    {
        const string astonMartinPath = "./AstonMartin";
        const string astonMartinAsyncPath = "./AstonMartinAsync";

        static void Main()
        {
            DoWithoutTransaction().Wait();
            DoWithTransaction().Wait();
            Console.ReadKey();
        }

        static async Task DoWithoutTransaction()
        {
            // Injectable ILogger in constructor.
            var asyncio = new IO(GetLogger());

            Configurations(asyncio);
            BrandToFormat(asyncio);
            await BrandToFile(asyncio);
            await BrandFromFile(asyncio);
        }

        static async Task DoWithTransaction()
        {
            // Transactions are not global, each instance has it's own transaction.
            // Injectable ILogger in constructor.
            var asyncio = new IO(GetLogger());
            try
            {
                asyncio.BeginTransaction();
                Configurations(asyncio);
                BrandToFormat(asyncio);
                await BrandToFile(asyncio);
                await BrandFromFile(asyncio);
                asyncio.Commit();
            }
            catch (Exception e)
            {
                asyncio.Rollback();
                Console.WriteLine(e);
            }
        }

        static void Configurations(IO asyncio = null)
        {
            // Configurations are not global, each instance has it's own settings.
            asyncio = asyncio ?? new IO();

            asyncio.CsvConfiguration = new CsvHelper.Configuration.Configuration();
            asyncio.JsonFormatting = Newtonsoft.Json.Formatting.Indented;
            asyncio.JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
        }

        static void BrandToFormat(IO asyncio = null)
        {
            asyncio = asyncio ?? new IO();
            var brand = GetAstonMartinBrand();

            string csvString = asyncio.Conversions.ToCsv(brand.Cars);
            string jsonString = asyncio.Conversions.ToJson(brand);

            // Self reference loops not handled in these types.
            string xmlString = asyncio.Conversions.ToXml(brand);
            byte[] bsonBytes = asyncio.Conversions.ToBson(brand);
        }

        static async Task BrandToFile(IO asyncio = null)
        {
            asyncio = asyncio ?? new IO();
            var brand = GetAstonMartinBrand();

            asyncio.File.WriteCsv(astonMartinPath + ".csv", brand.Cars);
            await asyncio.File.WriteCsvAsync(astonMartinAsyncPath + ".csv", brand.Cars);

            asyncio.File.WriteJson(astonMartinPath + ".json", brand);
            await asyncio.File.WriteJsonAsync(astonMartinAsyncPath + ".json", brand);

            // Self reference loops not handled in these types.
            asyncio.File.WriteXml(astonMartinPath + ".xml", brand);
            await asyncio.File.WriteXmlAsync(astonMartinPath + ".xml", brand);

            asyncio.File.WriteBson(astonMartinPath + ".bson", brand);
            await asyncio.File.WriteBsonAsync(astonMartinPath + ".bson", brand);
        }

        static async Task BrandFromFile(IO asyncio = null)
        {
            asyncio = asyncio ?? new IO();

            var csvCars = asyncio.File.ReadCsv<Car>(astonMartinPath + ".csv");
            var asyncCsvCars = await asyncio.File.ReadCsvAsync<Car>(astonMartinPath + ".csv");

            var jsonBrand = asyncio.File.ReadJson<Brand>(astonMartinPath + ".json");
            var asyncJsonBrand = await asyncio.File.ReadJsonAsync<Brand>(astonMartinPath + ".json");

            // Self reference loops not handled in these types.
            var xmlBrand = asyncio.File.ReadXml<Brand>(astonMartinPath + ".xml");
            var asyncXmlBrand = await asyncio.File.ReadXmlAsync<Brand>(astonMartinPath + ".xml");

            var bsonBrand = asyncio.File.ReadBson<Brand>(astonMartinPath + ".bson");
            var asyncBsonBrand = await asyncio.File.ReadBsonAsync<Brand>(astonMartinPath + ".bson");
        }

        static Brand GetAstonMartinBrand()
        {
            return new Brand()
            {
                Name = "Aston Martin",
                Owner = "Ford Motor Company",
                Cars = new List<Car>()
                {
                    new Car()
                    {
                        Model = "DB2",
                        HorsePower = 140,
                    },
                    new Car()
                    {
                        Model = "DB6",
                        HorsePower = 330,
                    }
                }
            };
        }

        static Microsoft.Extensions.Logging.ILogger GetLogger()
        {
            return new SerilogLogger(
                new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger());
        }
    }
}
