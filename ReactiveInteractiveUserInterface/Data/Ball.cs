using System;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private readonly double _boardWidth = 400.0;
        private readonly double _boardHeight = 420.0;
        private readonly double _diameter = 20.0;

        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        #endregion IBall

        #region private

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            if (newX < 0)
            {
                newX = 0; 
            }
            else if (newX > _boardWidth - _diameter)
            {
                newX = _boardWidth - _diameter; 
            }

            if (newY < 0)
            {
                newY = 0; 
            }
            else if (newY > _boardHeight - _diameter)
            {
                newY = _boardHeight - _diameter; 
            }

            Position = new Vector(newX, newY);
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}