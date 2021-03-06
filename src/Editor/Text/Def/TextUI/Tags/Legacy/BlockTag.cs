﻿//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//
namespace Microsoft.VisualStudio.Text.Tagging
{
    using System;
    using Microsoft.VisualStudio.Text.Adornments;

    /// <summary>
    /// An implementation of <see cref="IBlockTag" />.
    /// </summary>
    [Obsolete("Use StructureTag instead")]
    public abstract class BlockTag : IBlockTag
    {
        public BlockTag(SnapshotSpan span, SnapshotSpan statementSpan, IBlockTag parent, string type, bool isCollapsible, bool isDefaultCollapsed, bool isImplementation, object collapsedForm, object collapsedHintForm)
        {
            this.Span = span;
            this.Level = (parent == null) ? 0 : (parent.Level + 1);
            this.StatementSpan = statementSpan;
            this.Parent = parent;
            this.Type = type;
            this.IsCollapsible = isCollapsible;
            this.IsDefaultCollapsed = isDefaultCollapsed;
            this.IsImplementation = isImplementation;
            this.CollapsedForm = collapsedForm;
            this.CollapsedHintForm = collapsedHintForm;
        }

        /// <summary>
        /// Gets the span of the structural block.
        /// </summary>
        public virtual SnapshotSpan Span { get; set; }

        /// <summary>
        /// Gets the level of nested-ness of the structural block.
        /// </summary>
        public virtual int Level { get; }

        /// <summary>
        /// Gets the span of the statement that control the structral block.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example, in the following snippet of code,
        /// <code>
        /// if (condition1 &amp;&amp;
        ///     condition2) // comment
        /// {
        ///     something;
        /// }
        /// </code>
        /// this.StatementSpan would extend from the start of the "if" to the end of comment.
        /// this.Span would extend from before the "{" to the end of the "}".
        /// </para>
        /// </remarks>
        public virtual SnapshotSpan StatementSpan { get; set; }

        /// <summary>
        /// Gets the hierarchical parent of the structural block.
        /// </summary>
        public virtual IBlockTag Parent { get; }

        /// <summary>
        /// Determines the semantic type of the structural block.
        /// <remarks>
        /// See <see cref="PredefinedStructureTypes"/> for the canonical types.
        /// Use <see cref="PredefinedStructureTypes.Nonstructural"/> for blocks that will not have any visible affordance
        /// (but will be used for outlining).
        /// </remarks>
        /// </summary>
        public virtual string Type { get; }

        /// <summary>
        /// Determines whether a block can be collapsed.
        /// </summary>
        public virtual bool IsCollapsible { get; }

        /// <summary>
        /// Determines whether a block is collapsed by default.
        /// </summary>
        public virtual bool IsDefaultCollapsed { get; }

        /// <summary>
        /// Determines whether a block is an block region.
        /// </summary>
        /// <remarks>
        /// Implementation blocks are the blocks of code following a method definition.
        /// They are used for commands such as the Visual Studio Collapse to Definition command,
        /// which hides the implementation block and leaves only the method definition exposed.
        /// </remarks>
        public virtual bool IsImplementation { get; }

        /// <summary>
        /// Gets the data object for the collapsed UI. If the default is set, returns null.
        /// </summary>
        public virtual object CollapsedForm { get; }

        /// <summary>
        /// Gets the data object for the collapsed UI tooltip. If the default is set, returns null.
        /// </summary>
        public virtual object CollapsedHintForm { get; }
    }
}
