// -----------------------------------------------------------------------
//  <copyright file="RubikViewModel.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RubikDemo
{
    #region

    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.CommandWpf;
    using JetBrains.Annotations;
    using Rubik;

    #endregion

    public class RubikViewModel : INotifyPropertyChanged
    {
        private RubikCube rubikCube;

        public RubikViewModel()
        {
            this.RotateCommand = new RelayCommand<object>(this.OnRotate);
            this.UnscrambleCommand = new RelayCommand(this.OnUnscramble);
            this.ScrambleCommand = new RelayCommand(this.OnScramble);
            this.PlusSizeCommand = new RelayCommand(this.OnPlusSize);
            this.MinusSizeCommand = new RelayCommand(this.OnMinusSize);
        }

        public ICommand RotateCommand { get; }

        public ICommand MinusSizeCommand { get; }

        public ICommand PlusSizeCommand { get; }

        public ICommand ScrambleCommand { get; }

        public ICommand UnscrambleCommand { get; }

        public RubikCube RubikCube
        {
            get => this.rubikCube;
            set
            {
                if (Equals(value, this.rubikCube))
                {
                    return;
                }

                this.rubikCube = value;
                this.OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnRotate(object obj)
        {
            if (obj is Key key)
            {
                this.rubikCube.Rotate(key);
            }
        }

        private void OnMinusSize()
        {
            if (this.rubikCube.Size > 2)
            {
                this.rubikCube.Size--;
            }
        }

        private void OnPlusSize()
        {
            this.rubikCube.Size++;
        }

        private void OnUnscramble()
        {
            if (this.rubikCube.CanUnscramble())
            {
                this.rubikCube.Unscramble();
            }
        }

        private void OnScramble()
        {
            this.rubikCube.Scramble();
        }

        //private void OnKeyDown(Key key)
        //{
        //    if ((key == Key.Back || key == Key.Escape) && cube1.CanUnscramble())
        //    {
        //        cube1.Unscramble();
        //        //                e.Handled = true;
        //    }

        //    if (key == Key.Space)
        //    {
        //        cube1.Scramble();
        //        //              e.Handled = true;
        //    }

        //    if (key == Key.Add)
        //    {
        //        cube1.Size++;
        //        //               e.Handled = true;
        //    }

        //    if (key == Key.Subtract && cube1.Size > 2)
        //    {
        //        cube1.Size--;
        //        //              e.Handled = true;
        //    }

        //    cube1.Rotate(key);
        //}

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}