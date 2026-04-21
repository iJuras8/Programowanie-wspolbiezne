using System;
using System.Collections.ObjectModel;
using System.Windows.Input; 
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            StartCommand = new RelayCommand(() =>
            {
                if (int.TryParse(NumberOfBallsInput, out int count) && count > 0)
                {
                    Start(count);
                }
            });
        }

        #endregion ctor

        #region public API

        private string _numberOfBallsInput = "5"; 
        public string NumberOfBallsInput
        {
            get { return _numberOfBallsInput; }
            set
            {
                if (_numberOfBallsInput != value)
                {
                    _numberOfBallsInput = value;
                    RaisePropertyChanged(); 
                }
            }
        }

        public ICommand StartCommand { get; }

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));

            Balls.Clear();

            ModelLayer.Start(numberOfBalls);
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer?.Dispose();
                    ModelLayer?.Dispose();
                }
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}