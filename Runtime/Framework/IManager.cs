using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
    public abstract class IManager<T> : MonoBehaviour
    {
        private static Dictionary<Type, object> s_Singletons = new Dictionary<Type, object>();

        public static T GetInstance()
        {
            return (T)s_Singletons[typeof(T)];
        }

        void OnEnable()
        {
            if (s_Singletons.ContainsKey(GetType()))
            {
                Debug.LogError(String.Format("{0} is a singleton, multiple instances are not allowed!", GetType().Name));
                Destroy(this);
            }
            else
            {
                s_Singletons.Add(GetType(), this);
            }
        }
    }
}
