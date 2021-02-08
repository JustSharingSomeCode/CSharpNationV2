using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace CSharpNationV2
{
    public class WaveTools
    {        
        public static List<int> FindPeaks(List<float> spectrum)
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

        public static List<Vector2> PeaksToDegrees(List<float> spectrumData, List<int> peaks)
        {
            List<Vector2> degrees = new List<Vector2>();
            float left, right;
            float leftPercentaje, rightPercentaje;

            for (int i = 0; i < peaks.Count; i++)
            {
                left = spectrumData[ClampInt(peaks[i] - 1, 0, spectrumData.Count - 1)];
                right = spectrumData[ClampInt(peaks[i] + 1, 0, spectrumData.Count - 1)];

                leftPercentaje = 1.0f - (spectrumData[peaks[i]] / (spectrumData[peaks[i]] + left));
                rightPercentaje = 1.0f - (spectrumData[peaks[i]] / (spectrumData[peaks[i]] + right));

                if (left > right)
                {                    
                    degrees.Add(new Vector2((peaks[i] - Math.Abs(leftPercentaje - rightPercentaje)) * 180.0f / spectrumData.Count, spectrumData[peaks[i]]));
                }
                else
                {                    
                    degrees.Add(new Vector2((peaks[i] + Math.Abs(leftPercentaje - rightPercentaje)) * 180.0f / spectrumData.Count, spectrumData[peaks[i]]));
                }
            }

            return degrees;
        }

        private static int ClampInt(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static float ClampFloat(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
