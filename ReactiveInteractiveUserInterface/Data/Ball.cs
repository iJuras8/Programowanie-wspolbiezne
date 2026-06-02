using System;
using System.Diagnostics;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        public double Diameter { get; } = 5.0;
        public double Mass { get; } = 1.0;

        private readonly object _lock = new object();

        private Timer? _movementTimer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private long _lastTimeMs = 0;

        private Vector _position;
        private IVector _velocity;

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

        #endregion IBall

        #region Threading & Real-Time Physics

        internal void StartMovement()
        {
            _stopwatch.Start();
            _lastTimeMs = _stopwatch.ElapsedMilliseconds;

            _movementTimer = new Timer(MoveLoop, null, 0, 16);
        }

        private void MoveLoop(object? state)
        {
            if (_movementTimer == null) return;

            long currentTimeMs = _stopwatch.ElapsedMilliseconds;
            long elapsedMs = currentTimeMs - _lastTimeMs;
            _lastTimeMs = currentTimeMs;

            double timeMultiplier = elapsedMs / 16.0;

            lock (_lock)
            {
                double newX = _position.x + (_velocity.x * timeMultiplier);
                double newY = _position.y + (_velocity.y * timeMultiplier);

                _position = new Vector(newX, newY);
            }

            RaiseNewPositionChangeNotification();
        }

        private void RaiseNewPositionChangeNotification()
        {
            Vector currentPos;
            lock (_lock) { currentPos = _position; }
            NewPositionNotification?.Invoke(this, currentPos);
        }

        public void Dispose()
        {
            _movementTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _movementTimer?.Dispose();
            _movementTimer = null;
            _stopwatch.Stop();
        }

        #endregion Threading & Real-Time Physics
    }
}