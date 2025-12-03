using System.Collections.Generic;
public interface IIsDifferent_Db<T> where T : class
{
    bool DeepCopyFrom(T other);
}