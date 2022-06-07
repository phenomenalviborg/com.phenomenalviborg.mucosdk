using UnityEngine; // TODO: REMOVE

namespace PhenomenalViborg.Networking
{
    // 
    public class NetworkIdentity
    {
        System.UInt16 NetworkIdentifier;
    }

    #region Replicated properties

    public class ReplicatedVariable<T>
    {
        public static implicit operator T(ReplicatedVariable<T> instance)
        {
            return instance.Data;
        }

        public static implicit operator ReplicatedVariable<T>(T value)
        {
            // Called everytime the value is implicitly set, e.g.:
            // ReplicatedVariable<T> var = 234.0f;
            // var = 123.45f; Called here
            // var += 0.5f; Called here

            // TODO: Add replication code hereStackFrame

            // THIS IS MESSY, and VERY slow...
            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1);
            System.Type owningType = stackFrame.GetMethod().ReflectedType;
            Debug.Log(owningType);

            return new ReplicatedVariable<T> { Data = value };
        }

        T Data;
    }

    public class ReplicatedEvent
    {

    }

    public class ReplicatedEvent<T> where T : System.EventArgs
    {

    }

    #endregion
}