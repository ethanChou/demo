using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleEfficiencyCS
{
    class Program
    {
        static void Main(string[] args)
        {
            EffCompute provider = new EffCompute();
            const int NITER = 201;

            provider.InitPositions();
            provider.UpdatePositions();

            int start = Environment.TickCount;
            for (int i = 0; i < NITER; i++)
            {
                provider.Pot = 0.0;

                //低效模式  
                /* provider.ComputePot(); 
                 if (i % 10 == 0) 
                     Console.WriteLine("{0}: Potential: \t {1}", i, provider.Pot); 
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

    public class EffCompute
    {
        public double Pot { get; set; }
        private int DIMS = 3;
        private int NPARTS = 1000;
        private double[,] _r;
        private Random _rand;

        public EffCompute()
        {
            _r = new double[DIMS, NPARTS];
            _rand = new Random();
        }


        public void InitPositions()
        {
            for (int i = 0; i < DIMS; i++)
            {
                for (int j = 0; j < NPARTS; j++)
                {
                    _r[i, j] = 0.5 + _rand.NextDouble();
                }
            }
        }

        public void UpdatePositions()
        {
            for (int i = 0; i < DIMS; i++)
            {
                for (int j = 0; j < NPARTS; j++)
                {
                    _r[i, j] -= 0.5 + _rand.NextDouble();
                }
            }
        }

        public double ComputePot()
        {
            double distx, disty, distz, dist;
            double pot;
            distx = 0;
            disty = 0;
            distz = 0;
            pot = 0;

            for (int i = 0; i < NPARTS; i++)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    distx = Math.Pow((_r[0, j] - _r[0, i]), 2);
                    disty = Math.Pow((_r[1, j] - _r[1, i]), 2);
                    distz = Math.Pow((_r[2, j] - _r[2, i]), 2);
                    dist = Math.Sqrt(distx + disty + distz);
                    pot += 1.0 / dist;
                }
            }

            this.Pot = pot;

            return pot;
        }
    }  
}
