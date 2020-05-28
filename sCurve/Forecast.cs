using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sCurve
{
    class Forecast

    {
        private int iCurrentMonth = 1;
        private double dCurrentPercentageTotal = 0;
        private double dPreviousPercentageTotal = 0;
        public static int duration(DateTime from, DateTime to)
        {
            if (from > to) return 0;
            return Math.Abs((to.Year - from.Year) * 12 + to.Month - from.Month)+1;
        }

        public double dNormalPercantage(int totalMonths, int PValue)
        {

            if (this.iCurrentMonth > totalMonths) return 0;
            
            try
            {
                double T = (Convert.ToDouble(this.iCurrentMonth) / Convert.ToDouble(totalMonths))*100;
                double ret;

                ret = (Math.Pow(3, T) / 2);
                ret *= Math.Sin((Math.PI / 200) * (100 - T));
                ret *= Math.Sin((Math.PI * T) / 100);
                ret *= Math.Log((T + 50) / (PValue + T));
                ret -= (2 * Math.Pow(T, 3)) / Math.Pow(100,2);
                ret += (3 * Math.Pow(T, 2)) / 100;

                this.iCurrentMonth++;
                this.dPreviousPercentageTotal = this.dCurrentPercentageTotal;
                ret = Math.Round(ret, 2) - this.dCurrentPercentageTotal;
                this.dCurrentPercentageTotal = this.dPreviousPercentageTotal + Math.Round(ret, 2);

                return Math.Round(ret,2);
            }
            catch
            {
                return 0;
            }

        }

        public double Value(double dPercent, double iValue)
        {
            return Math.Round((dPercent / 100) * iValue,2);
        }

        private double Loading(int Step, int span, double degree)
        {
            int iDelta = 0;
            double Li = 0;
            double dRatio = 0;
            double dSum = 0;
            double dLoading = 0;
            if (Step == span)
            {
                return Math.Round(100 - this.dCurrentPercentageTotal, 2);
            }
            span += 1;

            for (int i =0; i < span; i++)
            {
                if(i != 0 && i != span)
                {
                    iDelta = span - i;
                    dRatio = Math.PI * iDelta / span;
                    Li = Math.Sin(dRatio + (Math.Sin(dRatio) / degree));
                }
                else
                {
                    Li = 0;
                }

                dSum += Li;
                if (i == Step) dLoading = Li;
            }
            this.dPreviousPercentageTotal = this.dCurrentPercentageTotal;
            this.dCurrentPercentageTotal = this.dPreviousPercentageTotal + Math.Round((dLoading / dSum) * 100, 2);
            return Math.Round((dLoading / dSum) * 100, 2);

        }

        public double FrontLoading(int Step, int span, int degree)
        {
            return Loading(Step+1, span, degree * -1);
        }

        public double FrontLoading(int Step, int span)
        {
            return Loading(Step+1, span, -1);
        }

        public double BackLoading(int Step, int span)
        {
            return Loading(Step+1, span, 1);
        }

        public double BackLoading(int Step, int span, int degree)
        {
            return Loading(Step + 1, span, degree);
        }

        public double Linear(int Step, int duration)
        {
            if (Step == duration-1)
            {
                return Math.Round(100 - this.dCurrentPercentageTotal, 2);
            }
            this.dPreviousPercentageTotal = this.dCurrentPercentageTotal;
            this.dCurrentPercentageTotal = this.dPreviousPercentageTotal + Math.Round((1 / Convert.ToDouble(duration)) * 100, 2);
            return Math.Round((1 / Convert.ToDouble(duration)) * 100, 2);
        }

        public double load(int step, int duration, double value, string sType, int iType)
        {
            if (iType == 1)
            {
                switch (sType)
                {
                    case "Back Loaded":
                        return this.Value(this.BackLoading(step, duration), value);
                    case "Front Loaded":
                        return this.Value(this.FrontLoading(step, duration), value);
                    case "SCurve":
                        return this.Value(this.dNormalPercantage(duration, 50), value);
                    default:
                        return load(step, duration, value, iType);
                }
            }

            switch (sType)
            {
                case "Back Loaded":
                    return this.BackLoading(step, duration);
                case "Front Loaded":
                    return this.FrontLoading(step, duration);
                case "SCurve":
                    return this.dNormalPercantage(duration, 50);
                default:
                    return load(step, duration, value, iType);
            }
        }

        public double load(int step, int duration, double value, int iType)
        {
            if (iType == 1) return this.Value(Linear(step, duration), value);
            return Linear(step, duration);
        }

    }


}
