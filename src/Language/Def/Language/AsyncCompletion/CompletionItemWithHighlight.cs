﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion
{
    /// <summary>
    /// Wraps <see cref="CompletionItem"/> with information about highlighted parts of its <see cref="CompletionItem.DisplayText"/>.
    /// </summary>
    [DebuggerDisplay("{CompletionItem}")]
    public struct CompletionItemWithHighlight : IEquatable<CompletionItemWithHighlight>
    {
        /// <summary>
        /// The completion item
        /// </summary>
        public CompletionItem CompletionItem { get; }

        /// <summary>
        /// Which parts of <see cref="CompletionItem.DisplayText"/> to highlight
        /// </summary>
        public ImmutableArray<Span> HighlightedSpans { get; }

        /// <summary>
        /// Constructs <see cref="CompletionItemWithHighlight"/> without any highlighting.
        /// Used when the <see cref="CompletionItem"/> appears in the completion list without being a text match.
        /// </summary>
        /// <param name="completionItem">Instance of the <see cref="CompletionItem"/></param>
        public CompletionItemWithHighlight(CompletionItem completionItem)
            : this (completionItem, ImmutableArray<Span>.Empty)
        {
        }

        /// <summary>
        /// Constructs <see cref="CompletionItemWithHighlight"/> with given highlighting.
        /// Used when text used to filter the completion list can be found in the <see cref="CompletionItem.DisplayText"/>.
        /// </summary>
        /// <param name="completionItem">Instance of the <see cref="CompletionItem"/></param>
        /// <param name="highlightedSpans"><see cref="Span"/>s of <see cref="CompletionItem.DisplayText"/> to highlight</param>
        public CompletionItemWithHighlight(CompletionItem completionItem, ImmutableArray<Span> highlightedSpans)
        {
            CompletionItem = completionItem ?? throw new ArgumentNullException(nameof(completionItem));
            if (highlightedSpans.IsDefault)
                throw new ArgumentException("Array must be initialized", nameof(highlightedSpans));
            HighlightedSpans = highlightedSpans;
        }

        bool IEquatable<CompletionItemWithHighlight>.Equals(CompletionItemWithHighlight other)
            => CompletionItem.Equals(other.CompletionItem) && HighlightedSpans.Equals(other.HighlightedSpans);
    }
}
