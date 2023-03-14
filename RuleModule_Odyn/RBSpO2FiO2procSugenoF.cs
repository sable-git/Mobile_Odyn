using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBSpO2FiO2procSugenoF : FunctionExtension
    {
        double fiO2;
        double spo2;
        int[,] SpO2MF = new int[,] {{ 80, 3 }, { 85, 3 }, { 90, 3 }, { 95, 3 },{ 100, 3 } };
        double[,] R = new double[,] {{ 0, 0}, { 1, 0 }, { 2, 0 }, { 3, 5 },{ 4, 10 } };

        public RBSpO2FiO2procSugenoF()
        {
            fiO2 = Double.NaN;
            spo2 = Double.NaN;
        }
        public RBSpO2FiO2procSugenoF(double x, double y)
        {
            this.spo2 = x;
            this.fiO2 = y;
        }
        public int getParametersNumber()
        {
            return 2;
        }
        public string getParameterName(int parameterIndex)
        {
            switch (parameterIndex)
            {
                case 0:
                    return "spo2";
                case 1:
                    return "fio2";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) spo2 = argumentValue;
            if (argumentIndex == 1) fiO2 = argumentValue;
        }
        public double calculate()
        {
            double s = 0;
            double num = 0;
            for (int i = 0; i < R.GetLength(0); i++)
            {
                int index = (int)R[i, 0];
                double xx = Math.Pow(spo2 - SpO2MF[index, 0], 2);
                double y = Math.Exp(-xx / (2 * Math.Pow(SpO2MF[index, 1], 2)));
                num += y * R[i, 1];
                s += y;
            }
            double res= fiO2 * (100 - num / s) / 100;

            if (res < 20)
                res = 20;
            return res;
        }
        public FunctionExtension clone()
        {
            return new RBSpO2FiO2procSugenoF(fiO2, spo2);
        }
    }
}
