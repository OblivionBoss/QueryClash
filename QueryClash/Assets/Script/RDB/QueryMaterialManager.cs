using System;
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
    [SerializeField] private Sprite[] queryMaterials;

    public QueryMaterial GenerateQueryMaterial(Image materialSlot)
    {
        Array values = Enum.GetValues(typeof(QueryMaterialType));
        int rand = UnityEngine.Random.Range(0, values.Length);
        materialSlot.sprite = queryMaterials[rand];
        return new QueryMaterial((QueryMaterialType)values.GetValue(rand));
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