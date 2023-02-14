using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class PathFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GraphArray.Main();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public class GraphArray
{
    private float[,] _adj;
    private int _size;
    List<int> _path = new List<int>();

    public GraphArray(int n)
    {
        _size = n;
        _adj = new float[_size, _size];
    }

    public void AddEdge(int a, int b, float cost = 1)
    {
        try
        {
            _adj[a, b] = cost;
            _adj[b, a] = cost;
        }
        catch (Exception e)
        {
            Debug.Log($"Last Param: a:{a}, b:{b}");

        }
    }

    public List<int> Dijkstra(int start, int dest)
    {
        bool[] visited = new bool[_size];
        float[] distance = new float[_size];
        int[] parent = new int[_size];


        // Array.Fill(distance, Int32.MaxValue);
        for (int i = 0; i < _size; i++)
        {
            distance[i] = float.MaxValue;
        }

        //distance = Enumerable.Repeat(float.MaxValue, _size).ToArray();
        //distance = Enumerable.Repeat(999f, _size).ToArray();


        distance[start] = 0;
        parent[start] = start;

        while (true)
        {
            //인접하면서 제일 가까운 후보를 찾기 
            int now = -1;
            float closest = float.MaxValue;
            for (int i = 0; i < _size; i++)
            {
                //이미 방문했으면 스킵
                if (visited[i])
                    continue;
                //시작점으로 부터 발견된 적 없는 경우
                if (distance[i] == float.MaxValue)
                    continue;

                //가장 가까운 후보를 기억하기
                if (distance[i] < closest)
                {
                    closest = distance[i];
                    now = i;
                }

            }

            if (now == -1)
                break;

            visited[now] = true;

            //방문한 정점과 인접한 정점중
            //최단 거리를 계산해서 갱신한다.
            for (int next = 0; next < _size; next++)
            {
                if (_adj[now, next] == 0)
                    continue;
                if (visited[next])
                    continue;

                float nextDist = distance[now] + _adj[now, next];

                //최단거리 갱신
                if (nextDist < distance[next])
                {
                    distance[next] = nextDist;
                    parent[next] = now;
                }
            }
        }

        CalcPathFromParent(parent, dest);

        return _path;
    }

    void CalcPathFromParent(int[] parent, int dest)
    {
        Debug.Log($"{dest}까지 최단 경로");
        while (parent[dest] != dest)
        {
            _path.Add(dest);
            dest = parent[dest];
        }
        _path.Add(dest);
        _path.Reverse();

        foreach (int n in _path)
        {
            Debug.Log("Path: " + n);
        }
    }

    //private static GraphArray g;

    //public static void Clear(int numEdges)
    //{
    //    g = new GraphArray(numEdges);
    //}

    ////public static void AddTransition(int from, int to, float dest)
    ////{
    ////    g.AddEdge(from, to, dest);
    ////}

    //public static List<int> GetPath(int from, int to)
    //{
    //    return g._path;
    //}

    public static void Main()
    {
        GraphArray g = new GraphArray(6);

        //g.AddEdge(0, 1, 15);
        //g.AddEdge(0, 3, 35);
        //g.AddEdge(1, 2, 15);
        //g.AddEdge(1, 3, 10);
        //g.AddEdge(3, 4, 5);
        //g.AddEdge(4, 5, 5);

        //g.AddEdge(0, 2, 20);
        //g.AddEdge(2, 0, 80);

        g.AddEdge(0, 1, 15);
        //g.AddEdge(1, 3, 15);
        g.AddEdge(1, 2, 999);
        g.AddEdge(2, 5, 99);
        g.AddEdge(3, 5, 15);



        g.Dijkstra(0, 5);
    }
}