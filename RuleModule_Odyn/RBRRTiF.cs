using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBRRTiF : FunctionExtension
    {
        double RR;

        public RBRRTiF()
        {
            RR = Double.NaN;
        }
        public RBRRTiF(double RR)
        {
            this.RR = RR;
        }
        public int getParametersNumber()
        {
            return 1;
        }
        public string getParameterName(int parameterIndex)
        {
            switch (parameterIndex)
            {
                case 0:
                    return "RR";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) RR = argumentValue;
        }
        public double calculate()
        {
            double Tcycl =  60/RR;
            return Tcycl/3;
        }
        public FunctionExtension clone()
        {
            return new RBRRTiF(RR);
        }
    
    }
}
