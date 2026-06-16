namespace OptimizationDemo;

public class AVLTree
{
    private class Node
    {
        public int Value;
        public Node? Left;
        public Node? Right;
        public int Height;

        public Node(int value)
        {
            Value = value;
            Height = 1;
        }
    }

    private Node? _root;

    public void Insert(int value)
    {
        _root = Insert(_root, value);
    }

    private Node Insert(Node? node, int value)
    {
        if (node == null)
            return new Node(value);

        if (value < node.Value)
            node.Left = Insert(node.Left, value);
        else if (value > node.Value)
            node.Right = Insert(node.Right, value);
        else
            return node; // Duplicados no permitidos

        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        return Balance(node, value);
    }

    private Node Balance(Node node, int value)
    {
        int balance = GetBalance(node);

        // Rotación izquierda-izquierda
        if (balance > 1 && value < node.Left!.Value)
            return RotateRight(node);

        // Rotación derecha-derecha
        if (balance < -1 && value > node.Right!.Value)
            return RotateLeft(node);

        // Rotación izquierda-derecha
        if (balance > 1 && value > node.Left!.Value)
        {
            node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }

        // Rotación derecha-izquierda
        if (balance < -1 && value < node.Right!.Value)
        {
            node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }

        return node;
    }

    private Node RotateRight(Node y)
    {
        Node x = y.Left!;
        Node? T2 = x.Right;

        x.Right = y;
        y.Left = T2;

        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));

        return x;
    }

    private Node RotateLeft(Node x)
    {
        Node y = x.Right!;
        Node? T2 = y.Left;

        y.Left = x;
        x.Right = T2;

        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));

        return y;
    }

    public bool Search(int value)
    {
        var current = _root;
        while (current != null)
        {
            if (value == current.Value) return true;
            if (value < current.Value) current = current.Left;
            else current = current.Right;
        }
        return false;
    }

    public int Height() => GetHeight(_root);

    private int GetHeight(Node? node) => node?.Height ?? 0;
    private int GetBalance(Node? node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
}