using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueryMaterial
{
    private int stat;
    private QueryMaterialType type;

    public QueryMaterial(QueryMaterialType type)
    {
        stat = 10;
        this.type = type;
    }
}
