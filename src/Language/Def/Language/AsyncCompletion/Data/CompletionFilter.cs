﻿using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Adornments;

namespace Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data
{
    /// <summary>
    /// Identifies a filter that toggles exclusive display of <see cref="CompletionItem"/>s that reference it.
    /// </summary>
    /// <remarks>
    /// These instances should be singletons. All <see cref="CompletionItem"/>s that should be filtered
    /// using the same filter button must use the same reference to the instance of <see cref="CompletionFilter"/>.
    /// </remarks>
    /// <example>
    /// static CompletionFilter MyFilter = new CompletionFilter("My items", "m", MyItemsImageElement);
    /// </example>
    [DebuggerDisplay("{DisplayText}")]
    public sealed class CompletionFilter
    {
        /// <summary>
        /// Localized name of this filter.
        /// </summary>
        public string DisplayText { get; }

        /// <summary>
        /// Key used in a keyboard shortcut that toggles this filter.
        /// </summary>
        public string AccessKey { get; }

        /// <summary>
        /// <see cref="ImageElement"/> that represents this filter.
        /// </summary>
        public ImageElement Image { get; }

        /// <summary>
        /// Constructs an instance of CompletionFilter.
        /// </summary>
        /// <param name="displayText">Name of this filter</param>
        /// <param name="accessKey">Key used in a keyboard shortcut that toggles this filter.</param>
        /// <param name="image">Image that represents this filter</param>
        public CompletionFilter(string displayText, string accessKey, ImageElement image)
        {
            if (string.IsNullOrWhiteSpace(displayText))
            {
                throw new ArgumentException("Display text must be non-empty", nameof(displayText));
            }
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("Access key must be non-empty", nameof(accessKey));
            }

            DisplayText = displayText;
            AccessKey = accessKey;
            Image = image;
        }
    }
}
