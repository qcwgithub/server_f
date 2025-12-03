using System.Collections.Generic;
public interface IIsDifferent<T> where T: class
{
    bool IsDifferent(T other);
    void Ensure();
    void DeepCopyFrom(T other);
}