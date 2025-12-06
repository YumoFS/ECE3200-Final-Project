using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapManager : MonoBehaviour
{
    [SerializeField] private Collider2D selfCollider;
    [SerializeField] private LayerMask layerMask;
    private uint contactBufferSize = 10;  
    private ContactFilter2D contactFilter;
    private Collider2D[] contactColliders;
    private int count;
    private void Awake()
    {
        contactColliders = new Collider2D[contactBufferSize];
        contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;
        contactFilter.layerMask = layerMask;
    }

    private void Update()
    {
        count = selfCollider.OverlapCollider(contactFilter, contactColliders);
    }

    public Collider2D[] GetAllContactColliders()
    {
        return contactColliders;
    }
    public Collider2D GetTopContactCollider()
    {
        if(IsContacting())
        {
            for (int i = 0; i < count; i ++)
            {
                if (contactColliders[i].GetComponent<InteractiveText>().isInteractive)
                    return contactColliders[i];
            }
        }
        return null;
    }
    public int GetContactColliderNum()
    {
        int res = 0;
        for (int i = 0; i < count; i ++)
        {
            if (contactColliders[i].GetComponent<InteractiveText>().isInteractive)
                res ++;
        }
        return res;
    }
    public bool IsContacting()
    {
        return GetContactColliderNum() > 0;
    }

}
