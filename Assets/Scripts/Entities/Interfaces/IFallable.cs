using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFallable
{
    LayerMask _groundLayers { get; set; }

    BoxCollider2D _groundCollider { get; set; }
    bool _isGrounded { get; set; }
    void CheckIfGrounded();

    void Fall();
}
