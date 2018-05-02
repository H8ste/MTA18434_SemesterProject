using System;

namespace discretefrouiertransform
{
    public class DOAclass
    {
        double meanx, meany, denom, sx, sy, sxy, max, r;
        int i, j, BestDelay;
        bool TreshReached;

        public  int CrossCorrelation(short[] x, short[] y, int Samples, int maxDelay)
        {
            meanx = 0; meany = 0; max = 0; r = 0; BestDelay = 0;
            short[] tempx = x;
            short[] tempy = y;

            //Calculating the mean of both signals
            for (int i = 0; i < Samples; i++)
            {
                meanx += tempx[i];
                meany += tempy[i];
            }
            meanx /= Samples;
            meany /= Samples;


            //Denominator for normalization
            sx = 0;
            sy = 0;
            for (i = 0; i < Samples; i++)
            {
                sx += (tempx[i] - meanx) * (tempx[i] - meanx);
                sy += (tempy[i] - meany) * (tempy[i] - meany);
            }
            denom = Math.Sqrt(sx * sy);

            //Calculate the correlation series
            for (int delay = -maxDelay; delay < maxDelay; delay++)
            {
                sxy = 0;
                for (i = 0; i < Samples; i++)
                {
                    j = i + delay;
                    //Normal Crosscorrelation
                    if (j < 0 || j >= Samples)
                        continue;
                    else
                        sxy += (x[i] - meanx) * (y[j] - meany);
                    /*
                    //Circular Crosscorrelation
                    if (j < 0 || j >= Samples)
                        sxy += (x[i] - meanx) * (-meany);
                    else
                        sxy += (x[i] - meanx) * (y[j] - meany);
                    */
                }

                r = sxy / denom;
                //Console.WriteLine("r: " + r);
                //Console.WriteLine("delay: " + delay);
                if (r > max)
                {
                    max = r;
                    BestDelay = delay;
                }
            }
            return BestDelay;
        }
        public double CrossCorrelationFFT(double[] x, double[] y, int Samples)
        {
            meanx = 0; meany = 0; max = 0; r = 0; BestDelay = 0;
            double[] tempx = x;
            double[] tempy = y;

            //Calculating the mean of both signals
            for (int i = 0; i < Samples; i++)
            {
                meanx += tempx[i];
                meany += tempy[i];
            }
            meanx /= Samples;
            meany /= Samples;


            //Denominator for normalization
            sx = 0;
            sy = 0;
            for (i = 0; i < Samples; i++)
            {
                sx += (tempx[i] - meanx) * (tempx[i] - meanx);
                sy += (tempy[i] - meany) * (tempy[i] - meany);
            }
            denom = Math.Sqrt(sx * sy);

            //Calculate the correlation series

                sxy = 0;
                for (i = 0; i < Samples; i++)
                {
                    
                    //Normal Crosscorrelation
                    if (i < 0 || i >= Samples)
                        continue;
                    else
                        sxy += (x[i] - meanx) * (y[j] - meany);
                    /*
                    //Circular Crosscorrelation
                    if (j < 0 || j >= Samples)
                        sxy += (x[i] - meanx) * (-meany);
                    else
                        sxy += (x[i] - meanx) * (y[j] - meany);
                    */
                }

                r = sxy / denom;
                return r;
              
        }
        public bool CheckTresholding(short[] Signal, int Thresh)
        {
            short[] Temp = Signal;
            for (int i = 0; i < Temp.Length; i++)
            {
                if (Temp[i] > Thresh)
                    return true;
            }
            return false;
        }
    }
}
