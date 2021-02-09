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
        public Wave(Color c, List<float> data, int width, int height, float _influence)
        {
            waveColor = c;
            spectrumData = data;

            UpdateWindowSize(width, height);
            influence = _influence;
        }

        private int Width, Height;
        private int X, Y;
        private float Radius;

        private Color waveColor;
        private List<float> spectrumData = new List<float>();
        private List<Vector2> peaksOnDegrees = new List<Vector2>(); //x = degrees, y = spectrumValue
        private List<Vector2> catmullRomPoints = new List<Vector2>();        

        private float influence = 20;

        public void UpdateWindowSize(int width, int height)
        {
            Width = width;
            Height = height;

            Radius = Height / 4;

            X = Width / 2;
            Y = Height / 2;
        }

        public void UpdateSpectrumData(List<float> data)
        {
            spectrumData = data;
            UpdatePeaks();
        }
        
        private void UpdatePeaks()
        {            
            peaksOnDegrees = WaveTools.PeaksToDegrees(spectrumData, WaveTools.FindPeaks(spectrumData));
            UpdatePoints();
        }

        private void UpdatePoints()
        {
            catmullRomPoints.Clear();
            for(int i = 0; i < peaksOnDegrees.Count; i++)
            {                

                float peakDegree = peaksOnDegrees[i].X;
                float peakForce = peaksOnDegrees[i].Y;

                catmullRomPoints.Add(WaveTools.DegreeToVector(X, Y, peakDegree - influence, Radius));

                for (float j = 0; j <= 1.0f; j += 0.05f)
                {
                    catmullRomPoints.Add(WaveTools.CatmullRom(j, WaveTools.DegreeToVector(X, Y, peakDegree - influence, Radius),
                        WaveTools.DegreeToVector(X, Y, peakDegree - (influence / 2.0f), Radius),
                        WaveTools.DegreeToVector(X, Y, peakDegree, Radius + peakForce),
                        WaveTools.DegreeToVector(X, Y, peakDegree + (influence / 2.0f), Radius)));
                }

                for (float j = 0; j <= 1.0f; j += 0.05f)
                {
                    catmullRomPoints.Add(WaveTools.CatmullRom(j, WaveTools.DegreeToVector(X, Y, peakDegree - (influence / 2.0f), Radius),
                        WaveTools.DegreeToVector(X, Y, peakDegree, Radius + peakForce),
                        WaveTools.DegreeToVector(X, Y, peakDegree + (influence / 2.0f), Radius),
                        WaveTools.DegreeToVector(X, Y, peakDegree + influence, Radius)));
                }
                catmullRomPoints.Add(WaveTools.DegreeToVector(X, Y, peakDegree + influence, Radius));
            }
        }

        public void DrawWave(double X, double Y, double Radius)
        {            
            for (int i = 0; i < catmullRomPoints.Count - 1; i++)
            {
                GL.Color3(waveColor);
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Vertex2(catmullRomPoints[i]);
                GL.Vertex2(catmullRomPoints[i + 1]);
                GL.Vertex2(X, Y);
                GL.End();
            }                       
        }       
    }
}
