namespace GestureSign.Daemon.Input
{
    // Own the complete native left-button pair, including an up arriving after
    // HID contact release. Never claim a drag which began before the edge.
    internal sealed class EdgeClickGate
    {
        private readonly object _sync = new object();
        private bool _active, _suppressedDown, _preexistingDown;
        public bool NeedsHook { get { lock (_sync) return _active || _suppressedDown; } }
        public void Begin(bool leftAlreadyDown)
        {
            lock (_sync)
            {
                _active = true;
                _preexistingDown = leftAlreadyDown && !_suppressedDown;
            }
        }
        public void End() { lock (_sync) _active = false; }
        public bool Filter(bool down, bool injected)
        {
            lock (_sync)
            {
                if (injected) return false;
                if (!down)
                {
                    _preexistingDown = false;
                    bool suppress = _suppressedDown;
                    _suppressedDown = false;
                    return suppress;
                }
                // A fresh down outside capture recovers a missing button-up.
                if (!_active) { _suppressedDown = false; return false; }
                if (_preexistingDown) return false;
                _suppressedDown = true;
                return true;
            }
        }
    }
}
