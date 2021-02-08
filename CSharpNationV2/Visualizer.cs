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

            increase = width / analyzer._lines;
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

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0f, Width, 0.0f, Height, 0.0f, 1.0f);

            increase = Width / analyzer._lines;

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);                                 
            
            for (int i = 0; i < SpectrumData.Count; i++)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(0,0,0);
                GL.Vertex2(actualPos, 0);
                GL.Vertex2(actualPos + increase, 0);                
                GL.Vertex2(actualPos + increase, SpectrumData[i]);
                GL.Vertex2(actualPos, SpectrumData[i]);
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
    }
}
