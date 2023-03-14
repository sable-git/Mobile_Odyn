using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBVtVt1kgF : FunctionExtension
    {
        protected double height;
        protected double sex;
        protected double Vt;
        public RBVtVt1kgF()
        {
            height = Double.NaN;
            sex = Double.NaN;
            Vt = Double.NaN;
        }
        public RBVtVt1kgF(double height, double sex, double Vt)
        {
            this.height = height;
            this.sex = sex;
            this.Vt = Vt;
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
                    return "height";
                case 1:
                    return "sex";
                case 2:
                    return "Vt";
            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) height = argumentValue;
            if (argumentIndex == 1) sex = argumentValue;
            if (argumentIndex == 2) Vt = argumentValue;
        }
    
        public  virtual double calculate()
        {
            double PBW;

            if (sex == 1)
                PBW = 50 + 0.91 * (height - 152.4);
            else
                PBW = 45.5 + 0.91 * (height - 152.4);

            return  Math.Round(Vt / PBW);

        }
        public  virtual FunctionExtension clone()
        {
            return new RBVtVt1kgF(height, sex, Vt);
        }

    }
}
