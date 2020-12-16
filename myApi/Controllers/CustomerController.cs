using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myApi.DTO;

namespace myApi.Controllers
{
    [Route("api/customer")]    
    [ApiController]
    public class CustomerController : BaseController
    {
        public CustomerController()
        {
            
        }

        [HttpPost]
        [Route("bookabike")]
        [Authorize(Policy = Policies.User)]
        public ActionResult BookABike(BookABike bookABike)
        {
            const int bookingStatusId = 1;              // PENDING

            // validate          

            // insert Bookings
            var sql = $@"
                START TRANSACTION;

                INSERT INTO `Bookings`(
                    `user_book_id`, `booking_datetime`, `price`, `distance`, 
                 `phone_delegate`, `fullname_delegate`, `booking_status_id`,
                    `from_address`, `from_latitude`, `from_longtitude`,
                    `to_address`, `to_latitude`, `to_longtitude`,
                    `current_address`, `current_latitude`, `current_longtitude`) 
                VALUES(
                    '{bookABike.UserBookId}', NOW(), {bookABike.PricePerKm}, {bookABike.Distance},
                    '{bookABike.PhoneDelegate}', '{bookABike.FullNameDelegate}', {bookingStatusId},
                    '{bookABike.CurrentAddress}', '{bookABike.CurrentLatitude}', '{bookABike.CurrentLongtitude}',
                    '{bookABike.DestinationAddress}', '{bookABike.DestinationLatitude}', '{bookABike.DestinationLongtitude}',
                    '{bookABike.CurrentAddress}', '{bookABike.CurrentLatitude}', '{bookABike.CurrentLongtitude}'
                );

                SELECT LAST_INSERT_ID();

                COMMIT;
            ";

            var bookId = _Data.BaseMYSQL.GetScalarValue(sql);

            return Ok(new { bookId });
        }
    }
}
