// -----------------------------------------------------------------------
//  <copyright file="RubikViewModel.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;

namespace Rubik
{
    #region

    #endregion

    public class RubikViewModel : INotifyPropertyChanged
    {

        private static int index = 0;

        public int Id
        {
            get { return id; }
        }

        private RubikCube rubikCube;
        private Grid grid;
        private string name;
        private readonly int id = Interlocked.Increment(ref index);

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


        public string Name
        {
            get => name;
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }

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

        public Grid Grid
        {
            get => this.grid;
            set
            {
                if (Equals(value, this.grid))
                {
                    return;
                }

                this.grid = value;
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