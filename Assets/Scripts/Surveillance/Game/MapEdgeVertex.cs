using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using UnityEngine;

public class MapEdgeVertex : MonoBehaviour
{
    private Source _source = null;

    public Source GetSource()
    {
        return _source;
    }

    public void RemoveSource()
    {
        if (_source != null)
        {
            Destroy(_source);
        }

        _source = null;
    } 
    
    private void OnMouseEnter()
    {
        Debug.Log("Mouse entered this vertex!");
    }

    public void SetSource(Source source)
    {
        _source = source;
    }
}
