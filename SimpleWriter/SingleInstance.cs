﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComplexWriter
{
    using System;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;

    public class SingleInstance : IDisposable
    {
        private readonly bool ownsMutex;
        private Mutex mutex;
        private Guid identifier;

        /// <summary>
        /// Occurs when [arguments received].
        /// </summary>
        public event EventHandler<GenericEventArgs<string>> ArgumentsReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstance"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public SingleInstance(Guid id)
        {
            this.identifier = id;
            mutex = new Mutex(true, identifier.ToString(), out ownsMutex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstance"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public SingleInstance()
        {
            this.identifier = new Guid("632ae9b3-0c48-47eb-8219-973d163f0bae");
#if DEBUG
            this.identifier = new Guid("632ae9b3-0c48-47eb-8219-973d163f0baa");
#endif
            mutex = new Mutex(true, identifier.ToString(), out ownsMutex);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is first instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstInstance
        {
            get
            {
                return ownsMutex;
            }
        }

        public Application Application { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (mutex != null && ownsMutex)
            {
                mutex.ReleaseMutex();
                mutex = null;
            }
        }

        /// <summary>
        /// Passes the arguments to first instance.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public void PassArgumentsToFirstInstance(string argument)
        {
            using (var client = new NamedPipeClientStream(identifier.ToString()))
            using (var writer = new StreamWriter(client))
            {
                client.Connect(200);
                writer.WriteLine(argument);
            }
        }

        /// <summary>
        /// Listens for arguments from successive instances.
        /// </summary>
        public void ListenForArgumentsFromSuccessiveInstances()
        {
            Task.Factory.StartNew(() =>
            {

                using (var server = new NamedPipeServerStream(identifier.ToString()))
                using (var reader = new StreamReader(server))
                {
                    while (true)
                    {
                        server.WaitForConnection();

                        var argument = string.Empty;
                        while (server.IsConnected)
                        {
                            argument += reader.ReadLine();
                        }

                        CallOnArgumentsReceived(argument);
                        server.Disconnect();
                    }
                }
            });
        }

        /// <summary>
        /// Calls the on arguments received.
        /// </summary>
        /// <param name="state">The state.</param>
        public void CallOnArgumentsReceived(object state)
        {
            if (ArgumentsReceived != null)
            {
                if (state == null)
                {
                    state = string.Empty;
                }

                ArgumentsReceived(this, new GenericEventArgs<string>() { Data = state.ToString() });
            }
        }

        public class GenericEventArgs<TEventDataType> : EventArgs
        {
            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            /// <value>The data.</value>
            public TEventDataType Data { get; set; }
        }
    }
}
