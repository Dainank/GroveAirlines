﻿using System;
using System.Threading.Tasks;
using GroveAirlines.DatabaseLayer;
using GroveAirlines.DatabaseLayer.Models;
using GroveAirlines.Exceptions;

namespace GroveAirlines.RepositoryLayer
{
    public class BookingRepository
    {
        private readonly GroveAirlinesContext _context; // DB Tools

        public BookingRepository(GroveAirlinesContext _context)  // Constructor Call
        {
            this._context = _context;
        }

        public async Task CreateBooking(int customerId, int flightNumber)
        {   // validate
            if (!customerId.IsPositiveInteger() || !flightNumber.IsPositiveInteger()) // can't be negative
            {
                Console.WriteLine($"Argument Exception in CreateBooking! Customer ID = {customerId}, Flight Number = {flightNumber}");  // for dev
                throw new ArgumentException("Invalid parameters provided; please check parameters.");
            }
            // save
            Booking newBooking = new Booking 
            {
                CustomerId = customerId, 
                FlightNumber = flightNumber
            };

            try
            {
                _context.Booking.Add(newBooking);   // add to save stack
                await _context.SaveChangesAsync(); // save to DB
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception during database save: {exception.Message}");
                throw new CouldNotAddBookingToDatabaseException();  // custom exception
            }
        }
    }
}
