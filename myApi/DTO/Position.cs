using System.Collections.Generic;

namespace myApi.DTO
{
    public class Position
    {
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }

        public string DriverTaken { get; set; }
        public int BookId { get; set; }                             // BookId
    }

    public sealed class Singleton
    {
        public static IDictionary<string, Position> PositionTree = new Dictionary<string, Position>();        
    }

    //public sealed class Singleton
    //{
    //    IDictionary<string, Position> PositionTree = new Dictionary<string, Position>();

    //    private Singleton()
    //    { }


    //    public static Singleton Instance { get { return Nested.instance; } }

    //    private class Nested
    //    {
    //        // Explicit static constructor to tell C# compiler
    //        // not to mark type as beforefieldinit
    //        static Nested()
    //        { }

    //        internal static readonly Singleton instance = new Singleton();
    //    }
    //}
}
