using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBPaO2FiO2PEEPminusF : FunctionExtension
    {
        protected double fiO2;
        protected double peep;
        protected double flag;
        protected double[,] tg = new double[,]{ { 30, 30, 30, 30, 30, 40, 40, 50, 50, 50, 60, 70, 80, 80, 90, 100, 100 },
    { 5,8,10,12,14,14,16,16,18,20,20,20,20,22,22,22,24}}; 

        public RBPaO2FiO2PEEPminusF()
        {
            fiO2 = Double.NaN;
            peep = Double.NaN;
        }
        public RBPaO2FiO2PEEPminusF(double fio2, double peep,double flag)
        {
            this.fiO2 = fio2;
            this.peep = peep;
            this.flag = flag;
        }
        public int getParametersNumber()
        {
            return 3;
        }
        public string getParameterName(int parameterIndex)
        {
            switch (parameterIndex)
            {
                case 0:
                    return "fio2";
                case 1:
                    return "peep";
                case 2:
                    return "flag";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) fiO2 = argumentValue;
            if (argumentIndex == 1) peep = argumentValue;
            if (argumentIndex == 2) flag = argumentValue;
        }
        public double calculate()
        {
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < tg.GetLength(1); i++)
            {
                if (tg[0, i] < fiO2)
                    index1 = i;
                if (tg[1, i] < peep)
                    index2 = i;
            }
            double fio2New = 0;
            double peepNew = 0;
            if (index1 == index2)
            {
                if (index1 > 0)
                {
                    fio2New = tg[0, index1 - 1];
                    peepNew = tg[1, index1 - 1];
                }
                else
                {
                    fio2New = fiO2;
                    peepNew = peep;
                }
            }
            else
            {
                int index = Math.Min(index1, index2);
                if (index > 0)
                {
                    fio2New = tg[0, index];
                    peepNew = tg[1, index];
                }
                else
                {
                    fio2New = fiO2;
                    peepNew = peep;
                }
            }
            if (flag == 1)
                return fio2New;
            return peepNew;
 
        }
        public FunctionExtension clone()
        {
            
            return new RBPaO2FiO2PEEPminusF(fiO2, peep,flag);
        }

    }
}
