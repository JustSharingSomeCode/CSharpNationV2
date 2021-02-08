using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CSharpNationV2
{
    class Visualizer : GameWindow
    {
        public Visualizer(int width, int height, string title, Analyzer _analyzer) : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 0), 24, 8, 4), title)
        {
            VSync = VSyncMode.On;
            analyzer = _analyzer;
            analyzer.multiplier = height / 2;

            increase = 2.0f / analyzer._lines;
            SpectrumData = analyzer.GetSpectrum();

            previousKeyboardState = Keyboard.GetState();
            actualKeyboardState = Keyboard.GetState();
        }

        private Analyzer analyzer;
        private List<double> SpectrumData;

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

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color3(60, 60, 60);
            //GL.Vertex2(0.0f,0.5f);
            //GL.Vertex2(-0.5f, -0.5f);
            //GL.Vertex2(0.5f, -0.5f);
            //GL.End();            
            
            for (int i = 0; i < SpectrumData.Count; i++)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(0,0,0);
                GL.Vertex2(actualPos, -1.0f);
                GL.Vertex2(actualPos + increase, -1.0f);                
                GL.Vertex2(actualPos + increase, SpectrumData[i] / 255.0f);
                GL.Vertex2(actualPos, SpectrumData[i] / 255.0f);
                GL.End();

                actualPos += increase;
            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            SpectrumData = analyzer.GetSpectrum();
            
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

            actualPos = -1;

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
    }
}
