using System;
using System.Collections.Generic;
using System.Xml;
using ITCompanyLocatorMVC.Data;
using ITCompanyLocatorMVC.Models;

namespace ITCompanyLocatorMVC.Service
{
    public class SearchCompany
    {
        private static string _apiKey = "AIzaSyDXNm60TAWJ0dDIF6CDAYxzuEL5RP7Yv3E";
        private static readonly List<CompanyDetails[]> PageList = new List<CompanyDetails[]>();
        private static int _countPageList;
        private static string _baseUrl;
        public static string NextPageToken;
        public static bool IsToken;
        public static bool HasPreviousPage;
        public static int CountPrevClicked;


        #region Private Methods

        /// <summary>
        /// Return the url for given city in string without using any token
        /// </summary>
        /// <param name="cityNameAndTokenStrings"></param>
        /// <returns>Url to make HTTP Request</returns>
        private string GiveBaseUrl(params string[] cityNameAndTokenStrings)
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
        /// Getting details of company from name and formatted_address tag of xml document
        /// and creating object for each company
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>Details of company as a list of CompanyDetails object</returns>
        private CompanyDetails[] GetCompanies(XmlDocument xmlDoc)
        {
            // Getting all the IT Companies name with tag 'name' in XML file to XmlNodeList object
            XmlNodeList nodeListName = xmlDoc.GetElementsByTagName("name");

            // Getting all the IT Companies address with tag 'formatted_address' in XML file to XmlNodeList object
            XmlNodeList nodeListAddress = xmlDoc.GetElementsByTagName("formatted_address");

            XmlNodeList nextToken = xmlDoc.GetElementsByTagName("next_page_token");

            IsToken = false;

            foreach (XmlNode token in nextToken)
            {
                NextPageToken = token.InnerText;
                IsToken = true;
            }

            // Count IT Companies for provided Location
            int countCompanyName = nodeListName.Count;

            // Create object for all the available IT Company
            CompanyDetails[] company = new CompanyDetails[countCompanyName];

            // Set name and address for each company in CompanyDetails object
            for (int i = 0; i < countCompanyName; i++)
            {
                company[i] = new CompanyDetails();
                company[i].Name = nodeListName[i].InnerText;
                company[i].Address = nodeListAddress[i].InnerText;
            }

            PageList.Add(company);

            _countPageList = PageList.Count;

            HasPreviousPage = false;

            if(PageList.Count >= 2)
            {
                HasPreviousPage = true;
            }

            return company;
        }

        #endregion

        #region Public Methods

        public static CompanyDetails[] GiveCompanyDetails(params string[] cityName)
        {
            SearchCompany sc = new SearchCompany();

            // Getting baseurl consist of cityname, api key, tokens(if any)
            _baseUrl = sc.GiveBaseUrl(cityName);

            //if args.length =2 , call give base url with token name

            // Getting company details in xml based on passed BaseUrl
            XmlDocument xmlDoc = ApiData.GiveCompaniesInXml(_baseUrl);

            return sc.GetCompanies(xmlDoc);
        }

        public static CompanyDetails[] GetPreviousResults()
        {
            CountPrevClicked++;
            IsToken = true;
            CompanyDetails[] prevCompany = PageList[_countPageList - 2];
            if (_countPageList == 2)
            {
                HasPreviousPage = false;
            }

            _countPageList -= 1;
            return prevCompany;
        }

        public static CompanyDetails[] GetNextResults()
        {
            CompanyDetails[] nextCompany = PageList[_countPageList];
            CountPrevClicked -= 1;
            HasPreviousPage = true;
            if (_countPageList == 2)
            {
                IsToken = false;
            }
            _countPageList += 1;
            return nextCompany;
        }

        #endregion

    }
}
