using System;
using UnityEngine;

namespace Utils
{
    public static class ComponentExtensions
    {
        public static bool GetComponent<T>(Component parent, ref T component) where T: Component
        {
            if (GetComponent(ref component, parent.GetComponent<T>))
            {
                return true;
            }
            
            if (GetComponent(ref component, parent.GetComponentInChildren<T>))
            {
                return true;
            }
            
            if (GetComponent(ref component, parent.GetComponentInParent<T>))
            {
                return true;
            }

            return false;
        }
        
        public static bool GetComponent<T>(ref T component, Func<T> getComponent) where T: Component
        {
            if (component)
            {
                return true;
            }

            component = getComponent();
            return component;
        }
    }
}
