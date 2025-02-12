using System;
using UnityEngine;

public class ShapeSelector : MonoBehaviour
{
    public GameObject[] shapePrefabs;
    private int selectedShapeIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SelectShape(++selectedShapeIndex);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SelectShape(--selectedShapeIndex);
    }

    void SelectShape(int index)
    {
        if(shapePrefabs is null) return;
        if (index < 0)
        {
            index = shapePrefabs.Length - 1;
        }
        if (index >= shapePrefabs.Length)
        {
            index = 0;
        }
        selectedShapeIndex = index;
        Debug.Log("Wybrano kszta³t: " + shapePrefabs[selectedShapeIndex].name);
    }

    public GameObject GetSelectedShape()
    {
        return shapePrefabs[selectedShapeIndex];
    }

    internal void SetAvailableShapes(GameObject[] availableShapes)
    {
        shapePrefabs = availableShapes;
        selectedShapeIndex = 0;
    }
}
