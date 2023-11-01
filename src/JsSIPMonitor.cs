using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPMonitor
    {
        public string? Id { get; internal set; }

        public event EventHandler<string?>? OnChanged;

        private void NotifyChanged()
        {
            OnChanged?.Invoke(this, Id);
        }

        public void Set(string? id)
        {
            if (Id != id)
            {
                Id = id;
                NotifyChanged();
            }
        }
    }
}
