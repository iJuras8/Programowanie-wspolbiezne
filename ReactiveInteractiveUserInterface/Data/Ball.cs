using System;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        public double Diameter { get; } = 5.0;
        public double Mass { get; } = 1.0;

        private readonly object _lock = new object();

        private bool _isDisposed = false;
        private readonly int _delayMs = 16;

        private Vector _position;
        private IVector _velocity;

        private string _color = "Red";
        private Timer? _realTimeTimer;

        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity
        {
            get { lock (_lock) { return _velocity; } }
            set { lock (_lock) { _velocity = value; } }
        }

        public IVector Position
        {
            get { lock (_lock) { return _position; } }
        }

        public string Color
        {
            get { lock (_lock) { return _color; } }
        }

        #endregion IBall

        #region Threading & Movement

        internal void StartMovement()
        {
            Task.Run(MoveLoop);

            _realTimeTimer = new Timer(ChangeColor, null, 0, 1000);
        }

        private void ChangeColor(object? state)
        {
            lock (_lock)
            {
                _color = _color == "Red" ? "Blue" : "Red";
            }
        }

        private async Task MoveLoop()
        {
            while (!_isDisposed)
            {
                lock (_lock)
                {
                    double newX = _position.x + _velocity.x;
                    double newY = _position.y + _velocity.y;

                    _position = new Vector(newX, newY);
                }

                RaiseNewPositionChangeNotification();

                await Task.Delay(_delayMs);
            }
        }

        private void RaiseNewPositionChangeNotification()
        {
            Vector currentPos;
            lock (_lock) { currentPos = _position; }
            NewPositionNotification?.Invoke(this, currentPos);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _realTimeTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _realTimeTimer?.Dispose();
        }

        #endregion Threading & Movement
    }
}