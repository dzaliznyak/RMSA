using System;

namespace Rmsa.Core.Utils
{
    public class InteractionParams
    {
        public Action<bool> Callback { get; set; }
        public string FileName { get; set; }
        public bool DialogResult { get; set; }
    }
}
