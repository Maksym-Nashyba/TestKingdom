using System;
using UnityEngine;

[RequireComponent(typeof(Grid))]
internal sealed class Map : MonoBehaviour
{
    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }
}
