using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpNationV2
{
    public class Replay
    {
        public Replay(int size)
        {
            for(int i = 0; i < size; i++)
            {
                SpectrumReplay.Add(new List<float>());
            }
        }

        private List<List<float>> SpectrumReplay = new List<List<float>>();

        public void UpdateReplay(List<float> spectrum)
        {
            for (int i = SpectrumReplay.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    SpectrumReplay[i] = spectrum;
                    break;
                }

                SpectrumReplay[i] = new List<float>(SpectrumReplay[i - 1]);
            }
        }

        public List<float> GetReplay(int index)
        {
            if (index < SpectrumReplay.Count - 1)
            {
                return SpectrumReplay[index];
            }

            return null;
        }
    }
}
