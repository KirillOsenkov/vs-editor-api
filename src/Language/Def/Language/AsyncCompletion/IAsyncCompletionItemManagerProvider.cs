﻿using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion
{
    /// <summary>
    /// Provides instances of <see cref="IAsyncCompletionItemManager"/> which filters and sorts available <see cref="CompletionItem"/>s given the current state of the editor.
    /// </summary>
    /// <remarks>
    /// This is a MEF component and should be exported with [ContentType] and [Name] attributes
    /// and optional [Order] and [TextViewRoles] attributes.
    /// An instance of <see cref="IAsyncCompletionItemManager"/> is selected
    /// first by matching ContentType with content type of the view's top buffer, and then by Order.
    /// Only one <see cref="IAsyncCompletionItemManager"/> is used in a given view.
    /// </remarks>
    /// <example>
    ///     [Export(typeof(IAsyncCompletionItemManagerProvider))]
    ///     [Name(nameof(MyCompletionItemManagerProvider))]
    ///     [ContentType("text")]
    ///     [TextViewRoles(PredefinedTextViewRoles.Editable)]
    ///     [Order(Before = "OtherCompletionItemManager")]
    ///     public class MyCompletionItemManagerProvider : IAsyncCompletionItemManagerProvider
    /// </example>
    public interface IAsyncCompletionItemManagerProvider
    {
        /// <summary>
        /// Creates an instance of <see cref="IAsyncCompletionItemManager"/> for the specified <see cref="ITextView"/>.
        /// </summary>
        /// <param name="textView">Text view that will host the completion. Completion acts on buffers of this view.</param>
        /// <returns>Instance of <see cref="IAsyncCompletionItemManager"/></returns>
        IAsyncCompletionItemManager GetOrCreate(ITextView textView);
    }
}
