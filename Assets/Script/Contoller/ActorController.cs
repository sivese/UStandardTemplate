using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Std.Controller
{
    public class ActorController : Collider
    {
        public bool IsGrounded => isGrounded;
        private bool isGrounded;

        /* Capsule size properties */
        private float _radius;
        private float _height;

        public void SetCollisionSize(float? radius, float? height)
        {
            _radius = radius ?? _radius;
            _height = height ?? _height;
        }

        public void Move()
        {

        }
    }
}