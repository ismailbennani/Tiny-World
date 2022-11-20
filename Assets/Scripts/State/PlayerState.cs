using System;
using Character.Player;
using UnityEngine;

namespace State
{
    [Serializable]
    public class PlayerState
    {
        public PlayerConfig config;
        public Vector3 position;
    }
}
