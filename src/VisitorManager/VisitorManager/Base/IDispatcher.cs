using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VisitorManager
{
    public interface IDispatcher
    {
        Dispatcher Dispatcher { get; set; }
    }
}
