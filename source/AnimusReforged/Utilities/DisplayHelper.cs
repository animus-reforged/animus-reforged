using System.Runtime.InteropServices;

namespace AnimusReforged.Utilities;

public static class DisplayHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;

        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    public static (List<int> Widths, List<int> Heights) GetSupportedResolutions()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            HashSet<(int Width, int Height)> resolutionPairs = new();

            DEVMODE devMode = new DEVMODE();
            int modeIndex = 0;

            while (EnumDisplaySettings(null, modeIndex, ref devMode))
            {
                resolutionPairs.Add((devMode.dmPelsWidth, devMode.dmPelsHeight));
                modeIndex++;
            }

            // Split into unique widths and heights
            List<int> widths = resolutionPairs.Select(r => r.Width)
                .Distinct()
                .OrderBy(w => w)
                .ToList();

            List<int> heights = resolutionPairs.Select(r => r.Height)
                .Distinct()
                .OrderBy(h => h)
                .ToList();

            return (widths, heights);
        }
        else
        {
            // Fallback (no Linux/macOS implementation yet)
            return (new List<int>(), new List<int>());
        }
    }
}