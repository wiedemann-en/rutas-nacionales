using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Routing
{
    class Program
    {
        static void Main(string[] args)
        {
            var routingCalculator = new RoutingCalculator();
            routingCalculator.CalculatePendingRoutes("driving");
        }
    }
}
