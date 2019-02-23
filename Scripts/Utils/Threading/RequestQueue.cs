using System.Collections.Generic;

namespace Utils.Threading
{
    /// <summary>
    /// A thread safe queue of requests to be processed.
    /// </summary>
    /// <typeparam name="REQUEST">The type of request the queue holds.</typeparam>
    public class RequestQueue<REQUEST> where REQUEST : IRequest
    {
        private readonly Queue<REQUEST> requests;

        public RequestQueue()
        {
            requests = new Queue<REQUEST>();
        }

        /// <summary>
        /// Clears all the requests.
        /// </summary>
        public void Clear()
        {
            lock (requests)
                requests.Clear();
        }
        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">The request to enqueue.</param>
        public void Enqueue(REQUEST request)
        {
            lock (requests)
                requests.Enqueue(request);
        }

        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests()
        {
            lock (requests)
                while (requests.Count > 0)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process();
                }
        }
        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(int max)
        {
            lock (requests)
                for (int i = 0; i < max && requests.Count > 0; i++)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process();
                }
        }
    }
    /// <summary>
    /// A thread safe queue of requests to be processed.
    /// </summary>
    /// <typeparam name="REQUEST">The type of request the queue holds.</typeparam>
    /// <typeparam name="T1">The type of argument passed to requests when they are processed.</typeparam>
    public class RequestQueue<REQUEST, T1> where REQUEST : IRequest<T1>
    {
        private readonly Queue<REQUEST> requests;

        public RequestQueue()
        {
            requests = new Queue<REQUEST>();
        }

        /// <summary>
        /// Clears all the requests.
        /// </summary>
        public void Clear()
        {
            lock (requests)
                requests.Clear();
        }
        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">The request to enqueue.</param>
        public void Enqueue(REQUEST request)
        {
            lock (requests)
                requests.Enqueue(request);
        }

        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val)
        {
            lock (requests)
                while (requests.Count > 0)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val);
                }
        }
        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val, int max)
        {
            lock (requests)
                for (int i = 0; i < max && requests.Count > 0; i++)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val);
                }
        }
    }
    /// <summary>
    /// A thread safe queue of requests to be processed.
    /// </summary>
    /// <typeparam name="REQUEST">The type of request the queue holds.</typeparam>
    /// <typeparam name="T1">The first type of argument passed to requests when they are processed.</typeparam>
    /// <typeparam name="T2">The second type of argument passed to requests when they are processed.</typeparam>
    public class RequestQueue<REQUEST, T1, T2> where REQUEST : IRequest<T1, T2>
    {
        private readonly Queue<REQUEST> requests;

        public RequestQueue()
        {
            requests = new Queue<REQUEST>();
        }

        /// <summary>
        /// Clears all the requests.
        /// </summary>
        public void Clear()
        {
            lock (requests)
                requests.Clear();
        }
        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">The request to enqueue.</param>
        public void Enqueue(REQUEST request)
        {
            lock (requests)
                requests.Enqueue(request);
        }

        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val, T2 val2)
        {
            lock (requests)
                while (requests.Count > 0)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val, val2);
                }
        }
        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val, T2 val2, int max)
        {
            lock (requests)
                for (int i = 0; i < max && requests.Count > 0; i++)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val, val2);
                }
        }
    }
    /// <summary>
    /// A thread safe queue of requests to be processed.
    /// </summary>
    /// <typeparam name="REQUEST">The type of request the queue holds.</typeparam>
    /// <typeparam name="T1">The first type of argument passed to requests when they are processed.</typeparam>
    /// <typeparam name="T2">The second type of argument passed to requests when they are processed.</typeparam>
    /// <typeparam name="T3">The third type of argument passed to requests when they are processed.</typeparam>
    public class RequestQueue<REQUEST, T1, T2, T3> where REQUEST : IRequest<T1, T2, T3>
    {
        private readonly Queue<REQUEST> requests;

        public RequestQueue()
        {
            requests = new Queue<REQUEST>();
        }

        /// <summary>
        /// Clears all the requests.
        /// </summary>
        public void Clear()
        {
            lock (requests)
                requests.Clear();
        }
        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">The request to enqueue.</param>
        public void Enqueue(REQUEST request)
        {
            lock (requests)
                requests.Enqueue(request);
        }

        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val, T2 val2, T3 val3)
        {
            lock (requests)
                while (requests.Count > 0)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val, val2, val3);
                }
        }
        /// <summary>
        /// Processes a specific number of requests.
        /// </summary>
        /// <param name="max">The maximum number of requests to process.</param>
        public void RunRequests(T1 val, T2 val2, T3 val3, int max)
        {
            lock (requests)
                for (int i = 0; i < max && requests.Count > 0; i++)
                {
                    REQUEST r = requests.Dequeue();
                    r.Process(val, val2, val3);
                }
        }
    }
}
