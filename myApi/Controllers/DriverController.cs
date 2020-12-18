using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myApi.DTO;
using System;

namespace myApi.Controllers
{
    [Route("api/driver")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        public DriverController()
        {

        }

        [HttpPost]
        [Route("getabook")]
        [Authorize(Policy = Policies.User)]
        public ActionResult GetABook(Position userPosition)
        {
            const int bookingStatusId = 2;              // ACCEPTED

            // validate          

            // insert Bookings
            var sql = $@"
                START TRANSACTION;

                UPDATE `Bookings` SET `booking_status_id` = '{bookingStatusId}' WHERE `id` = '{userPosition.BookId}';

                INSERT INTO `Orders`(
                    `booking_id`, 
                    `user_taken_id`, 
                    `order_datetime`                     
                )
                VALUES(
                    '{userPosition.BookId}', 
                    '{userPosition.DriverTaken}', 
                    NOW()
                );

                SELECT LAST_INSERT_ID();

                COMMIT;
            ";

            var orderId = _Data.BaseMYSQL.GetScalarValue(sql);

            var email = Convert.ToString(_Data.BaseMYSQL.GetScalarValue(@$"
                SELECT A.Email 
                FROM `accounts` A inner join `bookings` B on A.id = B.user_book_id 
                WHERE B.id = '{userPosition.BookId}';"
            ));

            // update Dictionary (support Driver manage user position)
            if (Singleton.PositionTree.ContainsKey(email))
            {
                Singleton.PositionTree[email].DriverTaken = userPosition.DriverTaken;
            }
            else
            {
                throw new Exception($"Api: This email '{email}' does not esits in Dictionary");
            }


            return Ok(new { orderId });
        }
    }
}
