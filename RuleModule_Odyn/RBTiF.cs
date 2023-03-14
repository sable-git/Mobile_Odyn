using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBTiF : FunctionExtension
    {
        double ti;
        double te;
       

        public RBTiF()
        {
            ti= Double.NaN;
            te= Double.NaN;
        }
        public RBTiF(double x, double y)
        {
            this.ti= x;
            this.te = y;
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
                    return "ti";
                case 1:
                    return "te";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) ti = argumentValue;
            if (argumentIndex == 1) te = argumentValue;
        }
        public double calculate()
        {
            return (ti + te) / 3;
        }
        public FunctionExtension clone()
        {
            return new RBSpO2FiO2procSugenoF(ti, te);
        }
    

    }
}
