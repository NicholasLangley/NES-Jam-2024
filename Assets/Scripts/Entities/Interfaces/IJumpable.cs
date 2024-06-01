using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJumpable
{
    bool _isJumping { get; set; }

    void Jump();
}
