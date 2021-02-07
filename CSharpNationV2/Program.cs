using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace CSharpNationV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer analyzer = new Analyzer();
            analyzer.Enable = true;                       
            using (Visualizer visualizer = new Visualizer(800, 450, "CSharpNation_V2", analyzer))
            {
                visualizer.Run(60.0,60.0);
            }
            analyzer.Free();
        }
    }
}
