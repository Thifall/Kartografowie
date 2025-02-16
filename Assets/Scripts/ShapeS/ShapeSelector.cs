using System.Collections.Generic;
using UnityEngine;

public class ShapeSelector : MonoBehaviour
{

    public ShapeSelectedEventSO shapeSelectedEvent;
    public Transform previewParent;
    private GameObject currentGhostShape;

    public GameObject[] shapePrefabs;
    private Dictionary<string, GameObject> shapeDictionary;

    private void Awake()
    {
        shapeDictionary = new Dictionary<string, GameObject>();
        foreach (var shape in shapePrefabs)
        {
            var shapeComponent = shape.GetComponent<Shape>();
            Debug.Log(shapeComponent.name);
            shapeDictionary[shapeComponent.Icon.name] = shape;
        }
    }

    private void OnEnable() => shapeSelectedEvent.OnShapeSelected += UpdateSelectedShape;
    private void OnDisable() => shapeSelectedEvent.OnShapeSelected -= UpdateSelectedShape;

    private void UpdateSelectedShape(Sprite sprite)
    {
        currentGhostShape = shapeDictionary[sprite.name];
    }

    public void ResetShape()
    {
        currentGhostShape = null;
    }

    public GameObject GetSelectedShape()
    {
        return currentGhostShape;
    }
}
