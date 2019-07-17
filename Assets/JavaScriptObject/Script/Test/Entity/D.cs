using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D<T> {
    public T d = default(T);

    public D()
    {
    }

    public D(T d)
    {
        this.d = d;
    }
}
