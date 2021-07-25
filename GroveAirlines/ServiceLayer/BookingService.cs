﻿using GroveAirlines.DatabaseLayer.Models;
using GroveAirlines.Exceptions;
using GroveAirlines.RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroveAirlines.ServiceLayer
{
    public class BookingService
    {

        private readonly BookingRepository _bookingRepository;
        private readonly FlightRepository _flightRepository;
        private readonly CustomerRepository _customerRepository;

        public BookingService(BookingRepository repository)
        {
            _bookingRepository = repository;
        }

        public BookingService(BookingRepository bookingRepository, CustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
        }

        public BookingService(BookingRepository bookingRepository, FlightRepository flightRepository, CustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _customerRepository = customerRepository;
        }

        public async Task<(bool, Exception)> CreateBooking(string customerName, int flightNumber)
        {
            if (string.IsNullOrEmpty(customerName) || !flightNumber.IsPositiveInteger())
            {
                return (false, new ArgumentException());
            }

            try
            {
                Customer customer;
                try
                {
                    customer = await _customerRepository.GetCustomerByName(customerName);   // pickup customer
                }
                catch (CustomerNotFoundException)   // if customer not found (or other exception)
                {
                    await _customerRepository.CreateCustomer(customerName); // create a new customer since he/she is a new customer
                    return await CreateBooking(customerName, flightNumber); // restart booking method with customer now existing
                }

                await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);  // now create booking with picked up user
                return (true, null);    // exit method cleanly
            }
            catch (Exception exception)
            {
                return (false, exception);  // exit method with error
            }
        }
    }
}
