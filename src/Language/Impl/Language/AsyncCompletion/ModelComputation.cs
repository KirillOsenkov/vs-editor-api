﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Implementation
{
    /// <summary>
    /// Facilitates enqueuing tasks to be ran on a worker thread.
    /// Each task takes an immutable instance of <typeparamref name="TModel"/>
    /// and outputs an instance of <typeparamref name="TModel"/>.
    /// The returned instance will serve as input to the next task.
    /// </summary>
    /// <typeparam name="TModel">Type that represents a snapshot of feature's state</typeparam>
    sealed class ModelComputation<TModel> where TModel : class
    {
        private readonly JoinableTaskFactory _joinableTaskFactory;
        private readonly TaskScheduler _computationTaskScheduler;
        private readonly CancellationToken _token;
        private readonly IGuardedOperations _guardedOperations;
        private readonly IModelComputationCallbackHandler<TModel> _callbacks;

        private bool _terminated;
        private JoinableTask<TModel> _lastJoinableTask;
        private CancellationTokenSource _uiCancellation;

        internal TModel RecentModel { get; private set; } = default(TModel);

        /// <summary>
        /// Creates an instance of <see cref="ModelComputation{TModel}"/>
        /// and enqueues an task that will generate the initial state of the <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="computationTaskScheduler"></param>
        /// <param name="joinableTaskContext"></param>
        /// <param name="initialTransformation"></param>
        /// <param name="token"></param>
        /// <param name="guardedOperations"></param>
        /// <param name="callbacks"></param>
        public ModelComputation(
            TaskScheduler computationTaskScheduler,
            JoinableTaskContext joinableTaskContext,
            Func<TModel, CancellationToken, Task<TModel>> initialTransformation,
            CancellationToken token,
            IGuardedOperations guardedOperations,
            IModelComputationCallbackHandler<TModel> callbacks)
        {
            _joinableTaskFactory = joinableTaskContext.Factory;
            _computationTaskScheduler = computationTaskScheduler;
            _token = token;
            _guardedOperations = guardedOperations;
            _callbacks = callbacks;

            // Start dummy tasks so that we don't need to check for null on first Enqueue
            _lastJoinableTask = _joinableTaskFactory.RunAsync(() => Task.FromResult(default(TModel)));
            _uiCancellation = new CancellationTokenSource();

            // Immediately run the first transformation, to operate on proper TModel.
            Enqueue(initialTransformation, updateUi: false);
        }

        /// <summary>
        /// Schedules work to be done on the background,
        /// potentially preempted by another piece of work scheduled in the future,
        /// <paramref name="updateUi" /> indicates whether a single piece of work should occue once all background work is completed.
        /// </summary>
        public void Enqueue(Func<TModel, CancellationToken, Task<TModel>> transformation, bool updateUi)
        {
            // The integrity of our sequential chain depends on this method not being called concurrently.
            // So we require the UI thread.
            if (!_joinableTaskFactory.Context.IsOnMainThread)
                throw new InvalidOperationException($"This method must be callled on the UI thread.");

            if (_token.IsCancellationRequested || _terminated)
                return; // Don't enqueue after computation has stopped.

            // Attempt to commit (CommitIfUnique) will cancel the UI updates. If the commit failed, we still want to update the UI.
            if (_uiCancellation.IsCancellationRequested)
                _uiCancellation = new CancellationTokenSource();

            var previousTask = _lastJoinableTask;
            JoinableTask<TModel> currentTask = null;
            currentTask = _joinableTaskFactory.RunAsync(async () =>
            {
                await Task.Yield(); // Yield to guarantee that currentTask is assigned.
                await _computationTaskScheduler; // Go to the above priority thread. Main thread will return as soon as possible.
                try
                {
                    var previousModel = await previousTask;
                    // Previous task finished processing. We are ready to execute next piece of work.
                    if (_token.IsCancellationRequested || _terminated)
                        return previousModel;

                    var transformedModel = await transformation(await previousTask, _token);
                    RecentModel = transformedModel;

                    // TODO: update UI even if updateUi is false but it wasn't updated yet.
                    if (_lastJoinableTask == currentTask && updateUi)
                    {
                        // update UI because we're the latest task
                        if (!_uiCancellation.IsCancellationRequested)
                            _callbacks.UpdateUi(transformedModel, _uiCancellation.Token).Forget();
                    }

                    return transformedModel;
                }
                catch (Exception ex)
                {
                    _terminated = true;
                    _guardedOperations.HandleException(this, ex);
                    _callbacks.Dismiss();
                    return await previousTask;
                }
            });

            _lastJoinableTask = currentTask;
        }

        /// <summary>
        /// Blocks, waiting for all background work to finish.
        /// Prevents the UI from displaying.
        /// </summary>
        public TModel WaitAndGetResult(CancellationToken token)
        {
            _uiCancellation.Cancel();
            try
            {
                return _lastJoinableTask.Join(token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
    }
}
