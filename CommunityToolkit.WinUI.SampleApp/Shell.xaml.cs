// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.SampleApp.Pages;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace CommunityToolkit.WinUI.SampleApp
{
    public sealed partial class Shell
    {
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public static Shell Current { get; private set; }

        public Shell()
        {
            InitializeComponent();
            Current = this;
            NavigationFrame.NavigationFailed += (s, args) => Console.WriteLine($"Navigation failed {args.SourcePageType}: {args.Exception}");
        }

        /// <summary>
        /// Navigates to a Sample via a deep link.
        /// </summary>
        /// <param name="deepLink">The deep link. Specified as protocol://[collectionName]?sample=[sampleName]</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task NavigateToSampleAsync(string deepLink)
        {
            var parser = DeepLinkParser.Create(deepLink);
            var targetSample = await Samples.GetSampleByName(parser["sample"]);
            if (targetSample != null)
            {
                NavigateToSample(targetSample);
            }
        }

        /// <summary>
        /// Navigates to a Sample
        /// </summary>
        /// <param name="sample">Sample to navigate to</param>
        public void NavigateToSample(Sample sample)
        {
            if (sample == null)
            {
                System.Console.WriteLine($"Navigating to about");
                NavigationFrame.Navigate(typeof(About), null, new SuppressNavigationTransitionInfo());
                System.Console.WriteLine($"Navigated to about");
            }
            else
            {
                NavigationFrame.Navigate(typeof(SampleController), sample);
                TrackingManager.TrackEvent("sample", "navigation", sample.Name);
            }
        }

        /// <summary>
        /// Set app title
        /// </summary>
        /// <param name="title">Title to set</param>
        public void SetAppTitle(string title)
        {
            ApplicationViewExtensions.SetTitle(this, title);
        }

        /// <summary>
        /// Attach a ScrollViewer to Parallax hosting the background image
        /// </summary>
        /// <param name="viewer">The ScrollViewer</param>
        public void AttachScroll(ScrollViewer viewer)
        {
            Parallax.Source = viewer;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationFrame.Navigated += NavigationFrameOnNavigated;
            NavView.BackRequested += NavView_BackRequested;

#if HAS_UNO
            if (!string.IsNullOrWhiteSpace(e?.Parameter?.ToString()))
            {
                var parser = DeepLinkParser.Create(e.Parameter.ToString());
                System.Console.WriteLine($"Deeplink Root:{parser.Root}, Keys: {string.Join(",", parser.Keys)}");

                if (parser.TryGetValue("ShowUnoUnsupported", out var showUnoUnsupportedString)
                    && bool.TryParse(showUnoUnsupportedString, out bool showUnoUnsupported))
                {
                    Console.WriteLine($"");
                    Samples.ShowUnoUnsupported = showUnoUnsupported;
                }
            }
#endif

            // Get list of samples
            var sampleCategories = await Samples.GetCategoriesAsync();
            System.Console.WriteLine($"Got {sampleCategories.Count} categorie (p: {e.Parameter})");
            NavView.MenuItemsSource = sampleCategories;

            SetAppTitle(string.Empty);
            NavigateToSample(null);

            if (!string.IsNullOrWhiteSpace(e?.Parameter?.ToString()))
            {
                var parser = DeepLinkParser.Create(e.Parameter.ToString());

                if (parser.TryGetValue("sample", out var sample))
                {
                    var targetSample = await Sample.FindAsync(parser.Root, sample);
                    if (targetSample != null)
                    {
                        NavigateToSample(targetSample);
                    }
                }
            }

            // NavView goes into a weird size on load unless the window size changes
            // Needs a gentle push to update layout
            NavView.Loaded += (s, args) => NavView.InvalidateMeasure();
            System.Console.WriteLine($"Done navigating");
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (NavigationFrame.CanGoBack)
            {
                NavigationFrame.GoBack();
            }
        }

        private void NavigationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            NavView.IsBackEnabled = NavigationFrame.CanGoBack;

            CurrentSample = navigationEventArgs.Parameter as Sample;
        }

        private void SamplePickerGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HideSamplePicker();
            NavigateToSample(e.ClickedItem as Sample);
        }
    }
}