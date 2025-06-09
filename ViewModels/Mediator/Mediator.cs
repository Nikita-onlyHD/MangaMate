using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaMate.ViewModels.Mediator
{
    public class Mediator
    {
        private readonly Dictionary<string, List<IEventSender>> _senders = [];
        private readonly Dictionary<string, List<IEventReceiver>> _receivers = [];

        public void AddSender(IEventSender sender, string channel)
        {
            if (!_senders.ContainsKey(channel))
            {
                var newSender = new List<IEventSender>() { sender };
                _senders.Add(channel, newSender);
            } 
            else
            {
                var s = _senders[channel];
                s.Add(sender);
                _senders[channel] = s;
            }
        }

        public void AddReceiver(IEventReceiver receiver, string channel)
        {
            
        }
    }
}
