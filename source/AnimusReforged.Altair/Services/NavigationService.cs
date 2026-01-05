using System;
using AnimusReforged.Altair.Views.Pages;
using AnimusReforged.Logging;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;

namespace AnimusReforged.Altair.Services;

public class NavigationService
{
    // Properties
    /// <summary>
    /// Content Frame where all Avalonia Pages are loaded
    /// </summary>
    private Frame? _contentFrame;

    /// <summary>
    /// NavigationView used by the NavigationService to show different Avalonia Pages in ContentFrame
    /// </summary>
    private NavigationView? _navigationView;

    /// <summary>
    /// Tag of the currently shown Avalonia Page
    /// </summary>
    private string? _currentPageTag;

    public string? CurrentPageTag => _currentPageTag;

    /// <summary>
    /// Event signalizing that the service navigated to the selected Avalonia Page
    /// </summary>
    public event EventHandler<string>? Navigated;

    // Functions
    /// <summary>
    /// Sets the ContentFrame used by the NavigationService
    /// </summary>
    /// <param name="frame">Frame the NavigationService will use</param>
    public void SetContentFrame(Frame frame)
    {
        Logger.Debug<NavigationService>($"Setting content frame");
        _contentFrame = frame;
        Logger.Debug<NavigationService>($"Content frame set");
    }

    /// <summary>
    /// Sets the NavigationView used by the NavigationService
    /// </summary>
    /// <param name="navigationView">NavigationView the NavigationService will use</param>
    public void SetNavigationView(NavigationView navigationView)
    {
        Logger.Debug<NavigationService>($"Setting navigation view");
        _navigationView = navigationView;
        Logger.Debug<NavigationService>($"Navigation view set");
    }

    /// <summary>
    /// Extracts tag from the NavigationViewItem and navigates to it
    /// </summary>
    /// <param name="navigationViewItem"></param>
    /// <param name="contentFrame"></param>
    public void Navigate(NavigationViewItem navigationViewItem, Frame? contentFrame = null)
    {
        string tag = navigationViewItem.Tag?.ToString() ?? string.Empty;
        Logger.Debug<NavigationService>($"Navigate called to: {tag}");
        NavigateToTag(tag, contentFrame);
    }

    /// <summary>
    /// Navigate to specific page using its tag
    /// </summary>
    /// <param name="tag">Tag of the page we're trying to navigate to</param>
    /// <param name="contentFrame">ContentFrame where we want to show the Avalonia Page, optional param and uses _contentFrame if it's null</param>
    public void NavigateToTag(string tag, Frame? contentFrame = null)
    {
        Frame? frame = contentFrame ?? _contentFrame;

        if (frame == null)
        {
            Logger.Warning<NavigationService>($"Cannot navigate because we're missing the ContentFrame");
            return;
        }

        _currentPageTag = tag;
        Logger.Info<NavigationService>($"Navigating to {tag}");
        switch (tag)
        {
            case "Welcome":
                frame.Navigate(typeof(WelcomePage), null, new EntranceNavigationTransitionInfo());
                break;
            case "Play":
                frame.Navigate(typeof(DefaultPage), null, new DrillInNavigationTransitionInfo());
                break;
            case "Manage":
                break;
            case "Settings":
                break;
            case "Credits":
                frame.Navigate(typeof(CreditsPage), null, new EntranceNavigationTransitionInfo());
                break;
            case "Donate":
                break;
            default:
                frame.Navigate(typeof(DefaultPage), null, null);
                break;
        }

        Navigated?.Invoke(this, tag);
        Logger.Info<NavigationService>($"Navigation to {tag} completed");
    }
}