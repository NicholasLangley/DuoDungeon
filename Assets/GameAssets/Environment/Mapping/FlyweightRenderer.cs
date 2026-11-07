using System;
using System.Collections.Generic;
using UnityEngine;

public struct FlyweightTransform : IEquatable<FlyweightTransform>
{
    public Vector3 position;
    public Quaternion rotation;

    public FlyweightTransform(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }

    public bool Equals(FlyweightTransform other)
    {
        return position == other.position && rotation == other.rotation;
    }
}

public class FlyweightRenderer : MonoBehaviour
{
    private Mesh _mesh;
    private Material _material;
    List<Matrix4x4> _flyweightMatricies;
    private int _totalCount;
    //can only draw 1023 instances per call
    private int _maxBatchSize = 1023;
    Matrix4x4[] _batchMatricies;
    private HashSet<int> _disabledIndexes;
    
    public void OnEnable()
    {
        _flyweightMatricies = new List<Matrix4x4>();
        _batchMatricies = new Matrix4x4[_maxBatchSize];
        _disabledIndexes = new HashSet<int>();
        _totalCount = _flyweightMatricies.Count;
    }

    public void Update()
    {
        if (_mesh == null || _material == null){return;}

        int batchArrayCounter = 0;
        for (int i = 0; i < _totalCount; i++)
        {
            //skip disabled indexes
            if(!_disabledIndexes.Contains(i))
            {
                //assign instances to batch array
                _batchMatricies[batchArrayCounter] = _flyweightMatricies[i];
                batchArrayCounter++;
                if (batchArrayCounter == _maxBatchSize)
                {
                    //draw
                    SendDrawCall(batchArrayCounter);
                    batchArrayCounter = 0;
                }
            }
        }
        //don't need to clear remaining cells in batch array since we pass in the final batch size
        //final draw
        if (batchArrayCounter > 0)
        {
            SendDrawCall(batchArrayCounter);
        }
        
    }

    public void SetMesh(Mesh mesh)
    {
        _mesh = mesh;
    }
    
    public void SetMaterial(Material m)
    {
        _material = m;
    }

    public void SetTransforms(List<FlyweightTransform> transforms)
    {
        _flyweightMatricies.Clear();
        _flyweightMatricies.Capacity = transforms.Count;
        foreach (var t in transforms)
        {
            AddTransform(t);
        }

        _disabledIndexes.Clear();
        _totalCount = _flyweightMatricies.Count;
    }

    public void AddTransform(FlyweightTransform transform)
    {
        Matrix4x4 newMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        _flyweightMatricies.Add(newMatrix);
        _totalCount = _flyweightMatricies.Count;
    }
    
    /*
    //due to floating point errors this may not be reliable. USe disable / enable transfrom instead where possible
    public void RemoveTransform(FlyweightTransform transform)
    {
        Matrix4x4 newMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        _flyweightMatricies.Remove(newMatrix);
    }*/

    public void UpdateTransform(FlyweightTransform transform, int index)
    {
        _flyweightMatricies[index] = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    }

    public void DisableTransform(int index)
    {
        _disabledIndexes.Add(index);
    }
    
    public void EnableTransform(int index)
    {
        _disabledIndexes.Remove(index);
    }

    public int GetNextIndex()
    {
        return _totalCount;
    }
    
    void SendDrawCall(int batchSize)
    {
        Graphics.DrawMeshInstanced(_mesh, 0, _material, _batchMatricies, batchSize);
    }
    
}
