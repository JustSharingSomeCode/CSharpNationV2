using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace CSharpNationV2
{
    public class Analyzer
    {
        private bool _enable;               //enabled status        
        public float[] _fft;               //buffer for fft data       
        private WASAPIPROC _process;        //callback function to obtain data            
        public List<double> _spectrumdata;   //spectrum data buffer        
        public List<string> _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex = 0;           //used device index      

        public float multiplier = 1;
        public int _lines = 50;

        public Analyzer()
        {
            _fft = new float[8192];
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<double>();
            _devicelist = new List<string>();
            _initialized = false;

            InitializeSpectrumData();

            Init(0);
        }
        
        private void InitializeSpectrumData()
        {
            for(int i = 0; i < _lines; i++)
            {
                _spectrumdata.Add(0);
            }
        }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                if (value)
                {
                    if (!_initialized)
                    {
                        var array = (_devicelist[0] as string).Split(' ');
                        devindex = Convert.ToInt32(array[0]);
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                        if (!result)
                        {
                            var error = Bass.BASS_ErrorGetCode();
                            //MessageBox.Show(error.ToString());
                        }
                        else
                        {
                            _initialized = true;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else BassWasapi.BASS_WASAPI_Stop(true);
                System.Threading.Thread.Sleep(500);
            }
        }

        private void Init(int Channel)
        {
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    _devicelist.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            //_devicelist.SelectedIndex = Channel;
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }

        public List<double> GetSpectrum()
        {
            //_spectrumdata.Clear();
            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT8192);
            if (ret < -1) { return _spectrumdata; }
            else
            {
                int x;
                double y;
                int b0 = 0;

                for (x = 0; x < _lines; x++)
                {
                    double peak = 0;
                    int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                    if (b1 > 1023) b1 = 1023;
                    if (b1 <= b0) b1 = b0 + 1;
                    for (; b0 < b1; b0++)
                    {
                        if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                    }

                    y = peak * multiplier;
                    if (y < 0) y = 0;
                    //_spectrumdata.Add(y);
                    _spectrumdata[x] = y;
                }

                return _spectrumdata;
            }
        }

        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }
    }
}
