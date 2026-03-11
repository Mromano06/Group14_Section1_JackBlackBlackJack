using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol
{
    /// <summary>
    /// Defines methods for serializing and deserializing objects to and from byte arrays.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize from a byte array.</typeparam>
    public interface Serializable<T>
    {
        byte[] Serialize(Type dto);
        T Deserialize(byte[] data);
    }
}
