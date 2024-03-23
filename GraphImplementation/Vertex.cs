namespace GraphExploring;

public class Vertex
{
    public Vertex(int value)
    {
        Value = value;
        CurrentId = _id;
        _id++;
    }
    public readonly int CurrentId;

    private static int _id = 1;
    public int Value { get; set; }

    public List<Edge> Edges = new List<Edge>();
}