using E_Administration.Models;

namespace E_Administration.Dto
{
    public class LabViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<Lab> Labs { get; set; }
        public Lab Lab { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
