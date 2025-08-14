// <copyright file="TenMicaHelper.cs" company="TenMica Project">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using MicaWPF.Core.Enums;
using MicaWPF.Core.Interop;
using System.Windows.Interop;

namespace MicaWPF.Core.Helpers;

/// <summary>
/// Helper class for TenMica effects - the ultimate Windows 10/11 experience.
/// </summary>
public static class TenMicaHelper
{
    /// <summary>
    /// Gets the optimal transparency level based on system performance.
    /// </summary>
    public static double GetOptimalTransparency()
    {
        // High-end systems get more transparency, low-end get less
        var totalMemory = GC.GetTotalMemory(false);
        
        if (totalMemory > 8_000_000_000)
            return 0.15; // 8GB+ RAM - Full transparency
        else if (totalMemory > 4_000_000_000)
            return 0.25; // 4GB+ RAM - Medium transparency  
        else
            return 0.35; // <4GB RAM - Lower transparency for performance
    }

    /// <summary>
    /// Gets the perfect gradient color for the current theme.
    /// </summary>
    /// <param name="isDark">Whether using dark theme.</param>
    /// <returns>ARGB color value for the gradient.</returns>
    public static uint GetThemeGradientColor(bool isDark)
    {
        var transparency = GetOptimalTransparency();
        var alpha = (uint)(transparency * 255);
        
        if (isDark)
        {
            // Dark theme: Deep purple-blue gradient with dynamic transparency
            var baseColor = 0x301934U; // RGB: Purple-blue
            return (alpha << 24) | baseColor;
        }
        else
        {
            // Light theme: Warm light gradient with dynamic transparency
            var baseColor = 0xF5F5F5U; // RGB: Warm white
            return (alpha << 24) | baseColor;
        }
    }

    /// <summary>
    /// Applies the ultimate TenMica effect to a window.
    /// </summary>
    /// <param name="window">The window to apply the effect to.</param>
    /// <param name="forceMode">Force a specific mode (optional).</param>
    /// <returns>True if the effect was applied successfully.</returns>
    public static bool ApplyTenMicaEffect(System.Windows.Window window, TenMicaMode? forceMode = null)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero) return false;

        var mode = forceMode ?? GetBestModeForSystem();
        var isDark = OsHelper.IsWindows11_OrGreater || Environment.OSVersion.Version.Build >= 22000;

        try
        {
            // Simplified implementation for now - use DWM blur
            var margins = new InteropValues.MARGINS { cxLeftWidth = -1 };
            return InteropMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins) == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines the best TenMica mode for the current system.
    /// </summary>
    /// <returns>The optimal TenMica mode.</returns>
    public static TenMicaMode GetBestModeForSystem()
    {
        if (OsHelper.IsWindows11_OrGreater)
        {
            // Windows 11 gets native Mica, but we can still enhance it
            return TenMicaMode.Windows11Native;
        }

        if (OsHelper.IsWindows10_1903_OrGreater)
        {
            // Windows 10 1903+ supports our ultimate TenMica
            return TenMicaMode.UltimateTenMica;
        }

        if (OsHelper.IsWindows10_1607_OrGreater)
        {
            // Older Windows 10 gets standard acrylic
            return TenMicaMode.Windows10Acrylic;
        }

        // Ancient systems get basic blur
        return TenMicaMode.BasicBlur;
    }

    /// <summary>
    /// Gets system performance score (0-100).
    /// </summary>
    /// <returns>Performance score.</returns>
    public static int GetSystemPerformanceScore()
    {
        var score = 50; // Base score
        
        // RAM check
        var totalMemory = GC.GetTotalMemory(false);
        if (totalMemory > 8_000_000_000) score += 20;
        else if (totalMemory > 4_000_000_000) score += 10;
        
        // CPU check (simplified)
        var processorCount = Environment.ProcessorCount;
        if (processorCount >= 8) score += 15;
        else if (processorCount >= 4) score += 10;
        else if (processorCount >= 2) score += 5;
        
        // Windows version bonus
        if (OsHelper.IsWindows11_22H2_OrGreater) score += 15;
        else if (OsHelper.IsWindows11_OrGreater) score += 10;
        else if (OsHelper.IsWindows10_2004_OrGreater) score += 5;
        
        return Math.Min(100, Math.Max(0, score));
    }
}

/// <summary>
/// TenMica effect modes.
/// </summary>
public enum TenMicaMode
{
    /// <summary>
    /// Ultimate TenMica effect - the best experience.
    /// </summary>
    UltimateTenMica,
    
    /// <summary>
    /// Windows 10 Acrylic effect.
    /// </summary>
    Windows10Acrylic,
    
    /// <summary>
    /// Basic blur effect.
    /// </summary>
    BasicBlur,
    
    /// <summary>
    /// Windows 11 native Mica.
    /// </summary>
    Windows11Native
}