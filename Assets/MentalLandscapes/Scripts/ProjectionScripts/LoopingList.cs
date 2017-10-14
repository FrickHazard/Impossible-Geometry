using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoopingList<T> {
    protected List<T> list = new List<T>();
    public int Count { get { return list.Count; } }
    public List<T> List { get { return list; } }
    public void AddItem(T item) {list.Add(item);}

    public T AcsessItem(int index) {

        return list[mod(index,list.Count)]; 
    }

    public void Clear() { list.Clear(); }

    public static int mod(int x, int m) {
        if (m == 0) { return 0; }
       return (x % m + m) % m;
    }

}
