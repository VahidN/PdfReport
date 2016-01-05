using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemosBrowser.Toolkit.Threading
{
    public class ParallelTasksQueue : TaskScheduler, IDisposable
    {
        #region Fields (5)

        private readonly ApartmentState _apartmentState;
        private bool _disposed;
        private BlockingCollection<Task> _tasks;
        private readonly ThreadPriority _threadPriority;
        private readonly List<Thread> _threads;

        #endregion Fields

        #region Constructors (2)

        public ParallelTasksQueue(int numberOfThreads, ApartmentState apartmentState, ThreadPriority threadPriority)
        {
            _apartmentState = apartmentState;
            _threadPriority = threadPriority;

            if (numberOfThreads < 1) numberOfThreads = Environment.ProcessorCount;

            _tasks = new BlockingCollection<Task>();

            _threads = Enumerable.Range(0, numberOfThreads).Select(i =>
            {
                var thread = new Thread(() =>
                {
                    foreach (var task in _tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(task);
                    }
                }) { IsBackground = true, Priority = _threadPriority };
                thread.SetApartmentState(_apartmentState);
                return thread;
            }).ToList();

            _threads.ForEach(t => t.Start());
        }

        public ParallelTasksQueue(int numberOfThreads)
            : this(numberOfThreads, ApartmentState.MTA, ThreadPriority.Normal) { }

        #endregion Constructors

        #region Properties (1)

        public override int MaximumConcurrencyLevel
        {
            get { return _threads.Count; }
        }

        #endregion Properties

        #region Methods (6)

        // Public Methods (1) 

        public void Dispose()
        {
            Dispose(true);
            // tell the GC that the Finalize process no longer needs to be run for this object.  
            GC.SuppressFinalize(this);
        }
        // Protected Methods (5) 

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;
            if (!disposeManagedResources) return;

            if (_tasks != null)
            {
                _tasks.CompleteAdding();

                foreach (var thread in _threads) thread.Join();

                _tasks.Dispose();
                _tasks = null;
            }

            _disposed = true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryDequeue(Task task)
        {
            return base.TryDequeue(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (Thread.CurrentThread.GetApartmentState() != _apartmentState) return false;
            return Thread.CurrentThread.Priority == _threadPriority && TryExecuteTask(task);
        }

        #endregion Methods
    }
}
