using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfficiencyCLRWrapper;

namespace EfficiencyNativeCLRWrapperCS
{
    class Program
    {
        static void Main(string[] args)
        {
            CLRWrapper provider = new CLRWrapper();
            const int NITER = 201;

            provider.InitPositions();
            provider.UpdatePositions();

            int start = Environment.TickCount;
            for (int i = 0; i < NITER; i++)
            {
                provider.Pot = 0.0;

                //低效模式  
                /*provider.ComputePot(); 
                if (i % 10 == 0) 
                    Console.WriteLine("{0}: Potential: \t {1}", i, provider.Pot()); 
                 */

                //高效模式                  
                if (i % 10 == 0)
                    Console.WriteLine("{0}: Potential: \t {1}", i, provider.ComputePot());
                provider.UpdatePositions();

            }
            int stop = Environment.TickCount;

            Console.WriteLine("Seconds = {0,10}", (double)(stop - start) / 1000);

            Console.ReadKey();  
        }
    }
}
