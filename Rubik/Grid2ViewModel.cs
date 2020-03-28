using System.Windows;
using System.Windows.Controls;

namespace Rubik
{
    public class Grid2ViewModel : DataContextBehavior<Grid>
    {
        /// <summary>
        /// The grid property
        /// </summary>
        public static readonly DependencyProperty GridProperty = DependencyProperty.Register(
            "Grid",
            typeof(Grid),
            typeof(Grid2ViewModel),
            new PropertyMetadata(default(Grid)));

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(Grid2ViewModel), new PropertyMetadata(default(int), OnPropertyChangedCallback));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(Grid2ViewModel), new PropertyMetadata(default(object), OnTitlePropertyChangedCallback));

        private static void OnTitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>
        /// The grid.
        /// </value>
        public Grid Grid
        {
            get => (Grid)this.GetValue(GridProperty);

            set => this.SetValue(GridProperty, value);
        }

        public int Id
        {
            get { return (int) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public object Title
        {
            get { return (object) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetRubikCube();
            Title = "Name,Id";
        }


        protected override void OnDataContextAttached(object obj)
        {
            if ((!(this.AssociatedObject is Grid grid)))
            {
                return;
            }

            this.Grid = grid;
        }

        private void SetRubikCube()
        {
            if ((!(this.AssociatedObject is Grid grid)))
            {
                return;
            }

            if (grid.Equals(this.Grid))
            {
                return;
            }

            this.Grid = grid;
        }

        //protected override void OnChanged()
        //{
        //    base.OnChanged();
        //    //           this.SetRubikCube();
        //}
    }
}