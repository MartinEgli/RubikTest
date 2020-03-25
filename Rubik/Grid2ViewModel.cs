using System.Windows;
using System.Windows.Controls;

namespace Rubik
{
    public class Grid2ViewModel : FrameworkElementBehavior<Grid>
    {
        /// <summary>
        /// The grid property
        /// </summary>
        public static readonly DependencyProperty GridProperty = DependencyProperty.Register(
            "Grid",
            typeof(Grid),
            typeof(Grid2ViewModel),
            new PropertyMetadata(default(Grid), OnFrameworkElementPropertyChanged));


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