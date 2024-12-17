using E_Administration.Models;

namespace E_Administration.Dto
{
    public class AssignmentViewModel
    {
        public IEnumerable<Assignments> Assignments { get; set; }
        public Assignments Assignment { get; set; }
    }
}
