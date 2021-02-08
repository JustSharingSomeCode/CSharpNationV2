using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CSharpNationV2
{
    public class Wave
    {
        public Wave(Color c, List<float> data)
        {
            waveColor = c;
            spectrumData = data;
        }

        private Color waveColor;
        private List<float> spectrumData = new List<float>();
        private List<Vector2> peaksOnDegrees = new List<Vector2>();

        public void UpdateSpectrumData(List<float> data)
        {
            spectrumData = data;
            UpdatePeaks();
        }
        
        public void UpdatePeaks()
        {            
            peaksOnDegrees = WaveTools.PeaksToDegrees(spectrumData, WaveTools.FindPeaks(spectrumData));
        }

        public void DrawWave(double X, double Y, double Radius)
        {
            GL.Color3(waveColor);
            GL.Begin(PrimitiveType.LineLoop);

            double rads, PosX, PosY, spectrumRadius;

            for (float i = 0; i <= 180.0f; i += 0.1f)
            {
                spectrumRadius = Radius;

                for (int j = 0; j < peaksOnDegrees.Count; j++)
                {
                    if (peaksOnDegrees[j].X >= i && peaksOnDegrees[j].X < i + 0.1f)
                    {
                        spectrumRadius = Radius + peaksOnDegrees[j].Y;
                    }
                }

                rads = Math.PI * i / 180.0f;
                PosX = X + (Math.Sin(rads) * spectrumRadius);
                PosY = Y + (Math.Cos(rads) * spectrumRadius);

                GL.Vertex2(PosX, PosY);
            }            

            GL.End();
        }
    }
}
