using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using MathNet.Numerics.LinearAlgebra;

namespace RuleModule_Odyn
{
    class RBVtInitF : FunctionExtension
    {
        double Vt1kg;
        double height;
        double sex;
      
        public RBVtInitF()
        {
            height = Double.NaN;
            sex = Double.NaN;
            Vt1kg = Double.NaN;
        }
        public RBVtInitF(double height, double sex, double Vt1kg)
        {
            this.Vt1kg = Vt1kg;
            this.height = height;
            this.sex = sex;
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
                    return "Vt1kg";

            }
            return "";
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) height = argumentValue;
            if (argumentIndex == 1) sex = argumentValue;
            if (argumentIndex == 2) Vt1kg = argumentValue;
        }
        public double calculate()
        {
            double PBW;

            Matrix<double> m = Matrix<double>.Build.Dense(11, 2);
            Vector<double> v = Vector<double>.Build.Dense(11);
            int[,] VtH;
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
                v[i] = VtH[i, (int)Vt1kg - 2];
            }
            Matrix<double> r = m.Transpose() * m;
            r = r.Inverse() * m.Transpose();

            Vector<double> z = r * v;
            return z[0] + z[1] * PBW;
        }
        public FunctionExtension clone()
        {
            return new RBVtInitF(height, sex, Vt1kg);
        }
    }
}
