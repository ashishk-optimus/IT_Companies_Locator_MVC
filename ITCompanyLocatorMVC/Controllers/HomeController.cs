using Microsoft.AspNetCore.Mvc;
using ITCompanyLocatorMVC.Models;
using ITCompanyLocatorMVC.Service;

namespace ITCompanyLocatorMVC.Controllers
{
    /// <summary>
    /// Home Controller which take Http requests and 
    /// route it to the particular function particular action
    /// </summary>
    public class HomeController : Controller
    {
        private static string _cityName;
        private CompanyDetails[] _company;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchActionResult()
        {
            if (Request.Form.ContainsKey("cityName"))
            {
                _cityName = Request.Form["cityName"].ToString();

                // Getting list of objects for all the companies
                _company = SearchCompany.GiveCompanyDetails(_cityName);
            }
            else
            {
                if (Request.Form.ContainsKey("nextButton"))
                {
                    if(SearchCompany.CountPrevClicked != 0)
                    {
                        // Getting List of objects for all the next available companies
                        _company = SearchCompany.GetNextResults();
                    }
                    else
                    {
                        // Getting list of objects for all the companies for available token
                        _company = SearchCompany.GiveCompanyDetails(_cityName, SearchCompany.NextPageToken);
                    }
                }

                if (Request.Form.ContainsKey("prevButton"))
                {
                    // Getting List of objects for all the previous available companies
                    _company = SearchCompany.GetPreviousResults();
                }
            }

            ViewBag.Companies = _company;
            ViewData["CityName"] = _cityName;
            ViewData["isToken"] = SearchCompany.IsToken;
            ViewData["hasPreviousPage"] = SearchCompany.HasPreviousPage;
            ViewData["AvailableResults"] = SearchCompany.CountPageList;

            return View();
        }
    }
}
