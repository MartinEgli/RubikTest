// -----------------------------------------------------------------------
//  <copyright file="RubikCube.cs" company="Anori Soft">
//      Copyright (c) Anori Soft Martin Egli. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Rubik
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    #endregion

    /// <summary>
    /// A rubik's cube - demo of building a geometry with Helix Toolkit's MeshBuilder
    /// and using animated transforms to do the rotations
    /// http://en.wikipedia.org/wiki/Rubik's_Cube
    /// http://www.rubiks.com/
    /// </summary>
    public class RubikCube : ModelVisual3D
    {
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size",
            typeof(int),
            typeof(RubikCube),
            new UIPropertyMetadata(3, SizeChanged));

        private readonly Color[] faceColors =
            {
                Colors.White, Colors.Red, Colors.Blue, Colors.Orange, Colors.Green, Colors.Yellow
            };

        private readonly Dictionary<Key, int> FaceKey = new Dictionary<Key, int>
                                                            {
                                                                { Key.B, 0 },
                                                                { Key.F, 1 },
                                                                { Key.L, 2 },
                                                                { Key.R, 3 },
                                                                { Key.D, 4 },
                                                                { Key.U, 5 }
                                                            };

        /// <summary>
        /// The history of all moves that has been done.
        /// </summary>
        private readonly Stack<Tuple<int, double>> history = new Stack<Tuple<int, double>>();

        private readonly Random random = new Random();

        private readonly Dictionary<int, Vector3D> RotationAxis = new Dictionary<int, Vector3D>
                                                                      {
                                                                          { 0, new Vector3D(-1, 0, 0) },
                                                                          { 1, new Vector3D(1, 0, 0) },
                                                                          { 2, new Vector3D(0, -1, 0) },
                                                                          { 3, new Vector3D(0, 1, 0) },
                                                                          { 4, new Vector3D(0, 0, -1) },
                                                                          { 5, new Vector3D(0, 0, 1) }
                                                                      };

        private readonly double size = 5.7;

        // The indices of the faces are
        // 0:Bottom
        // 1:Front
        // 2:Left
        // 3:Right
        // 4:Down
        // 5:Up

        private readonly double spacing = 0.06;

        /// <summary>
        /// This array keeps track of which cubelet is located in each
        /// position (i,j,k) in the cube.
        /// </summary>
        private Model3DGroup[,,] cubelets;

        public RubikCube()
        {
            this.CreateCubelets();
        }

        public int Size
        {
            get => (int)this.GetValue(SizeProperty);
            set => this.SetValue(SizeProperty, value);
        }

        private static Brush CreateFaceBrush(Color c, string text)
        {
            var db = new DrawingBrush
                         {
                             TileMode = TileMode.None,
                             ViewportUnits = BrushMappingMode.Absolute,
                             Viewport = new Rect(0, 0, 1, 1),
                             Viewbox = new Rect(0, 0, 1, 1),
                             ViewboxUnits = BrushMappingMode.Absolute
                         };
            var dg = new DrawingGroup();
            dg.Children.Add(
                new GeometryDrawing { Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1)), Brush = Brushes.Black });
            dg.Children.Add(
                new GeometryDrawing
                    {
                        Geometry = new RectangleGeometry(new Rect(0.05, 0.05, 0.9, 0.9))
                                       {
                                           RadiusX = 0.05, RadiusY = 0.05
                                       },
                        Brush = new SolidColorBrush(c)
                    });

            if (text != null)
            {
                var ft = new FormattedText(
                             text,
                             CultureInfo.CurrentCulture,
                             FlowDirection.LeftToRight,
                             new Typeface("Segoe UI"),
                             0.3,
                             Brushes.Black) { TextAlignment = TextAlignment.Center };
                var geometry = ft.BuildGeometry(new Point(0, -0.2));
                var tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(45));
                tg.Children.Add(new TranslateTransform(0.5, 0.5));
                geometry.Transform = tg;
                dg.Children.Add(new GeometryDrawing { Geometry = geometry, Brush = Brushes.Black });
            }

            db.Drawing = dg;
            return db;
        }

        private static void SizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RubikCube)d).OnSizeChanged();
        }

        private void OnSizeChanged()
        {
            this.CreateCubelets();
        }

        private void CreateCubelets()
        {
            this.Children.Clear();
            this.cubelets = new Model3DGroup[this.Size, this.Size, this.Size];
            var o = -(this.Size - 1) * 0.5 * this.size;
            var faceBrushes = new Brush[this.faceColors.Length];

            for (var i = 0; i < this.faceColors.Length; i++)
            {
                faceBrushes[i] = CreateFaceBrush(this.faceColors[i], null);
                // SolidColorBrush is much faster
                // faceBrushes[i] = new SolidColorBrush(faceColors[i]);
            }

            var brush011 = CreateFaceBrush(Colors.White, "RUBIK");
            // var logobrush = new ImageBrush(new BitmapImage(new Uri(@"logo.png")));

            for (var i = 0; i < this.Size; i++)
            {
                for (var j = 0; j < this.Size; j++)
                {
                    for (var k = 0; k < this.Size; k++)
                    {
                        // the center of the cubelet
                        var center = new Point3D(o + i * this.size, o + j * this.size, o + k * this.size);

                        // add the 6 faces of a cubelet
                        var cubelet = new Model3DGroup();
                        for (var face = 0; face < 6; face++)
                        {
                            // find the color of the face
                            var color = this.IsOutsideFace(face, i, j, k) ? faceBrushes[face] : Brushes.Black;
                            if (face == 0 && i == 0 && j == 1 && k == 1)
                            {
                                color = brush011;
                            }

                            // and add a cube face
                            cubelet.Children.Add(
                                CreateFace(
                                    face,
                                    center,
                                    this.size * (1 - this.spacing),
                                    this.size * (1 - this.spacing),
                                    this.size * (1 - this.spacing),
                                    color));
                        }

                        this.cubelets[i, j, k] = cubelet;
                        this.Children.Add(new ModelVisual3D { Content = cubelet });
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether a face given a position in the cube is on the outside.
        /// If it is not, we will give the face black colour.
        /// </summary>
        private bool IsOutsideFace(int face, int i, int j, int k)
        {
            switch (face)
            {
                case 0:
                    return i == 0;

                case 1:
                    return i == this.Size - 1;

                case 2:
                    return j == 0;

                case 3:
                    return j == this.Size - 1;

                case 4:
                    return k == 0;

                case 5:
                    return k == this.Size - 1;
            }

            return false;
        }

        private static GeometryModel3D CreateFace(
            int face,
            Point3D center,
            double width,
            double length,
            double height,
            Brush brush)
        {
            var m = new GeometryModel3D();
            var b = new MeshBuilder(false, true);
            switch (face)
            {
                case 0:
                    b.AddCubeFace(center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), length, width, height);
                    break;

                case 1:
                    b.AddCubeFace(center, new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), length, width, height);
                    break;

                case 2:
                    b.AddCubeFace(center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), width, length, height);
                    break;

                case 3:
                    b.AddCubeFace(center, new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), width, length, height);
                    break;

                case 4:
                    b.AddCubeFace(center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), height, length, width);
                    break;

                case 5:
                    b.AddCubeFace(center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), height, length, width);
                    break;
            }

            m.Geometry = b.ToMesh();
            m.Material = MaterialHelper.CreateMaterial(brush);
            return m;
        }

        public bool CanUnscramble()
        {
            return this.history.Count > 0;
        }

        public void Scramble()
        {
            var face = this.random.Next(6);
            var rotation = 90;
            if (this.random.Next(2) == 0)
            {
                rotation = -90;
            }

            // push the move into the history
            this.history.Push(new Tuple<int, double>(face, rotation));

            this.Rotate(face, rotation);
        }

        public void Unscramble()
        {
            // pop the last move
            var tuple = this.history.Pop();
            var face = tuple.Item1;

            this.Rotate(face, -tuple.Item2);
        }

        public void Rotate(Key key)
        {
            var control = (Keyboard.IsKeyDown(Key.LeftCtrl));
            var shift = (Keyboard.IsKeyDown(Key.LeftShift));

            double angle = 90;
            if (shift)
            {
                angle = -90;
            }

            if (control)
            {
                angle *= 2;
            }

            int face;
            if (this.FaceKey.TryGetValue(key, out face))
            {
                this.history.Push(new Tuple<int, double>(face, angle));
                this.Rotate(face, angle);
            }
        }

        private DoubleAnimation Rotate(int face, double angle, double animationTime = 200)
        {
            var axis = this.RotationAxis[face];
            DoubleAnimation result = null;

            // we must update the array that contains the position of each cubelet
            var rotatedCubelets = new Model3DGroup[this.Size, this.Size, this.Size];
            for (var i = 0; i < this.Size; i++)
            for (var j = 0; j < this.Size; j++)
            for (var k = 0; k < this.Size; k++)
            {
                rotatedCubelets[i, j, k] = this.cubelets[i, j, k];
            }

            // positive angle is turning clockwise
            // turning face 0: (fix,*,*)

            //  2,0 2,1 2,2      2,2 1,2 0,2
            //  1,0 1,1 1,2  =>  2,1 1,1 0,1
            //  0,0 0,1 0,2      2,0 1,0 0,0

            // if angle is negative we need to rotate
            // the cubelets the other way

            var n = this.Size - 1;

            // this method only supports rotating the outer sides of the cube

            for (var a = 0; a < this.Size; a++)
            {
                for (var b = 0; b < this.Size; b++)
                {
                    var at = b;
                    var bt = n - a;
                    if (angle < 0)
                    {
                        at = n - b;
                        bt = a;
                    }

                    Model3DGroup group = null;
                    switch (face)
                    {
                        case 0:
                            group = rotatedCubelets[0, at, bt] = this.cubelets[0, a, b];
                            break;

                        case 1:
                            group = rotatedCubelets[n, bt, at] = this.cubelets[n, b, a];
                            break;

                        case 2:
                            group = rotatedCubelets[bt, 0, at] = this.cubelets[b, 0, a];
                            break;

                        case 3:
                            group = rotatedCubelets[at, n, bt] = this.cubelets[a, n, b];
                            break;

                        case 4:
                            group = rotatedCubelets[at, bt, 0] = this.cubelets[a, b, 0];
                            break;

                        case 5:
                            group = rotatedCubelets[bt, at, n] = this.cubelets[b, a, n];
                            break;

                        default:
                            continue;
                    }

                    var rot = new AxisAngleRotation3D { Axis = axis };
                    var anim = new DoubleAnimation(angle, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                                   {
                                       AccelerationRatio = 0.3, DecelerationRatio = 0.5
                                   };

                    rot.BeginAnimation(AxisAngleRotation3D.AngleProperty, anim);
                    if (result == null)
                    {
                        result = anim;
                    }

                    var rott = new RotateTransform3D(rot);
                    var gt = new Transform3DGroup();
                    gt.Children.Add(group.Transform);
                    gt.Children.Add(rott);
                    group.Transform = gt;
                }
            }

            this.cubelets = rotatedCubelets;

            // can subscribe to the Completed event on this
            return result;
        }
    }
}