﻿//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//
using System;

namespace Microsoft.VisualStudio.Text.Editor
{
    /// <summary>
    /// Utility <see cref="ITextView"/> extension methods.
    /// </summary>
    public static class TextViewExtensions
    {
        /// <summary>
        /// Gets whether given <see cref="ITextView"/> is embedded in another <see cref="ITextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="ITextView"/> for which to determine if it's embedded.</param>
        /// <returns><c>true</c> if given <see cref="ITextView"/> is embedded, <c>false</c> otherwise.</returns>
        public static bool IsEmbeddedTextView(this ITextView textView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException(nameof(textView));
            }

            return textView.Roles.Contains(PredefinedTextViewRoles.EmbeddedPeekTextView);
        }

        /// <summary>
        /// Gets containing <see cref="ITextView"/> for given embedded <see cref="ITextView"/>.
        /// </summary>
        /// <param name="textView">An embedded <see cref="ITextView"/>, for which to get a containing <see cref="ITextView"/>.</param>
        /// <param name="containingTextView">A <see cref="ITextView"/> that contains given <see cref="ITextView"/> or null if
        /// given <see cref="ITextView"/> is not embedded in another <see cref="ITextView"/>.</param>
        /// <returns><c>true</c> if containing <see cref="ITextView"/> was found, <c>false</c> otherwise.</returns>
        public static bool TryGetContainingTextView(this ITextView textView, out ITextView containingTextView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException(nameof(textView));
            }

            // Extra scrutiny because Peek is on a different layer and we cannot just rely on it doing the right thing
            if (textView.IsEmbeddedTextView())
            {
                bool success = textView.Properties.TryGetProperty("PeekContainingTextView", out containingTextView);
                if (!success || containingTextView == null)
                {
                    throw new InvalidOperationException("Unexpected failure to obtain containing text view of an embedded text view.");
                }

                return true;
            }

            containingTextView = null;
            return false;
        }
    }
}
