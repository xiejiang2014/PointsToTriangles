using System.Collections.Generic;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Meshing.Algorithm;
using TriangleNet.Meshing.Iterators;

namespace PointsToTriangles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ButtonBase_OnClick(null, null);
        }

        private List<Vertex> _points = new();

        private IMesh? _mesh;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // Generate points.
            var points = Generate.RandomPoints(50, new Rectangle(0, 0, 100, 100));

            _mesh = GetMesh1(points);
            SKElement.InvalidateVisual();
        }

        private void SKElement_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_mesh is null)
            {
                return;
            }

            e.Surface.Canvas.Clear(SKColors.White);


            e.Surface.Canvas.Scale(10, 10);


            //绘制所有顶点
            using var paintVertex = new SKPaint()
                                    {
                                        IsAntialias = true,
                                        IsStroke    = false,
                                        Color       = SKColors.LightGreen
                                    };

            foreach (var vertex in _mesh.Vertices)
            {
                e.Surface.Canvas.DrawCircle((float)vertex.X, (float)vertex.Y, 0.4f, paintVertex);
            }


            //绘制所有三角形的边线
            using var paintEdge = new SKPaint()
                                  {
                                      IsAntialias = true,
                                      IsStroke    = true,
                                      StrokeWidth = 0.1f,
                                      Color       = SKColors.IndianRed
                                  };


            foreach (var meshEdge in EdgeIterator.EnumerateEdges(_mesh))
            {
                var p0 = meshEdge.GetVertex(0);
                var p1 = meshEdge.GetVertex(1);


                e.Surface.Canvas.DrawLine((float)p0.X, (float)p0.Y,
                                          (float)p1.X, (float)p1.Y,
                                          paintEdge);
            }


            //foreach (var tri in mesh.Triangles)
            //{
            //    var v1 = tri.GetVertex(0);
            //    var v2 = tri.GetVertex(1);
            //    var v3 = tri.GetVertex(2);


            //    e.Surface.Canvas.DrawLine((float)v1.X, (float)v1.Y,
            //                              (float)v2.X, (float)v2.Y,
            //                              paintEdge);
            //    e.Surface.Canvas.DrawLine((float)v3.X, (float)v3.Y,
            //                              (float)v2.X, (float)v2.Y,
            //                              paintEdge);
            //    e.Surface.Canvas.DrawLine((float)v1.X, (float)v1.Y,
            //                              (float)v3.X, (float)v3.Y,
            //                              paintEdge);
            //}
        }


        private static IMesh GetMesh2(List<Vertex> points)
        {
            // We use a polygon as input to enable segment insertion on the convex hull.
            var poly = new Polygon(points.Count);

            poly.Points.AddRange(points);

            // Set the 'convex' option to enclose the convex hull with segments.
            var options = new ConstraintOptions() { Convex = true };

            // Generate mesh.
            var mesh = poly.Triangulate(options);

            return mesh;
        }

        private static IMesh GetMesh1(List<Vertex> points)
        {
            // Choose triangulator: Incremental, SweepLine or Dwyer.
            //var triangulator = new SweepLine();
            var triangulator = new Dwyer();
            //var triangulator = new Incremental();

            // Generate mesh.

            var predicates = new RobustPredicates();
            var pool       = new TrianglePool();
            var mesh = triangulator.Triangulate(points, new Configuration()
                                                        {
                                                            //Predicates   = () => predicates,
                                                            //TrianglePool = () => pool.Restart(),
                                                            //RandomSource = () => Random.Shared
                                                        });
            return mesh;
        }
    }
}