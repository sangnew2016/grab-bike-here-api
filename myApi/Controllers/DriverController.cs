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
                
                SELECT 
                    A.Email, 
                    A.phone, 
                    LAST_INSERT_ID() as orderId, 
                    (SELECT email FROM `accounts` WHERE id = '{userPosition.DriverTaken}') as DriverEmail
                FROM `accounts` A inner join `bookings` B on A.id = B.user_book_id 
                WHERE B.id = '{userPosition.BookId}';

                COMMIT;
            ";
            
            var accountDatable = _Data.BaseMYSQL.getDataTable(sql);
            if (accountDatable.Rows.Count <= 0) {
                throw new Exception($"Api: Query Account with bookId '{userPosition.BookId}' failed. Check your data again");
            }
            var email = Convert.ToString(accountDatable.Rows[0]["email"]);
            var phone = Convert.ToString(accountDatable.Rows[0]["phone"]);
            var orderId = Convert.ToString(accountDatable.Rows[0]["orderId"]);
            var driverEmail = Convert.ToString(accountDatable.Rows[0]["DriverEmail"]);

            // update Dictionary of User (support Driver manage user position)
            if (Singleton.PositionTree.ContainsKey(email))
            {
                Singleton.PositionTree[email].DriverTaken = userPosition.DriverTaken;                                
            }
            else
            {
                throw new Exception($"Api: This email '{email}' does not esits in Dictionary");
            }

            // update Dictionary of Driver (support User found Driver position)
            if (Singleton.PositionTree.ContainsKey(driverEmail))
            {
                Singleton.PositionTree[driverEmail].CombineEmail = email + "___" + driverEmail;
            }
            else
            {
                throw new Exception($"Api: This email '{email}' does not esits in Dictionary");
            }
                        
            return Ok(new { orderId, phone, email });
        }
    }
}
