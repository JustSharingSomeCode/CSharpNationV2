using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace CSharpNationV2
{
    class Visualizer : GameWindow
    {
        public Visualizer(int width, int height, string title, Analyzer _analyzer) : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 0), 24, 8, 4), title)
        {
            VSync = VSyncMode.On;
            analyzer = _analyzer;
            analyzer.multiplier = height / 2;

            increase = width / analyzer._lines;
            SpectrumData = analyzer.GetSpectrum();

            previousKeyboardState = Keyboard.GetState();
            actualKeyboardState = Keyboard.GetState();
        }

        private Analyzer analyzer;
        private List<double> SpectrumData;
        private List<int> peaks = new List<int>();
        private List<Vector2> peaksOnDegrees = new List<Vector2>();        

        private float increase = 0;
        private float actualPos = -1.0f;

        KeyboardState previousKeyboardState;
        KeyboardState actualKeyboardState;

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(new Color4(100, 100, 100, 255));

            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0f, Width, 0.0f, Height, 0.0f, 1.0f);

            increase = Width / analyzer._lines;

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            DrawCircle(Width / 2, Height / 2, Height / 4, Color.Red);
            DrawWave(Width / 2, Height / 2, Height / 4, Color.Black);

            for (int i = 0; i < SpectrumData.Count; i++)
            {
                GL.Begin(PrimitiveType.Quads);
                if (peaks.Contains(i))
                {
                    GL.Color3(0, 0, 0);                    
                }
                else
                {
                    GL.Color3(Color.Red);
                }                
                GL.Vertex2(actualPos, 0);
                GL.Vertex2(actualPos + increase, 0);
                GL.Vertex2(actualPos + increase, SpectrumData[i]);
                GL.Vertex2(actualPos, SpectrumData[i]);
                GL.End();

                if(peaks.Contains(i))
                {
                    DrawCircle(actualPos + increase / 2, SpectrumData[i], 5, Color.Green);
                }

                actualPos += increase;
            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {            
            SpectrumData = analyzer.GetSpectrum();
            peaks = WaveTools.FindPeaks(SpectrumData);
            PeaksToDegrees();

            previousKeyboardState = actualKeyboardState;
            actualKeyboardState = Keyboard.GetState();

            if (IsKeyPressed(Key.P))
            {
                analyzer.PauseCapture();
            }
            else if (IsKeyPressed(Key.R))
            {
                analyzer.ResumeCapture();
            }
            else if (IsKeyPressed(Key.C))
            {
                analyzer.ChangeDevice(Convert.ToInt32(Console.ReadLine()));
            }

            actualPos = 0;

            base.OnUpdateFrame(e);
        }

        private bool IsKeyPressed(Key key)
        {
            if(actualKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key))
            {
                return true;
            }

            return false;
        }

        private Vector2 CatmullRom(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 a = 2f * p1;
            Vector2 b = p2 - p0;
            Vector2 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector2 d = -p0 + 3f * p1 - 3f * p2 + p3;

            Vector2 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        private void DrawCircle(double X, double Y, double Radius, Color C)
        {
            GL.Color3(C);
            GL.Begin(PrimitiveType.Polygon);

            double rads, PosX, PosY;

            for (int i = 0; i <= 360; i += 2)
            {
                rads = Math.PI * i / 180;
                PosX = X + (Math.Sin(rads) * Radius);
                PosY = Y + (Math.Cos(rads) * Radius);

                GL.Vertex2(PosX, PosY);
            }

            GL.End();
        }

        private void PeaksToDegrees()
        {
            peaksOnDegrees.Clear();            

            for(int i = 0; i < peaks.Count; i++)
            {
                peaksOnDegrees.Add(new Vector2(peaks[i] * 180 / analyzer._lines, float.Parse(SpectrumData[peaks[i]].ToString())));                
            }
        }

        
        private void DrawWave(double X, double Y, double Radius, Color C)
        {
            GL.Color3(C);
            GL.Begin(PrimitiveType.LineLoop);

            double rads, PosX, PosY, spectrumRadius;

            for (int i = 0; i <= 180; i += 2)
            {
                spectrumRadius = Radius;

                for (int j = 0; j < peaksOnDegrees.Count; j++)
                {
                    if(peaksOnDegrees[j].X == i)
                    {
                        spectrumRadius = Radius + peaksOnDegrees[j].Y;
                    }
                }

                rads = Math.PI * i / 180;
                PosX = X + (Math.Sin(rads) * spectrumRadius);
                PosY = Y + (Math.Cos(rads) * spectrumRadius);

                GL.Vertex2(PosX, PosY);
            }

            GL.End();
        }
        
    }
}
