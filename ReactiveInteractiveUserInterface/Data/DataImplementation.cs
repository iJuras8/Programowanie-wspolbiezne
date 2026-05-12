using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            // Całkowicie wykasowaliśmy Timer!
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            // Zabezpieczenie: ubijamy ewentualne stare wątki przed stworzeniem nowych
            foreach (var ball in BallsList)
            {
                ball.Dispose();
            }
            BallsList.Clear();

            Random random = new Random();

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.NextDouble() * 95, random.NextDouble() * 95);
                Vector startingVelocity = new((random.NextDouble() - 0.5) * 2, (random.NextDouble() - 0.5) * 2);

                Ball newBall = new(startingPosition, startingVelocity);

                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);

                // Uruchamiamy niezależny wątek na nowo utworzonej kuli
                newBall.StartMovement();
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // Kiedy zamykamy aplikację/resetujemy, musimy zabić wszystkie Taski z kulek
                    foreach (var ball in BallsList)
                    {
                        ball.Dispose();
                    }
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private List<Ball> BallsList = [];

        // Znika cała metoda Move(object? x) oraz zmienna Timera

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}