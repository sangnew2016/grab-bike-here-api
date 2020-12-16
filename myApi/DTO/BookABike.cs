namespace myApi.DTO
{
    public class BookABike
    {
        public string CurrentAddress { get; set; }
        public string CurrentLatitude { get; set; }
        public string CurrentLongtitude { get; set; }

        public string DestinationAddress { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongtitude { get; set; }

        public string Distance { get; set; }
        public string PricePerKm { get; set; }
        public string Amount { get; set; }

        public string UserBookId { get; set; }
        public string FullNameDelegate { get; set; }
        public string PhoneDelegate { get; set; }
    }
}
