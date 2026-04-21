using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        private readonly double _scaleX;
        private readonly double _scaleY;

        public ModelBall(double abstractLeft, double abstractTop, LogicIBall underneathBall, double scaleX, double scaleY, double abstractDiameter)
        {
            _scaleX = scaleX;
            _scaleY = scaleY;

            LeftBackingField = abstractLeft * _scaleX;
            TopBackingField = abstractTop * _scaleY;

            Diameter = abstractDiameter * _scaleX;

            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        #region IBall

        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                if (TopBackingField == value) return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                if (LeftBackingField == value) return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter { get; init; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double TopBackingField;
        private double LeftBackingField;

        private void NewPositionNotification(object sender, IPosition e)
        {
            Left = e.x * _scaleX;
            Top = e.y * _scaleY;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion private

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x) { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x) { Top = x; }

        #endregion testing instrumentation
    }
}