using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBVt1kgVF : RBVtVt1kgF
    {
        public RBVt1kgVF():base()
        {
        }
        public RBVt1kgVF(double height, double sex,double Vt1kg): base(height,sex,Vt1kg)
        {
        }
        public override double calculate()
        {
            double PBW;
            if(sex==1)
                PBW = 50 + 0.91 * (height - 152.4);
            else
                PBW = 45.5 + 0.91 * (height - 152.4);
            
            return Vt * PBW;

        }
        public override FunctionExtension clone()
        {
            return new RBVt1kgVF(height, sex,Vt);
        }
    
    }
}
