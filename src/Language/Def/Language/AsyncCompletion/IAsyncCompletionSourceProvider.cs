﻿using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion
{
    /// <summary>
    /// Provides instances of <see cref="IAsyncCompletionSource"/> which filters and sorts available <see cref="CompletionItem"/>s given the current state of the editor.
    /// </summary>
    /// <summary>
    /// Provides instances of <see cref="IAsyncCompletionSource"/> which provides <see cref="CompletionItem"/>s
    /// and other information relevant to the completion feature at a specific <see cref="SnapshotPoint"/>
    /// </summary>
    /// <remarks>
    /// This is a MEF component and should be exported with [ContentType] and [Name] attributes
    /// and optional [TextViewRoles] attribute.
    /// Completion feature will request data from all exported <see cref="IAsyncCompletionSource"/>s whose ContentType
    /// matches content type of any buffer in the completion's trigger location.
    /// </remarks>
    /// <example>
    ///     [Export(typeof(IAsyncCompletionSource))]
    ///     [Name(nameof(MyCompletionSource))]
    ///     [ContentType("text")]
    ///     [TextViewRoles(PredefinedTextViewRoles.Editable)]
    ///     public class MyCompletionSource : IAsyncCompletionSource
    /// </example>
    public interface IAsyncCompletionSourceProvider
    {
        /// <summary>
        /// Creates an instance of <see cref="IAsyncCompletionSource"/> for the specified <see cref="ITextView"/>.
        /// </summary>
        /// <param name="textView">Text view that will host the completion. Completion acts on buffers of this view.</param>
        /// <returns>Instance of <see cref="IAsyncCompletionSource"/></returns>
        IAsyncCompletionSource GetOrCreate(ITextView textView);
    }
}
