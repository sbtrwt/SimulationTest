using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{NONE,CUBE,SPHERE,CYLINDER}
public class ReferigeratorItem : MonoBehaviour
{
    public bool IsExpired;
    public bool IsOutside;
    [SerializeField] public ItemType ItemType;
    public bool IsRightPlace;

    private void Start()
    {
        if (IsExpired)
            ChangeMeshColors(Color.red);
    }

    private void ChangeMeshColors(Color colorToSet)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = colorToSet;
        }
        else
        {
            Debug.LogWarning("MeshRenderer component not found on ball.");
        }
    }
}
