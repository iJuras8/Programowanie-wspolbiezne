using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector initialPosition = new Vector(10.0, 10.0);
            Vector initialVelocity = new Vector(1.0, -1.0);

            Ball newInstance = new(initialPosition, initialVelocity);

            Assert.AreEqual(10.0, newInstance.Position.x);
            Assert.AreEqual(10.0, newInstance.Position.y);

            Assert.AreEqual(1.0, newInstance.Velocity.x);
            Assert.AreEqual(-1.0, newInstance.Velocity.y);
        }

        [TestMethod]
        public async Task StartMovementAndNotificationTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Vector initialVelocity = new(1.0, 1.0);

            using Ball newInstance = new(initialPosition, initialVelocity);

            bool eventRaised = false;
            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                eventRaised = true;
            };

            newInstance.StartMovement();

            await Task.Delay(50);

            Assert.IsTrue(eventRaised);
        }
    }
}