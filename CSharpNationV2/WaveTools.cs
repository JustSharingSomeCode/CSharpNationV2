using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpNationV2
{
    public class WaveTools
    {        
        public static List<int> FindPeaks(List<double> spectrum)
        {
            List<int> peaks = new List<int>();
            bool IsPeak = false;

            for(int i = 0; i < spectrum.Count; i++)
            {
                for (int p = i - 4; p <= i + 4; p++)
                {
                    if(p == i)
                    {
                        continue;
                    }

                    if (!(spectrum[i] > spectrum[ClampInt(p, 0, spectrum.Count - 1)]))
                    {
                        IsPeak = false;
                        break;
                    }

                    IsPeak = true;
                }

                if(IsPeak)
                {
                    peaks.Add(i);
                }
            }

            return peaks;
        }

        private static int ClampInt(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
