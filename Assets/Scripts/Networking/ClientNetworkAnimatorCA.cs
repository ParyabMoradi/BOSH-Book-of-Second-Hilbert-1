using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class ClientNetworkAnimatorCA : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}