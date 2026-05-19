using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class Tooltip : MonoBehaviour
    {
        public Transform location;
        public string title;
        public string subtitle;
        public string message;

        public bool forceEnabled = false;
        // public string flavor

        private void Awake()
        {
            if (location == null)
            {
                location = transform;
            }
        }
    }
}