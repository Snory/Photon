using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviourPun
{
    public bool IsMine { get; set; }
    private Rigidbody2D _body;
    public UnitMovement Movement { get; private set; }
    private bool _selected;

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            Movement.DisplayMovementArea(value);
        }
    }


    private void Awake()
    {
        _body = this.GetComponent<Rigidbody2D>();
        Movement = this.GetComponent<UnitMovement>();
    }


    [PunRPC]
    public void Initialize(bool isMine)
    {
        IsMine = isMine;
        if (!isMine)
            _body.isKinematic = true;

        GameManager.Instance.Units.Add(this);


    }
}
