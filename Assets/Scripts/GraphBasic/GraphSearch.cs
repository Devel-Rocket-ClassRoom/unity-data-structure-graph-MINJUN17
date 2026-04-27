using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();
    public void Init(Graph graph)
    {
        this.graph = graph;
    }
    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                stack.Push(adjacent);
            }
        }
    }
    public void BFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
    }
    public void DFSRecrusive(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        DFSRecrusive(node, visited);
    }
    protected void DFSRecrusive(GraphNode node, HashSet<GraphNode> visited)
    {
        visited.Add(node);
        path.Add(node);
        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent))
            {
                continue;
            }
            DFSRecrusive(adjacent, visited);
        }
    }
    public bool pathFindingBFS(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var queue = new Queue<GraphNode>();
        var visited = new HashSet<GraphNode>();

        queue.Enqueue(start);
        visited.Add(start);

        bool success = false;
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode == end)
            {
                success = true;
                break;
            }

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                adjacent.previous = currentNode;
                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
        if (!success)
        {
            return false;
        }
        var temp = end;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;
    }

    public bool Dijkstra(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pqueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];

        for (int i = 0; i < graph.nodes.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        pqueue.Enqueue(start, start.weight);

        bool success = false;
        while (pqueue.Count > 0)
        {
            var currentNode = pqueue.Dequeue();
            if (visited.Contains(currentNode))
            {
                continue;
            }
            visited.Add(currentNode);
            if (currentNode == end)
            {
                success = true;
                break;
            }
            for (int i = 0; i < currentNode.adjacents.Count; i++)
            {
                var adjacent = currentNode.adjacents[i];
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                int newDistance = distances[currentNode.id] + adjacent.weight;

                if (newDistance < distances[adjacent.id])
                {
                    adjacent.previous = currentNode;
                    distances[adjacent.id] = newDistance;
                    pqueue.Enqueue(adjacent, newDistance);
                }
            }
        }

        if (!success)
            return false;

        var temp = end;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;
    }

    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public bool AStar(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pqueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];

        for (int i = 0; i < graph.nodes.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        pqueue.Enqueue(start, start.weight + Heuristic(start, end));

        bool success = false;
        while (pqueue.Count > 0)
        {
            var currentNode = pqueue.Dequeue();
            if (visited.Contains(currentNode))
            {
                continue;
            }
            visited.Add(currentNode);

            if (currentNode == end)
            {
                success = true;
                break;
            }
            for (int i = 0; i < currentNode.adjacents.Count; i++)
            {
                var adjacent = currentNode.adjacents[i];
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                int newDistance = distances[currentNode.id] + adjacent.weight;

                if (newDistance < distances[adjacent.id])
                {
                    adjacent.previous = currentNode;
                    distances[adjacent.id] = newDistance;
                    pqueue.Enqueue(adjacent, newDistance + Heuristic(adjacent, end));
                }
            }
        }

        if (!success)
            return false;

        var temp = end;
        while (temp != null)
        {
            path.Add(temp);
            temp = temp.previous;
        }
        path.Reverse();
        return true;

    }
}
