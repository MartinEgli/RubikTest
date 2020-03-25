using System.Windows;

namespace Rubik
{
    public class RubikCube2ViewModel : DataContextBehavior<RubikCube>
    {
        /// <summary>
        /// Called when [data context attached].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataContextAttached(object newValue)
        {
 //           this.RubikCube = AssociatedObject;
        }

        /// <summary>
        /// Called when [data context detaching].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        protected override void OnDataContextDetaching(object oldValue)
        {
//            this.RubikCube = null;
        }

        /// <summary>
        /// The rubik cube property
        /// </summary>
        public static readonly DependencyProperty RubikCubeProperty = DependencyProperty.Register(
            "RubikCube",
            typeof(RubikCube),
            typeof(RubikCube2ViewModel),
            new PropertyMetadata(default(RubikCube)));

     /// <summary>
        /// Gets or sets the rubik cube.
        /// </summary>
        /// <value>
        /// The rubik cube.
        /// </value>
        public RubikCube RubikCube
        {
            get => (RubikCube)this.GetValue(RubikCubeProperty);

            set => this.SetValue(RubikCubeProperty, value);
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

        /// <summary>
        /// Sets the rubik cube.
        /// </summary>
        private void SetRubikCube()
        {
            if ((!(this.AssociatedObject is RubikCube rubikCube)))
            {
                return;
            }

            if (rubikCube.Equals(this.RubikCube))
            {
                return;
            }

            this.RubikCube = rubikCube;
        }
    }
}