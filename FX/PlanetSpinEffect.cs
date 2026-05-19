using System;
using UnityEngine;

namespace FX
{
    public class PlanetSpinEffect : MonoBehaviour
    {
        [Header("Animation Settings")]
        public float rotationsPerSecond;

        private void Update()
        {
            transform.localRotation = Quaternion.Euler(0, 0, rotationsPerSecond * Time.time);
        }
    }
}