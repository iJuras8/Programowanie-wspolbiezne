using System;
using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using DataAPI = TP.ConcurrentProgramming.Data; 

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly double _boardSize = 100.0;
        private readonly double _ballDiameter = 5.0;

        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                databall.NewPositionNotification += (sender, currentPosition) =>
                {
                    CheckWallCollisions(databall, currentPosition);
                };

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall));
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region Physics & Collisions

        private void CheckWallCollisions(DataAPI.IBall ball, DataAPI.IVector position)
        {
            var velocity = ball.Velocity;
            double newVx = velocity.x;
            double newVy = velocity.y;
            bool bounced = false;

            if (position.x <= 0 && velocity.x < 0)
            {
                newVx = -velocity.x;
                bounced = true;
            }
            else if (position.x >= _boardSize - _ballDiameter && velocity.x > 0)
            {
                newVx = -velocity.x;
                bounced = true;
            }

            if (position.y <= 0 && velocity.y < 0)
            {
                newVy = -velocity.y;
                bounced = true;
            }
            else if (position.y >= _boardSize - _ballDiameter && velocity.y > 0)
            {
                newVy = -velocity.y;
                bounced = true;
            }

            if (bounced)
            {
                ball.Velocity = new LogicVector(newVx, newVy);
            }
        }

        private class LogicVector : DataAPI.IVector
        {
            public double x { get; init; }
            public double y { get; init; }
            public LogicVector(double x, double y) { this.x = x; this.y = y; }
        }

        #endregion Physics & Collisions

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}