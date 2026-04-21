using TP.ConcurrentProgramming.BusinessLogic;
using System;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
    [TestClass]
    public class ModelBallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            double abstractLeft = 10.0;
            double abstractTop = 10.0;
            double scaleX = 2.0;
            double scaleY = 3.0;
            double abstractDiameter = 5.0;

            ModelBall ball = new ModelBall(abstractLeft, abstractTop, new BusinessLogicIBallFixture(), scaleX, scaleY, abstractDiameter);

            Assert.AreEqual<double>(20.0, ball.Left);     
            Assert.AreEqual<double>(30.0, ball.Top);      
            Assert.AreEqual<double>(10.0, ball.Diameter); 
        }

        [TestMethod]
        public void PositionChangeNotificationTestMethod()
        {
            int notificationCounter = 0;

            ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture(), 1.0, 1.0, 5.0);

            ball.PropertyChanged += (sender, args) => notificationCounter++;
            Assert.AreEqual(0, notificationCounter);

            ball.SetLeft(1.0);
            Assert.AreEqual<int>(1, notificationCounter);
            Assert.AreEqual<double>(1.0, ball.Left);
            Assert.AreEqual<double>(0.0, ball.Top);

            ball.SettTop(1.0);
            Assert.AreEqual(2, notificationCounter);
            Assert.AreEqual<double>(1.0, ball.Left);
            Assert.AreEqual<double>(1.0, ball.Top);
        }

        #region testing instrumentation

        private class BusinessLogicIBallFixture : BusinessLogic.IBall
        {
            public event EventHandler<IPosition>? NewPositionNotification { add { } remove { } }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        #endregion testing instrumentation
    }
}