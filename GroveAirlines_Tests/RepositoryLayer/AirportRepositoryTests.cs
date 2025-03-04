﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroveAirlines.DatabaseLayer;
using GroveAirlines.DatabaseLayer.Models;
using GroveAirlines.Exceptions;
using GroveAirlines.RepositoryLayer;
using GroveAirlines_Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroveAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class AirportRepositoryTests
    {
        private GroveAirlinesContext _context;
        private AirportRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            DbContextOptions<GroveAirlinesContext> dbContextOptions =   // in memory temp database
                new DbContextOptionsBuilder<GroveAirlinesContext>().UseInMemoryDatabase("Grove").Options;
            _context = new GroveAirlinesContext_Stub(dbContextOptions);

            SortedList<string, Airport> airports = new SortedList<string, Airport>
            {
                {
                    "GOH",
                    new Airport
                    {
                        AirportId = 1,
                        City = "Amsterdam",
                        Iata = "AMS"
                    }
                },
                {
                    "PHX",
                    new Airport
                    {
                        AirportId = 2,
                        City = "Luxembourg",
                        Iata = "LUX"
                    }
                },
                {
                    "DDH",
                    new Airport
                    {
                        AirportId = 3,
                        City = "Manchester",
                        Iata = "MAN"
                    }
                },
                {
                    "RDU",
                    new Airport
                    {
                        AirportId = 4,
                        City = "Strasbourg",
                        Iata = "SXB"
                    }
                }
            };

            _context.Airport.AddRange(airports.Values);
            await _context.SaveChangesAsync();

            _repository = new AirportRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public async Task GetAirportByID_Success(int airportId)
        {
            Airport airport = await _repository.GetAirportById(airportId);
            Assert.IsNotNull(airport);

            Airport dbAirport = _context.Airport.First(a => a.AirportId == airportId);
            Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
            Assert.AreEqual(dbAirport.City, airport.City);
            Assert.AreEqual(dbAirport.Iata, airport.Iata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAirportByID_Failure_InvalidInput()
        {
            StringWriter outputStream = new StringWriter();
            try
            {
                Console.SetOut(outputStream);
                await _repository.GetAirportById(-1);
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(outputStream.ToString().Contains($"Argument exception in GetAirportByID! airportID = "));
                throw new ArgumentException();
            }
            finally
            {
                outputStream.Dispose(); // TODO: Async Overload?
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportByID_Failure_DatabaseException()
        {
            await _repository.GetAirportById(10);
        }

    }
}
