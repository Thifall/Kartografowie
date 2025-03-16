using System.Collections.Generic;
using UnityEngine;

public class ShapeSelector : MonoBehaviour
{

    public ShapeSelectedEventSO shapeSelectedEvent;
    public ForceSingleSquareEventSO ForceSingleSquareEvent;
    public Transform previewParent;
    public GameObject[] shapePrefabs;

    private Dictionary<string, GameObject> shapeDictionary;
    private GameObject currentGhostShape;
    private bool shapeWasForced;

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

    private void OnEnable()
    {
        shapeSelectedEvent.OnShapeSelected += UpdateSelectedShape;
        ForceSingleSquareEvent.OnForceSingleSquare += OnForceSingleSquare;
    }

    private void OnDisable()
    {
        shapeSelectedEvent.OnShapeSelected -= UpdateSelectedShape;
        ForceSingleSquareEvent.OnForceSingleSquare -= OnForceSingleSquare;
    }

    private void OnForceSingleSquare()
    {
        currentGhostShape = shapeDictionary["singlesquare"];
        shapeWasForced = true;
    }

    private void UpdateSelectedShape(Sprite sprite)
    {
        if(shapeWasForced) { return; }
        currentGhostShape = shapeDictionary[sprite.name];
    }

    public void ResetShape()
    {
        currentGhostShape = null;
        shapeWasForced = false;
    }

    public GameObject GetSelectedShape()
    {
        return currentGhostShape;
    }
}
