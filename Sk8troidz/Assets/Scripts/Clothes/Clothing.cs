using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new_clothes", menuName = "Scripts/Clothes", order = 1)]

public class Clothing : ScriptableObject
{
    
    public Texture icon; //for display in inventory
    public Mesh mesh;
    public Material material;
    public Type type;
    //Leave blank unless clothing is a shirt
    public Mesh sleeveL_mesh;
    public Material sleeveL_mat;
    public Mesh sleeveR_mesh;
    public Material sleeveR_mat;

    public enum Type
    {
        Top, Shirt, Pants, Shoes 
    }
    
}
