using System;
using System.Collections.Generic;
using System.Linq;

public class BTree<K, V> : IIndex<K, V> where K : IComparable
{
    private class Node
    {
        public bool IsLeaf;
        public List<K> Keys;
        public List<V> Values;
        public List<Node> Children;

        public Node(bool isLeaf)
        {
            IsLeaf = isLeaf;
            Keys = new List<K>();
            Values = new List<V>();
            Children = new List<Node>();
        }
    }

    private Node _root;
    private int _t;

    public BTree(int t)
    {
        _root = new Node(true);
        _t = t;
    }

    public void Insert(K key, V value)
    {
        Node r = _root;
        if (r.Keys.Count == 2 * _t - 1)
        {
            Node s = new Node(false);
            _root = s;
            s.Children.Add(r);
            SplitChild(s, 0);
        }
        InsertNonFull(_root, key, value);
    }

    private void SplitChild(Node x, int i)
    {
        Node y = x.Children[i];
        Node z = new Node(y.IsLeaf);
        x.Children.Insert(i + 1, z);
        x.Keys.Insert(i, y.Keys[_t - 1]);
        
        for (int j = 0; j < _t - 1; j++)
        {
            z.Keys.Add(y.Keys[_t + j]);
        }
        y.Keys.RemoveRange(_t - 1, _t);
        
        if (!y.IsLeaf)
        {
            for (int j = 0; j < _t; j++)
            {
                z.Children.Add(y.Children[_t]);
                y.Children.RemoveAt(_t);
            }
        }
    }

    private void InsertNonFull(Node x, K key, V value)
    {
        int i = x.Keys.Count - 1;
        if (x.IsLeaf)
        {
            x.Keys.Add(default(K));
            x.Values.Add(default(V));
            while (i >= 0 && key.CompareTo(x.Keys[i]) < 0)
            {
                x.Keys[i + 1] = x.Keys[i];
                x.Values[i + 1] = x.Values[i];
                i--;
            }
            x.Keys[i + 1] = key;
            x.Values[i + 1] = value;
        }
        else
        {
            while (i >= 0 && key.CompareTo(x.Keys[i]) < 0)
            {
                i--;
            }
            i++;
            if (x.Children[i].Keys.Count == 2 * _t - 1)
            {
                SplitChild(x, i);
                if (key.CompareTo(x.Keys[i]) > 0)
                {
                    i++;
                }
            }
            InsertNonFull(x.Children[i], key, value);
        }
    }

    public Tuple<K, V> Get(K key)
    {
        return Search(_root, key);
    }

    private Tuple<K, V> Search(Node x, K key)
    {
        int i = 0;
        while (i < x.Keys.Count && key.CompareTo(x.Keys[i]) > 0)
        {
            i++;
        }
        if (i < x.Keys.Count && key.CompareTo(x.Keys[i]) == 0)
        {
            return new Tuple<K, V>(x.Keys[i], x.Values[i]);
        }
        if (x.IsLeaf)
        {
            return null;
        }
        else
        {
            return Search(x.Children[i], key);
        }
    }

    public IEnumerable<Tuple<K, V>> LargerThanOrEqualTo(K key)
    {
        List<Tuple<K, V>> results = new List<Tuple<K, V>>();
        FindLargerThanOrEqualTo(_root, key, results);
        return results;
    }

    private void FindLargerThanOrEqualTo(Node x, K key, List<Tuple<K, V>> results)
    {
        int i = 0;
        while (i < x.Keys.Count && key.CompareTo(x.Keys[i]) > 0)
        {
            i++;
        }
        for (int j = i; j < x.Keys.Count; j++)
        {
            if (key.CompareTo(x.Keys[j]) <= 0)
            {
                results.Add(new Tuple<K, V>(x.Keys[j], x.Values[j]));
            }
        }
        if (!x.IsLeaf)
        {
            for (int j = i; j <= x.Keys.Count; j++)
            {
                FindLargerThanOrEqualTo(x.Children[j], key, results);
            }
        }
    }

    public IEnumerable<Tuple<K, V>> LargerThan(K key)
    {
        List<Tuple<K, V>> results = new List<Tuple<K, V>>();
        FindLargerThan(_root, key, results);
        return results;
    }

    private void FindLargerThan(Node x, K key, List<Tuple<K, V>> results)
    {
        int i = 0;
        while (i < x.Keys.Count && key.CompareTo(x.Keys[i]) >= 0)
        {
            i++;
        }
        for (int j = i; j < x.Keys.Count; j++)
        {
            if (key.CompareTo(x.Keys[j]) < 0)
            {
                results.Add(new Tuple<K, V>(x.Keys[j], x.Values[j]));
            }
        }
        if (!x.IsLeaf)
        {
            for (int j = i; j <= x.Keys.Count; j++)
            {
                FindLargerThan(x.Children[j], key, results);
            }
        }
    }

    public IEnumerable<Tuple<K, V>> LessThanOrEqualTo(K key)
    {
        List<Tuple<K, V>> results = new List<Tuple<K, V>>();
        FindLessThanOrEqualTo(_root, key, results);
        return results;
    }

    private void FindLessThanOrEqualTo(Node x, K key, List<Tuple<K, V>> results)
    {
        int i = 0;
        while (i < x.Keys.Count && key.CompareTo(x.Keys[i]) <= 0)
        {
            results.Add(new Tuple<K, V>(x.Keys[i], x.Values[i]));
            i++;
        }
        if (!x.IsLeaf)
        {
            for (int j = 0; j <= i; j++)
            {
                FindLessThanOrEqualTo(x.Children[j], key, results);
            }
        }
    }

    public IEnumerable<Tuple<K, V>> LessThan(K key)
    {
        List<Tuple<K, V>> results = new List<Tuple<K, V>>();
        FindLessThan(_root, key, results);
        return results;
    }

    private void FindLessThan(Node x, K key, List<Tuple<K, V>> results)
    {
        int i = 0;
        while (i < x.Keys.Count && key.CompareTo(x.Keys[i]) < 0)
        {
            results.Add(new Tuple<K, V>(x.Keys[i], x.Values[i]));
            i++;
        }
        if (!x.IsLeaf)
        {
            for (int j = 0; j <= i; j++)
            {
                FindLessThan(x.Children[j], key, results);
            }
        }
    }

    public bool Delete(K key, V value, IComparer<V> valueComparer = null)
    {
        throw new NotImplementedException();
    }

    public bool Delete(K key)
    {
        throw new NotImplementedException();
    }
}