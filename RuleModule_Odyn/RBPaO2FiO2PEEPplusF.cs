using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace RuleModule_Odyn
{
    class RBPaO2FiO2PEEPplusF:RBPaO2FiO2PEEPminusF
    {
        public RBPaO2FiO2PEEPplusF() : base() { }
        public RBPaO2FiO2PEEPplusF(double fio2, double peep, double flag) : base(fio2, peep, flag) { }
        
        public new double calculate()
        {
            
            int index1 = -1;
            int index2 = -1;
            for (int i = 0; i < tg.GetLength(1); i++)
            {
                if (tg[0, i] < fiO2 && index1==-1)
                    index1 = i;
                if (tg[1, i] < peep && index2==-1)
                    index2 = i;
            }
            double fio2New = 0;
            double peepNew = 0;
            if (index1 == index2)
            {
                if (index1 < tg.GetLength(1)-1)
                {
                    fio2New = tg[0, index1 + 1];
                    peepNew = tg[1, index1 + 1];
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
                if (index < tg.GetLength(1)-1)
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
        public new FunctionExtension clone()
        {
            return new RBPaO2FiO2PEEPplusF(fiO2, peep, flag);
        }
    }
}
