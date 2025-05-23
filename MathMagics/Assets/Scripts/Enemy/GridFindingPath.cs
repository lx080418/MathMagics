using System.Collections.Generic;
using UnityEngine;

public static class GridPathfinding
{
    private static readonly Vector2[] directions = {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    public static List<Vector3> FindPath(Vector3 startPos, Vector3 endPos, int maxSteps = 100)
    {
        Queue<Node> openSet = new Queue<Node>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

        Vector3 start = Round(startPos);
        Vector3 goal = Round(endPos);

        openSet.Enqueue(new Node(start, 0));
        visited.Add(start);

        while (openSet.Count > 0 && maxSteps-- > 0)
        {
            Node current = openSet.Dequeue();

            if (current.position == goal)
                return ReconstructPath(cameFrom, goal);

            foreach (var dir in directions)
            {
                Vector3 neighbor = current.position + new Vector3(dir.x, dir.y, 0f);
                if (visited.Contains(neighbor)) continue;

                Collider2D hit = Physics2D.OverlapBox(neighbor, Vector2.one * 0.8f, 0f);
                if (hit != null && hit.CompareTag("Wall")) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current.position;
                openSet.Enqueue(new Node(neighbor, current.cost + 1));
            }
        }

        return null;
    }

    private static List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private static Vector3 Round(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
    }

    private class Node
    {
        public Vector3 position;
        public int cost;
        public Node(Vector3 pos, int c)
        {
            position = pos;
            cost = c;
        }
    }
}
