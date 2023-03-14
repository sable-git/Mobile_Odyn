using System;
using System.Collections.Generic;
using System.Text;

namespace GUI_mobile4
{
    public static class StaticElements
    {
        public static int fontSize=0;
        public static Dictionary<string, Dictionary<string, string>> dataFormat = new Dictionary<string, Dictionary<string, string>>
        {
            {"RR",new Dictionary<string, string>{{"unit","[f/min]"},{"format","{0:0}"},{"range","1-80"}} },
            {"ph",new Dictionary<string, string>{{"unit",""},{"format","{0:0.#}"},{"range","0-14"}} },
            {"SpO2",new Dictionary<string, string>{{"unit","[%]"},{"format","{0:0.#}"},{"range","10-100"}} },
            {"PaO2",new Dictionary<string, string>{{"unit","[mmHg]"},{"format","{0:0.#}"},{"range","10-200"}} },
            {"FiO2",new Dictionary<string, string>{{"unit","" },{"format","{0:0.##}"},{"range","0-1"}} },
            {"pCO2",new Dictionary<string, string>{{ "unit","[mmHg]"},{"format","{0:0.#}"},{"range","0-100"}} },
            {"PEEP",new Dictionary<string, string>{{"unit","[cmH2O]" },{"format","{0:0}"},{"range","0-40"}} },
            {"Ppeak",new Dictionary<string, string>{{"unit", "[cmH2O]" },{"format","{0:0.#}"},{"range","0-50"}} },
            {"Vt",new Dictionary<string, string>{{"unit","[ml]" },{"format","{0:0.#}"},{"range","0-1000"}} },
            {"Vt1kg",new Dictionary<string, string>{{"unit","[ml/kg]" },{"format","{0:0.#}"},{"range","0-1000"}} },
            {"Ti",new Dictionary<string, string>{{"unit",  "[s]" },{"format","{0:0.##}"},{"range","0.1-2"}} },
            {"TiTe",new Dictionary<string, string>{{"unit",  "" },{"format","{0:0.##}"},{"range","0-3"}} },
            {"height",new Dictionary<string, string>{{"unit",  "[cm]" },{"format","{0:0}"},{"range","100-220"}} }
        };

        static StaticElements()
        {
            var disp = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            /*double hh = disp.Height;
            if (disp.Height > disp.Width)
                hh = disp.Width;*/
            double hh = disp.Width;
            double ss=disp.Width/disp.Height;
            if (disp.Height > disp.Width)
            {
                hh = disp.Height;
                ss = disp.Height / disp.Width;
            }


            var pixels = hh / ss;

            var scale = disp.Density; ;
            var dps = Math.Sqrt((pixels - 0.5f) / scale);
            //fontSize = (int)(Math.Pow(dps, 2) / 14) - 10;
            //fontSize =(int) dps;
            fontSize=(int)(Math.Pow(dps, 2) / 26)-1;
        }
    }
}
