namespace ITCompanyLocatorMVC.Models
{
    /// <summary>
    /// Model class to model name and address of respective company
    /// </summary>
    public class CompanyDetails
    { 
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
    }
}
