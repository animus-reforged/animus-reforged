using System.Runtime.InteropServices;
using AnimusReforged.Logging;

namespace AnimusReforged.Utilities;

/// <summary>
/// Provides functionality for retrieving display resolution information on Windows systems.
/// </summary>
public class DisplayHelper
{
    /// <summary>
    /// Represents the device mode structure used by Windows API for display settings.
    /// </summary>
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

    /// <summary>
    /// Imports the EnumDisplaySettings function from user32.dll to enumerate display settings.
    /// </summary>
    /// <param name="lpszDeviceName">Pointer to a null-terminated string that specifies the display device.</param>
    /// <param name="iModeNum">Index of the display mode to query.</param>
    /// <param name="lpDevMode">Reference to a DEVMODE structure that receives information about the display mode.</param>
    /// <returns>True if the function succeeds, false otherwise.</returns>
    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    /// <summary>
    /// Retrieves all supported screen resolutions for the primary display device on Windows.
    /// </summary>
    /// <returns>A tuple containing lists of unique widths and heights supported by the display.</returns>
    /// <remarks>
    /// Currently only supports Windows platforms. On other platforms, returns empty lists.
    /// </remarks>
    public static (List<int> Widths, List<int> Heights) GetSupportedResolutions()
    {
        Logger.Info<DisplayHelper>("Starting to fetch supported display resolutions");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Logger.Debug<DisplayHelper>("Running on Windows platform, proceeding with EnumDisplaySettings");

            try
            {
                HashSet<(int Width, int Height)> resolutionPairs = [];
                int totalModesChecked = 0;

                DEVMODE devMode = new DEVMODE();
                int modeIndex = 0;

                while (EnumDisplaySettings(null, modeIndex, ref devMode))
                {
                    Logger.Trace<DisplayHelper>($"Found resolution mode {modeIndex}: {devMode.dmPelsWidth}x{devMode.dmPelsHeight}");
                    resolutionPairs.Add((devMode.dmPelsWidth, devMode.dmPelsHeight));
                    modeIndex++;
                    totalModesChecked++;
                }

                Logger.Info<DisplayHelper>($"Found {totalModesChecked} total display modes, {resolutionPairs.Count} unique resolutions");

                // Split into unique widths and heights
                List<int> widths = resolutionPairs.Select(r => r.Width)
                    .Distinct()
                    .OrderBy(w => w)
                    .ToList();

                List<int> heights = resolutionPairs.Select(r => r.Height)
                    .Distinct()
                    .OrderBy(h => h)
                    .ToList();

                Logger.Info<DisplayHelper>($"Returning {widths.Count} unique widths and {heights.Count} unique heights");
                Logger.Trace<DisplayHelper>($"Unique widths: [{string.Join(", ", widths)}]");
                Logger.Trace<DisplayHelper>($"Unique heights: [{string.Join(", ", heights)}]");

                return (widths, heights);
            }
            catch (Exception ex)
            {
                Logger.Error<DisplayHelper>("Failed to fetch supported display resolutions");
                Logger.LogExceptionDetails<DisplayHelper>(ex);
                return ([], []);
            }
        }
        else
        {
            Logger.Warning<DisplayHelper>("Non-Windows platform detected, returning empty resolution lists");
            // Fallback (no Linux/macOS implementation yet)
            return ([], []);
        }
    }
}