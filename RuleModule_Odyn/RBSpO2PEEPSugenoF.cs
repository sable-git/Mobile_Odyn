using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBSpO2PEEPSugenoF : FunctionExtension
    {
        double peep;
        double spo2;
       int[,] SpO2MF = new int[,] { { 60, 3 }, { 65, 3 }, { 70, 3 }, { 75, 3 }, { 80, 3 }, { 85, 3 }, { 90, 3 }, { 95, 3 } };
        double[,] R = new double[,] { { 0, 2/*SpO2=60*/}, { 1, 2 /*SpO2=65*/}, { 2, 1.5 }, { 3, 1 }, { 4, 0.5 }, { 5, 0.5 }, { 6, 0 }, { 7, 0 } };
      
        public RBSpO2PEEPSugenoF()
        {
            peep = Double.NaN;
            spo2 = Double.NaN;
        }
        public RBSpO2PEEPSugenoF(double x, double y)
        {
            this.peep = y;
            this.spo2 = x;
        }
        public int getParametersNumber()
        {
            return 2;
        }
        public string getParameterName(int parameterIndex)
        {
            switch(parameterIndex)
            {
                case 0:
                    return "spo2";
                case 1:
                    return "peep";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) spo2 = argumentValue;
            if (argumentIndex == 1) peep = argumentValue;
        }
        public double calculate()
        {
            double s = 0;
            double num = 0;
            for(int i=0;i<R.GetLength(0);i++)
            {
                int index = (int)R[i, 0];
                double y = Math.Exp(-Math.Pow(spo2 - SpO2MF[index, 0], 2) / (2 * Math.Pow(SpO2MF[index, 1],2)));
                num += y * R[i, 1];
                s += y;
            }
            return peep+num/s;
        }
        public FunctionExtension clone()
        {
            return new RBSpO2PEEPSugenoF(spo2, peep);
        }
    }
}
