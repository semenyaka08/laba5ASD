namespace GraphExploring;

public class Edge
{
    public Edge(Vertex vertex1, Vertex vertex2)
    {
        From = vertex1;
        To = vertex2;
    }
    public Vertex From { get; set; }

    public Vertex To { get; set; }

    public int Weight { get; set; } = 1;
}