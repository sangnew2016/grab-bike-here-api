using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myApi.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace myApi.Controllers
{
    [ApiController]
    [Route("api/position")]
    public class PositionController : ControllerBase
    {                        
        public PositionController()
        {                     
        }        
                
        [HttpPut]
        public IActionResult PutCurrentPosition(Position position)
        {                                                
            if (Singleton.PositionTree.ContainsKey(position.Email)) {
                Singleton.PositionTree[position.Email] = position;
            } else {
                Singleton.PositionTree.Add(position.Email, position);
            }
            return Ok();
        }        

        [HttpGet]
        [Route("drivers")]
        public ActionResult<IList<Position>> GetDriverPositions()
        {
            var email = HttpContext.Request.Query["email"].ToString();
            if (!Singleton.PositionTree.ContainsKey(email)) return null;

            var currentPosition = Singleton.PositionTree[email];

            var positions = Singleton.PositionTree
                .Where(p => p.Value.UserType == "driver" 
                    && Distance(currentPosition, p.Value) <= 500)           // with Driver: no need BookId
                .Select(p => p.Value).ToList();            

            return positions;            
        }

        [HttpGet]
        [Route("users")]
        public ActionResult<IList<Position>> GetUserPositions()
        {
            var email = HttpContext.Request.Query["email"].ToString();
            if (!Singleton.PositionTree.ContainsKey(email)) return null;

            var radius = Convert.ToInt32(HttpContext.Request.Query["radius"]);
            var currentPosition = Singleton.PositionTree[email];

            var positions = Singleton.PositionTree
                .Where(p => p.Value.UserType == "user" 
                    && Distance(currentPosition, p.Value) <= radius
                    && p.Value.DriverTaken is null                          // with User: not captured by driver yet
                    && p.Value.BookId > 0)                                  // with User: should have BookId
                .Select(p => p.Value).ToList();

            return positions;            
        }











        private double Distance(Position p1, Position p2)
        {
            var R = 6378137; // Earth’s mean radius in meter
            var dLat = rad(Convert.ToDouble(p2.Latitude) - Convert.ToDouble(p1.Latitude));
            var dLong = rad(Convert.ToDouble(p2.Longtitude) - Convert.ToDouble(p1.Longtitude));
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(rad(Convert.ToDouble(p1.Latitude))) * Math.Cos(rad(Convert.ToDouble(p2.Latitude))) *
                      Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return (d / 1);             // returns the distance in meter (/ 1000 -> km)

            double rad(double x)
            {
                return x * Math.PI / 180;
            }
        }
    }
}
