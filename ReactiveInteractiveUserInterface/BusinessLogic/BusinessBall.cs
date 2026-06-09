using System;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private readonly Data.IBall _dataBall;

        public Ball(Data.IBall ball)
        {
            _dataBall = ball;

            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        public string Color => _dataBall.Color;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        #endregion private
    }
}