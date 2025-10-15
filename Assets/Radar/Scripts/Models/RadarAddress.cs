using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents an address.
    /// </summary>
    [System.Serializable]
    public class RadarAddress
    {
        [SerializeField] private RadarCoordinate coordinate;
        [SerializeField] private string formattedAddress;
        [SerializeField] private string country;
        [SerializeField] private string countryCode;
        [SerializeField] private string countryFlag;
        [SerializeField] private string dma;
        [SerializeField] private string dmaCode;
        [SerializeField] private string state;
        [SerializeField] private string stateCode;
        [SerializeField] private string postalCode;
        [SerializeField] private string city;
        [SerializeField] private string borough;
        [SerializeField] private string county;
        [SerializeField] private string neighborhood;
        [SerializeField] private string number;
        [SerializeField] private string street;
        [SerializeField] private string addressLabel;
        [SerializeField] private string placeLabel;
        [SerializeField] private string unit;
        [SerializeField] private string plus4;
        [SerializeField] private double? distance;
        [SerializeField] private string layer;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarAddressConfidence confidence;
        [SerializeField] private RadarTimeZone timeZone;

        public RadarCoordinate Coordinate { get => coordinate; set => coordinate = value; }
        public string FormattedAddress { get => formattedAddress; set => formattedAddress = value; }
        public string Country { get => country; set => country = value; }
        public string CountryCode { get => countryCode; set => countryCode = value; }
        public string CountryFlag { get => countryFlag; set => countryFlag = value; }
        public string Dma { get => dma; set => dma = value; }
        public string DmaCode { get => dmaCode; set => dmaCode = value; }
        public string State { get => state; set => state = value; }
        public string StateCode { get => stateCode; set => stateCode = value; }
        public string PostalCode { get => postalCode; set => postalCode = value; }
        public string City { get => city; set => city = value; }
        public string Borough { get => borough; set => borough = value; }
        public string County { get => county; set => county = value; }
        public string Neighborhood { get => neighborhood; set => neighborhood = value; }
        public string Number { get => number; set => number = value; }
        public string Street { get => street; set => street = value; }
        public string AddressLabel { get => addressLabel; set => addressLabel = value; }
        public string PlaceLabel { get => placeLabel; set => placeLabel = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Plus4 { get => plus4; set => plus4 = value; }
        public double? Distance { get => distance; set => distance = value; }
        public string Layer { get => layer; set => layer = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarAddressConfidence Confidence { get => confidence; set => confidence = value; }
        public RadarTimeZone TimeZone { get => timeZone; set => timeZone = value; }
    }
}
