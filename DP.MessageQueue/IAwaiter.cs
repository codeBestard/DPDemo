using System;
using System.Collections.Generic;
using System.Text;

namespace DP.MessageQueue
{
    public interface IAwaiter
    {
        void WaitOne();
    }

    public class Awaiter : IAwaiter
    {
        public Awaiter()
        {
            
        }
        public void WaitOne()
        {
            
        }
    }
}
