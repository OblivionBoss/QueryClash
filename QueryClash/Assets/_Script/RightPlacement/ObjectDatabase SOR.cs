using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ObjectDatabaseSOR : ScriptableObject
{ 
    public List<ObjectDataR> objectsData;

}


[Serializable]
public class ObjectDataR
{

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Vector2Int Size { get; private set; }

    [field: SerializeField]
    public GameObject Prefab { get; private set; }


}
