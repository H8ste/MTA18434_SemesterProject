using System;

namespace discretefrouiertransform
{
    public class DOAclass
    {
        double meanx, meany, denom, sx, sy, sxy;
        int i, j;

        public int CrossCorrelation(double[] x, double[] y, int Samples, int maxDelay)
        {
            meanx = 0;
            meany = 0;
            double r;
            double max = 0;
            int BestDelay = 0;
            double[] tempx = x;
            double[] tempy = y;

            //Calculating the mean
            for (int i = 0; i < Samples; i++)
            {
                meanx += x[i];
                meany += y[i];
            }
            meanx /= Samples;
            meany /= Samples;


            //Denominator
            sx = 0;
            sy = 0;
            for (i = 0; i < Samples; i++)
            {
                sx += (tempx[i] - meanx) * (x[i] - meanx);
                sy += (tempy[i] - meany) * (y[i] - meany);
            }
            denom = Math.Sqrt(sx * sy);

            //Calculate the correlation series
            for (int delay = -maxDelay; delay < maxDelay; delay++)
            {
                sxy = 0;
                for (i = 0; i < Samples; i++)
                {
                    j = i + delay;
                    /*
                    if (j < 0 || j >= Samples)
                        continue;
                    else
                        sxy += (x[i] - meanx) * (y[j] - meany);
                        */

                    if (j < 0 || j >= Samples)
                       sxy += (x[i] - meanx) * (-meany);
                    else
                       sxy += (x[i] - meanx) * (y[j] - meany);
                    //Console.Write(sxy + " , ");
                }

                r = sxy / denom;
                Console.WriteLine("r: " + r);
                Console.WriteLine("delay: " + delay);
                if (r > max)
                {
                    max = r;
                    BestDelay = delay;
                }
            }
            return BestDelay;
        }
    }
}
