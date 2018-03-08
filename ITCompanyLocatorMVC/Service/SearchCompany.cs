using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using ITCompanyLocatorMVC.Models;

namespace ITCompanyLocatorMVC.Service
{
    public class SearchCompany
    {
        private static string _apiKey = "AIzaSyDhQGQHpPK9JjK4_e1grZ31_zWAkDft6_o";
        public static string NextPageToken;
        public static bool isToken;
        public static bool hasPreviousPage;
        public static List<CompanyDetails[]> PageList = new List<CompanyDetails[]>(); 
        public static int CountPageList;
        /// <summary>
        /// Return the url for given city in string without using any token
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns>Url to make HTTP Request</returns>
        public static string GiveBaseUrl(params string[] cityNameAndTokenStrings)
        {
            string cityName = cityNameAndTokenStrings[0];
            string formattedCityName = String.Empty; // string to format the provided city name in reuired url
            string[] cityInfo = cityName.Split(' ');
            string baseUrl = String.Empty;

            // format the provided city name to their respective url
            foreach (string word in cityInfo)
            {
                formattedCityName += word + "+";
            }

            // URL mor making request to Google Places API along with the Key without token
            if (cityNameAndTokenStrings.Length == 1)
            {
                baseUrl = "https://maps.googleapis.com/maps/api/place/textsearch/xml?query=it+companies+in+" +
                                 formattedCityName + "&hasNextPage=true&nextPage()=true&key=" + _apiKey;
            }

            // URL mor making request to Google Places API along with the Key with token
            if (cityNameAndTokenStrings.Length == 2)
            {
                string nextPageToken = cityNameAndTokenStrings[1];
                baseUrl = "https://maps.googleapis.com/maps/api/place/textsearch/xml?query=it+companies+in+" +
                                 formattedCityName + "&hasNextPage=true&nextPage()=true&key=" + _apiKey + "&pagetoken=" + nextPageToken;
            }

            return baseUrl;
        }

        /// <summary>
        /// Return the response for given city in XmlDocument object for the given baseUrl
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument GiveCompaniesInXml(string baseUrl)
        {
            // Making HTTP Request using baseURL 
            HttpWebRequest req = WebRequest.Create(baseUrl) as HttpWebRequest;

            // Object of XmlDocument to hold the Http Response in XML format
            XmlDocument _xmlDoc = new XmlDocument();
            if (req != null)
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    if (resp != null)
                    {
                        _xmlDoc.Load(resp.GetResponseStream() ?? throw new InvalidOperationException()); // Loading response to XmlDocument object
                    }
                }

            return _xmlDoc;
        }

        /// <summary>
        /// Getting details of company from name and formatted_address tag of xml document
        /// and creating object for each company
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>Details of company as a list of CompanyDetails object</returns>
        public static CompanyDetails[] GiveCompanyDetails(XmlDocument xmlDoc)
        {
            // Getting all the IT Companies name with tag 'name' in XML file to XmlNodeList object
            XmlNodeList nodeListName = xmlDoc.GetElementsByTagName("name");

            // Getting all the IT Companies address with tag 'formatted_address' in XML file to XmlNodeList object
            XmlNodeList nodeListAddress = xmlDoc.GetElementsByTagName("formatted_address");

            XmlNodeList nextToken = xmlDoc.GetElementsByTagName("next_page_token");

            isToken = false;

            foreach (XmlNode token in nextToken)
            {
                NextPageToken = (token.InnerText).ToString();
                isToken = true;
            }

            // Count IT Companies for provided Location
            int _countCompanyName = nodeListName.Count;

            // Create object for all the available IT Company
            CompanyDetails[] company = new CompanyDetails[_countCompanyName];

            // Set name and address for each company in CompanyDetails object
            for (int i = 0; i < _countCompanyName; i++)
            {
                company[i] = new CompanyDetails();
                company[i].Name = nodeListName[i].InnerText;
                company[i].Address = nodeListAddress[i].InnerText;
            }

            PageList.Add(company);

            CountPageList = PageList.Count;

            hasPreviousPage = false;

            if(PageList.Count >= 2)
            {
                hasPreviousPage = true;
            }

            return company;
        }
        
    }
}
