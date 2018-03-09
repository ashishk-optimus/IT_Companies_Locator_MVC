using System;
using System.Collections.Generic;
using System.Xml;
using ITCompanyLocatorMVC.Data;
using ITCompanyLocatorMVC.Models;

namespace ITCompanyLocatorMVC.Service
{
    /// <summary>
    /// This class is responsible to take request from controller and 
    /// return responses as model to controller
    /// </summary>
    public class SearchCompany
    {


        #region Private Fields

        private const string ApiKey = "AIzaSyBnYtjMBztN69lfDr2UF04LJg1TpHAokek";
        private static readonly List<CompanyDetails[]> PageList = new List<CompanyDetails[]>();
        private static string _baseUrl;

        #endregion

        #region Public Fields

        public static string NextPageToken;
        public static bool IsToken;
        public static bool HasPreviousPage;
        public static int CountPrevClicked;
        public static int CountPageList;

        #endregion
         
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
                                 formattedCityName + "&hasNextPage=true&nextPage()=true&key=" + ApiKey;
            }

            // URL mor making request to Google Places API along with the Key with token
            if (cityNameAndTokenStrings.Length == 2)
            {
                string nextPageToken = cityNameAndTokenStrings[1];
                baseUrl = "https://maps.googleapis.com/maps/api/place/textsearch/xml?query=it+companies+in+" +
                                 formattedCityName + "&hasNextPage=true&nextPage()=true&key=" + ApiKey + "&pagetoken=" + nextPageToken;
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

            // Adding list of company in a readonly list
            PageList.Add(company);

            // Keep track of number of added list of company
            CountPageList = PageList.Count;

            // when we have only one page available i.e. PageList of length 1
            HasPreviousPage = false;

            // Setting Previous pages available after we have 2 or more than 2 pages available in pagelist
            if(PageList.Count >= 2)
            {
                HasPreviousPage = true;
            }

            return company;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Getting list of companies for passed city name
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Getting company details for next button clicked
        /// </summary>
        /// <returns></returns>
        public static CompanyDetails[] GetPreviousResults()
        {
            // Keep track of prev button clicked
            CountPrevClicked++;

            // Setting next button visible on UI
            IsToken = true;
            CompanyDetails[] prevCompany = PageList[CountPageList - 2];

            // when PageList[0] is reached, then making prev button disable on UI
            if (CountPageList == 2)
            {
                HasPreviousPage = false;
            }

            CountPageList -= 1;

            return prevCompany;
        }

        /// <summary>
        /// Getting company details for prev button clicked
        /// </summary>
        /// <returns></returns>
        public static CompanyDetails[] GetNextResults()
        {
            CompanyDetails[] nextCompany = PageList[CountPageList];

            CountPrevClicked -= 1;
            HasPreviousPage = true;

            if (CountPageList == 2)
            {
                IsToken = false;
            }

            CountPageList += 1;

            return nextCompany;
        }

        #endregion

    }
}
