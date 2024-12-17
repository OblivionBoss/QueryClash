using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildingStateR
{
    void Endstate();
    void OnAction(Vector3Int gridPosition);
    void UpdateState(Vector3Int gridPosition);
}
