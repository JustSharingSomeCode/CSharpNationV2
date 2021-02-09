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

        public static Vector2 DegreeToVector(double X, double Y, float degree, float radius)
        {
            double rads, PosX, PosY;            

            rads = Math.PI * degree / 180;
            PosX = X + (Math.Sin(rads) * radius);
            PosY = Y + (Math.Cos(rads) * radius);

            return new Vector2(float.Parse(PosX.ToString()), float.Parse(PosY.ToString()));
        }

        public static Vector2 CatmullRom(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 a = 2f * p1;
            Vector2 b = p2 - p0;
            Vector2 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector2 d = -p0 + 3f * p1 - 3f * p2 + p3;

            Vector2 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
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
