using System;
using System.Collections.Generic;
using Rebus.Extensions;
using Rebus.Logging;
using Rebus.Tests.Contracts.Transports;
using Rebus.Transport;
using Rebus.Transport.Msmq;

namespace Rebus.Tests.Transport.Msmq
{
    public class MsmqTransportFactory : ITransportFactory
    {
        readonly List<IDisposable> _disposables = new List<IDisposable>();
        readonly HashSet<string> _queuesToDelete = new HashSet<string>();

        public ITransport CreateOneWayClient()
        {
            return Create(null);
        }

        public ITransport Create(string inputQueueAddress)
        {
            var transport = new MsmqTransport(inputQueueAddress, new ConsoleLoggerFactory(true));

            _disposables.Add(transport);

            if (inputQueueAddress != null)
            {
                transport.PurgeInputQueue();
            }

            transport.Initialize();

            if (inputQueueAddress != null)
            {
                _queuesToDelete.Add(inputQueueAddress);
            }

            return transport;
        }

        public void CleanUp()
        {
            _disposables.ForEach(d => d.Dispose());
            _disposables.Clear();

            _queuesToDelete.ForEach(MsmqUtil.Delete);
            _queuesToDelete.Clear();
        }
    }
}