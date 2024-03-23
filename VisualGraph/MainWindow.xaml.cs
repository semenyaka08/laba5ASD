using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphExploring;

namespace VisualGraph;

public partial class MainWindow 
{
    private readonly Graph _graph = new Graph(true);
    
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
        
        DisplayMatrix(_graph);
        
        var dictionary = new Dictionary<Vertex, Coordinates>();
        
        ArrangeVerticesInCircle(683, 352, 300, dictionary);
        
        foreach (var vertex in _graph.Vertices)
        {
            foreach (var edge in vertex.Edges)
            {
                DrawEdge(edge, dictionary);
                
            }
        }
    }
    
    private void DisplayMatrix(Graph graph)
    {
        var matrix = graph.GetMatrix();
        
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write(matrix[i, j] + "  ");
            }
            Console.WriteLine();
        }
    }
    
    private void ArrangeVerticesInCircle(double centreX,double centreY,int radius, Dictionary<Vertex, Coordinates> dictionary)
    {
        int angleIncrement = 360 / _graph.Vertices.Count;
        for (int i = 0; i < _graph.Vertices.Count; i++)
        {
            double angle = i * angleIncrement;
            double angleRad = angle * Math.PI / 180; // Угол в радианах

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
        
        var line = new Line
        {
            X1 = dictionary[edge.From].X + k*(dictionary[edge.To].X - dictionary[edge.From].X),
            Y1 = dictionary[edge.From].Y + k*(dictionary[edge.To].Y - dictionary[edge.From].Y),
            X2 = dictionary[edge.To].X + k*(dictionary[edge.From].X - dictionary[edge.To].X),
            Y2 = dictionary[edge.To].Y + k*(dictionary[edge.From].Y - dictionary[edge.To].Y),
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        Canvas.Children.Add(line);

        if (_graph.IsDirected)
        {
            double angle = Math.Atan2(line.Y2 - line.Y1, line.X2 - line.X1) * (180 / Math.PI);
            
            Polygon arrowhead = new Polygon
            {
                Points = new PointCollection { new Point(line.X2, line.Y2), new Point(line.X2 - 10, line.Y2 - 5), new Point(line.X2 - 10, line.Y2 + 5) }, // Треугольник
                Fill = Brushes.Black, 
                StrokeThickness = 0,
                RenderTransform = new RotateTransform(angle, line.X2, line.Y2)
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