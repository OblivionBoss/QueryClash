using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum QueryMaterialType
{
    Frontline,
    Sniper,
    Shielder
}

public class QueryMaterialManager : MonoBehaviour
{
    [SerializeField] private int randomSeed;
    [SerializeField] private QueryMaterial[] queryMaterials;

    public QueryMaterial GenerateQueryMaterial(Image materialSlot)
    {
        int rand = UnityEngine.Random.Range(0, queryMaterials.Length);
        materialSlot.sprite = queryMaterials[rand].icon;
        return queryMaterials[rand];
        //return new QueryMaterial(Instantiate(queryMaterials[rand], materialSlot), (QueryMaterialType)values.GetValue(rand));
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(randomSeed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}