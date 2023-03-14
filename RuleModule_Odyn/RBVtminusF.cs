using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using MathNet.Numerics.LinearAlgebra;

namespace RuleModule_Odyn
{
    class RBVtminusF : FunctionExtension
    {
        double mV;
        double height;
        double sex;
        double flag;
        public RBVtminusF()
        {
            mV = Double.NaN;
            height = Double.NaN;
            sex = Double.NaN;
            flag = 1;
        }
        public RBVtminusF(double height, double sex, double mV,double flag=1)
        {
            this.mV = mV;
            this.height = height;
            this.sex = sex;
            this.flag = flag;
        }
        public int getParametersNumber()
        {
            return 4;
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
                    return "mV";
                case 3:
                    return "flag";

            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) height = argumentValue;
            if (argumentIndex == 1) sex = argumentValue;
            if (argumentIndex == 2) mV = argumentValue;
            if (argumentIndex == 3) flag = argumentValue;
        }
        public double calculate()
        {
            double PBW;

            Matrix<double> m = Matrix<double>.Build.Dense(11, 2);
            Vector<double> v = Vector<double>.Build.Dense(11);
            int[,] VtH = null;
            if (sex == 1)//MAN
            {
                VtH = StaticData.VtMan;
                PBW = 50 + 0.91 * (height - 152.4);
            }
            else
            {
                VtH = StaticData.VtWoman;
                PBW = 45.5 + 0.91 * (height - 152.4);
            }
            for (int i = 0; i < m.RowCount; i++)
            {
                m[i, 0] = 1;
                m[i, 1] = VtH[i, 1];
                v[i] = VtH[i, 4];
            }
            Matrix<double> r = m.Transpose() * m;
            r = r.Inverse() * m.Transpose();

            Vector<double> z = r * v;
            double vtNew= z[0] + z[1] * PBW;
            if (flag == 1)
                return vtNew;
            else
                return Math.Round(mV / vtNew);
        }
        public FunctionExtension clone()
        {
            return new RBVtminusF(height, sex, mV,flag);
        }
    
    }
}
