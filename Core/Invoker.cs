using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogViewer
{
    public interface IInvoker
    {
        void Invoke(Action run);
    }
    public class DirectInvoker : IInvoker
    {
        public void Invoke(Action run) 
        {
            run();
        }
    }
}
