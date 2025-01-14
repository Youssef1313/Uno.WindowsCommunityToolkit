// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// In App Notification defines a control to show local notification in the app.
    /// </summary>
    public partial class InAppNotification
    {
        /// <summary>
        /// Key of the VisualStateGroup that show/dismiss content
        /// </summary>
        private const string GroupContent = "State";

        /// <summary>
        /// Key of the VisualState when content is showed
        /// </summary>
        private const string StateContentVisible = "Visible";

        /// <summary>
        /// Key of the VisualState when content is dismissed
        /// </summary>
        private const string StateContentCollapsed = "Collapsed";

        /// <summary>
        /// Key of the UI Element that dismiss the control
        /// </summary>
        private const string DismissButtonPart = "PART_DismissButton";

        /// <summary>
        /// Key of the UI Element that will display the notification content.
        /// </summary>
        private const string ContentPresenterPart = "PART_Presenter";
    }
}