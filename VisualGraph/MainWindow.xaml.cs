﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphExploring;
using System.Threading.Tasks;

namespace VisualGraph;

public partial class MainWindow
{
    private readonly Graph _graph = new Graph(true);
    
    private readonly Dictionary<int, Ellipse> _drawnCircles = new Dictionary<int, Ellipse>();
    
    private readonly Dictionary<(Vertex, Vertex), Line> _drawnLines = new Dictionary<(Vertex,Vertex), Line>();
    
    private readonly Dictionary<(Vertex, Vertex), Path> _drawnCurves = new Dictionary<(Vertex,Vertex), Path>();

    
    private bool _flag = false;
    
    public MainWindow()
    {
        InitializeComponent();
        _graph.AddVertex(1);
        _graph.AddVertex(2);
        _graph.AddVertex(3);
        _graph.AddVertex(4);
        _graph.AddVertex(5);
        _graph.AddVertex(6);
        _graph.AddVertex(7);
        _graph.AddVertex(8);
        _graph.AddVertex(9);
        _graph.AddVertex(10);
        _graph.AddVertex(11);
        _graph.AddVertex(12);
        
        _graph.GenerateMatrix();
        
        var dictionary = new Dictionary<Vertex, Coordinates>();
        
        ArrangeVerticesInCircle(683, 352, 300, dictionary);
        
        foreach (var vertex in _graph.Vertices)
        {
            foreach (var edge in vertex.Edges)
            {
                DrawEdge(edge, dictionary);
            }
        }

        var startDfsButton = new Button
        {
            Content = "StartDfsButton",
            Width = 115,
            Height = 50,
        };
        var nextStepButton = new Button
        {
            Content = "NextStepDFS",
            Width = 115,
            Height = 50,
        };
        startDfsButton.Click += StartDFSButton_Click;
        nextStepButton.Click += NextStepButton_Click;
        Canvas.SetLeft(nextStepButton, 50);
        Canvas.SetTop(nextStepButton, 140);
        Canvas.SetLeft(startDfsButton, 50);
        Canvas.SetTop(startDfsButton, 50);
        Canvas.Children.Add(startDfsButton);
        Canvas.Children.Add(nextStepButton);
        
    }
    
    private void UpdateCircleColor(Vertex vertex, Brush color)
    {
        Dispatcher.Invoke(() =>
        {
            _drawnCircles[vertex.CurrentId].Fill = color;
        });
    }
    private void UpdateLineColor((Vertex,Vertex) edge, Brush color)
    {
        Dispatcher.Invoke(() =>
        {
            try
            {
                _drawnLines[edge].Stroke = color;
            }
            catch (Exception e)
            {
                _drawnCurves[edge].Stroke = color;
            }
        });
    }
    
    private async void StartDFSButton_Click(object sender, RoutedEventArgs e)
    {
        await Task.Run(() => DfsSearch(_graph.Vertices[0]));
    }
    private void NextStepButton_Click(object sender, RoutedEventArgs e)
    {
        _flag = true;
    }
    public void DfsSearch(Vertex startVertex)
    {
        var edge = default((Vertex, Vertex));
        var visited = new Dictionary<Vertex, bool>();
        DfsSearchRecur(startVertex, visited, edge);
    }

    private void DfsSearchRecur(Vertex  vertex, Dictionary<Vertex, bool> visited, (Vertex,Vertex) edge)
    {
        edge.Item1 = vertex;
        Console.WriteLine(vertex.Value);
        UpdateCircleColor(vertex, Brushes.Green);
        while (true)
        {
            if(_flag) break;
        }
        _flag = false;
        visited.Add(vertex, true);
        foreach (var t in vertex.Edges)
        {
            if (visited.ContainsKey(t.To))
                continue;
            edge.Item2 = t.To;
            UpdateLineColor(edge, Brushes.Red);
            DfsSearchRecur(t.To, visited, edge);
        }
    }
    
    
    
    
    
    
    private void ArrangeVerticesInCircle(double centreX,double centreY,int radius, Dictionary<Vertex, Coordinates> dictionary)
    {
        int angleIncrement = 360 / _graph.Vertices.Count;
        for (int i = 0; i < _graph.Vertices.Count; i++)
        {
            double angle = i * angleIncrement;
            double angleRad = angle * Math.PI / 180; 

            double x = centreX + radius * Math.Cos(angleRad);
            double y = centreY + radius * Math.Sin(angleRad);

            Coordinates coordinates = new Coordinates(x, y);
            dictionary.Add(_graph.Vertices.First(p=>p.CurrentId == i+1), coordinates);
            DrawVertex(coordinates, _graph.Vertices.First(p=>p.CurrentId == i+1));
        }
    }
    
    private void DrawVertex(Coordinates coordinates, Vertex vertex)
    {
        var ellipse = new Ellipse
        {
            Width = 30,
            Height = 30,
            Fill = Brushes.LightBlue,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        _drawnCircles.Add(vertex.CurrentId, ellipse);
        
        Canvas.SetLeft(ellipse, coordinates.X - 15);
        Canvas.SetTop(ellipse, coordinates.Y - 15);
        Canvas.Children.Add(ellipse);
        
        var textBlock = new TextBlock
        {
            Text = vertex.Value.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        Canvas.Children.Add(textBlock);
        
        Canvas.SetLeft(textBlock, (coordinates.X + ellipse.Width / 2 - textBlock.ActualWidth / 2)-19);
        Canvas.SetTop(textBlock, (coordinates.Y + ellipse.Height / 2 - textBlock.ActualHeight / 2)-24);
    }

    private void DrawEdge(Edge edge, Dictionary<Vertex, Coordinates> dictionary)
    {
        if (edge.From.CurrentId == edge.To.CurrentId)
        {
            var ellipse = new Ellipse()
            {
                Width = 15,
                Height = 15,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            
            const double koef = 322.5 / 300;
        
            Canvas.SetLeft(ellipse, (683 +(dictionary[edge.To].X-683)*koef)-7.5);
            Canvas.SetTop(ellipse, (352 + (dictionary[edge.To].Y-352)*koef)-7.5);
            Canvas.Children.Add(ellipse);
            return;
        }
        
        double lineLength = Math.Sqrt(Math.Pow((dictionary[edge.To].X - dictionary[edge.From].X), 2) +
                                      Math.Pow(dictionary[edge.To].Y - dictionary[edge.From].Y, 2));
        double k = 15 / lineLength;

        
        if (_graph.IsDirected && _graph.Edges.Any( z => z.To == edge.From && z.From == edge.To))
        {
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 2;
                Point startPoint = new Point(dictionary[edge.From].X + k*(dictionary[edge.To].X - dictionary[edge.From].X), dictionary[edge.From].Y + k*(dictionary[edge.To].Y - dictionary[edge.From].Y));
                Point endPoint = new Point(dictionary[edge.To].X + k*(dictionary[edge.From].X - dictionary[edge.To].X), dictionary[edge.To].Y + k*(dictionary[edge.From].Y - dictionary[edge.To].Y));
                Point center = new Point((startPoint.X + endPoint.X)/2, (startPoint.Y + endPoint.Y)/2);
                double dx = endPoint.X - startPoint.X;
                double dy = startPoint.Y - endPoint.Y;
                double length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                double xGrow = dx / length;
                double yGrow = dy / length;
                
                Point middlePoint = new Point(center.X - (50 * yGrow), center.Y - (50*xGrow));
                
                PathGeometry pathGeometry = new PathGeometry();
                
                PathFigure pathFigure = new PathFigure();
                
                pathFigure.StartPoint = startPoint; 
            
                BezierSegment bezierSegment = new BezierSegment(
                    startPoint,
                    middlePoint,
                    endPoint,
                    true);
                
                pathFigure.Segments.Add(bezierSegment);
                pathGeometry.Figures.Add(pathFigure);
                path.Data = pathGeometry;
                _drawnCurves.Add((edge.To, edge.From),path);
                Canvas.Children.Add(path);
        }

        else
        {
            var line = new Line
            {
                X1 = dictionary[edge.From].X + k * (dictionary[edge.To].X - dictionary[edge.From].X),
                Y1 = dictionary[edge.From].Y + k * (dictionary[edge.To].Y - dictionary[edge.From].Y),
                X2 = dictionary[edge.To].X + k * (dictionary[edge.From].X - dictionary[edge.To].X),
                Y2 = dictionary[edge.To].Y + k * (dictionary[edge.From].Y - dictionary[edge.To].Y),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            _drawnLines.Add((edge.From, edge.To), line);
            Canvas.Children.Add(line);
        }

        if (_graph.IsDirected)
        {
            double y2 = dictionary[edge.To].Y + k * (dictionary[edge.From].Y - dictionary[edge.To].Y);
            double y1 = dictionary[edge.From].Y + k * (dictionary[edge.To].Y - dictionary[edge.From].Y);
            double x1 = dictionary[edge.From].X + k * (dictionary[edge.To].X - dictionary[edge.From].X);
            double x2 = dictionary[edge.To].X + k * (dictionary[edge.From].X - dictionary[edge.To].X);
            
            double angle = Math.Atan2(y2 - y1, x2 - x1) * (180 / Math.PI);
            if (_graph.IsDirected && _graph.Edges.Any(z => z.To == edge.From && z.From == edge.To))
            {
                angle -= 15;
            }
            
            Polygon arrowhead = new Polygon
            {
                Points = new PointCollection { new Point(x2, y2), new Point(x2 - 10, y2 - 5), new Point(x2 - 10, y2 + 5) }, // Треугольник
                Fill = Brushes.Black, 
                StrokeThickness = 0,
                RenderTransform = new RotateTransform(angle, x2, y2)
            };

            Canvas.Children.Add(arrowhead);
        }
    }
    
    
}

public struct Coordinates
{
    public double X { get; }
    
    public double Y { get; }
    
    public Coordinates(double x, double y)
    {
        X = x;
        Y = y;
    }
}