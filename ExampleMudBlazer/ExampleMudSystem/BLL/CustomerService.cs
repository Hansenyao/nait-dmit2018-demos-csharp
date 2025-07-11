using ExampleMudSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMudSystem.BLL
{
    public class CustomerService
    {
        private readonly OLTPDMIT2018Context _context;
        internal CustomerService(OLTPDMIT2018Context context) 
        {
            _context = context;
        }


    }
}
