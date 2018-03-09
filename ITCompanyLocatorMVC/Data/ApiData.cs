using System;
using System.Net;
using System.Xml;

namespace ITCompanyLocatorMVC.Data
{
    public class ApiData
    {
        
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
            XmlDocument xmlDoc = new XmlDocument();

            if (req != null)
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    if (resp != null)
                    {
                        // Loading response to XmlDocument object
                        xmlDoc.Load(resp.GetResponseStream() ?? throw new InvalidOperationException()); 
                    }
                }

            return xmlDoc;
        }
    }
}
