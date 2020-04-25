using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyDeleteUtil : MonoBehaviour
{
    public void CopyTransform(Transform transform)
    {
        Instantiate(transform, transform.parent);
    }

    public void DeleteTransform(Transform transform)
    {
        Destroy(transform);
    }
}
