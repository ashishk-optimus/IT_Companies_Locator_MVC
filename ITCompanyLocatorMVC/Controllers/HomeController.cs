using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ITCompanyLocatorMVC.Models;
using ITCompanyLocatorMVC.Service;
using System.Xml;

namespace ITCompanyLocatorMVC.Controllers
{
    public class HomeController : Controller
    {
        private static string cityName;
        private static string BaseUrl;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchActionResult()
        { 

            if (Request.Form.ContainsKey("cityName"))
            {
                cityName = Request.Form["cityName"].ToString();

                // Getting baseurl consist of cityname, api key, tokens(if any)
                BaseUrl = SearchCompany.GiveBaseUrl(cityName);
            }
            else
            {
                if (Request.Form.ContainsKey("nextButton"))
                {
                    if(SearchCompany.CountPageList + 1 == SearchCompany.PageList.Count)
                    {
                        ViewData["isToken"] = SearchCompany.isToken;
                        ViewBag.Companies = (CompanyDetails[])SearchCompany.PageList[SearchCompany.CountPageList];
                        if (SearchCompany.PageList.Count == 3)
                        {
                            SearchCompany.isToken = false;
                        }

                        SearchCompany.CountPageList += 1;
                        ViewData["hasPreviousPage"] = SearchCompany.hasPreviousPage;

                        return View();
                    }
                    else
                    {
                        BaseUrl = SearchCompany.GiveBaseUrl(cityName, SearchCompany.NextPageToken);
                    }
                    
                }

                if (Request.Form.ContainsKey("prevButton"))
                {
                    SearchCompany.isToken = true;
                    ViewData["isToken"] = SearchCompany.isToken;
                    ViewBag.Companies = (CompanyDetails[])SearchCompany.PageList[SearchCompany.CountPageList - 2];
                    if(SearchCompany.CountPageList == 2)
                    {
                        SearchCompany.hasPreviousPage = false;
                    }

                    SearchCompany.CountPageList -= 1;
                    ViewData["hasPreviousPage"] = SearchCompany.hasPreviousPage;

                    return View();
                }
            }
            

            // Getting company details in xml based on passed BaseUrl
            XmlDocument xmlDoc = SearchCompany.GiveCompaniesInXml(BaseUrl);

            // Getting array of objects for all the companies
            CompanyDetails[] company = SearchCompany.GiveCompanyDetails(xmlDoc);

            ViewBag.Companies = company;
            ViewData["CityName"] = cityName;
            ViewData["isToken"] = SearchCompany.isToken;
            ViewData["hasPreviousPage"] = SearchCompany.hasPreviousPage;

            return View();
        }
    }
}
