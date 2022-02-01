using System;
using System.Linq;
using System.Reflection.Extensions; //https://github.com/c3nb/Reflection

public class Binary
{
    public static implicit operator Binary(byte[] bits) => new Binary(bits);
    public static Binary operator <<(Binary binary, int bitCount)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        Array.Copy(binary.Bits, bitCount, bitArr, 0, binary.Bits.Length - bitCount);
        return new Binary(bitArr);
    }
    public static Binary operator >>(Binary binary, int bitCount)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        Array.Copy(binary.Bits, 0, bitArr, bitCount, binary.Bits.Length - bitCount);
        return new Binary(bitArr);
    }
    public static Binary operator |(Binary binary, Binary dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 || destBin[i] == 1) ? 1 : 0);
        return new Binary(newBin);
    }
    public static Binary operator &(Binary binary, Binary dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 && destBin[i] == 1) ? 1 : 0);
        return new Binary(newBin);
    }
    public static Binary operator ^(Binary binary, Binary dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 && destBin[i] == 0) || (bin[i] == 0 && destBin[i] == 1) ? 1 : 0);
        return new Binary(newBin);
    }
    public static Binary operator ~(Binary binary)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        for (int i = 0; i < bitArr.Length; i++)
            bitArr[i] = (byte)(bitArr[i] == 1 ? 0 : 1);
        return new Binary(bitArr);
    }
    public byte this[int index]
    {
        get => Bits[index];
        set => Bits[index] = value;
    }
    public Binary(byte[] bits) => Bits = bits;
    public byte[] Bits;
    public override string ToString()
    {
        return Bits.Aggregate("", (s, b) => s + b);
    }
    public string GetBinaryInfo()
    {
        return $"Bits:{Bits.Aggregate("", (s, b) => s + b)}" +
            $"\nBits:{Bits.Length}" +
            $"\nBytes:{Bits.Length / 8}";
    }
    public T To<T>() => To<T>(this);
    public static Binary ToBinary<T>(T obj)
    {
        byte Read(ref IntPtr pointer)
        {
            pointer = pointer.Read(out byte value);
            return value;
        }
        int size = Reflection.SizeOf<T>();
        bool[] bits = new bool[size * 8];
        byte[] bytes = new byte[size];
        IntPtr ptr = IntPtrUtils.GetPointer(ref obj);
        for (int i = 0; i < size; i++)
            bytes[i] = Read(ref ptr);
        for (int i = 0; i < size; i++)
            for (int j = 0; j < 8; j++)
                bits[j + i * 8] = ((bytes[i] >> j) & 1) == 1;
        if (BitConverter.IsLittleEndian)
            return bits.Reverse().Select(bit => (byte)(bit ? 1 : 0)).ToArray();
        return bits.Select(bit => (byte)(bit ? 1 : 0)).ToArray();
    }
    public static unsafe T To<T>(Binary binary)
    {
        int bytes = binary.Bits.Length / 8;
        byte* results = stackalloc byte[bytes];
        for (int i = 0; i < bytes; i++)
        {
            byte[] bits = new byte[8];
            Array.Copy(binary.Bits, i * 8, bits, 0, 8);
            if (BitConverter.IsLittleEndian)
                for (int j = 0; j < bits.Length; j++)
                {
                    if (bits[bits.Length - j - 1] == 0) continue;
                    results[bytes - 1 - i] += (byte)Math.Pow(2, j);
                }
            else
                for (int j = 0; j < bits.Length; j++)
                {
                    if (bits[j] == 0) continue;
                    results[i] += (byte)Math.Pow(2, j);
                }
        }
        return IntPtrUtils.As<T>(results);
    }
}
public class Binary<T> : Binary
{
    public static implicit operator T(Binary<T> binary) => binary.GetValue();
    public static implicit operator Binary<T>(T obj) => new Binary<T>(obj);
    public static Binary<T> operator <<(Binary<T> binary, int bitCount)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        Array.Copy(binary.Bits, bitCount, bitArr, 0, binary.Bits.Length - bitCount);
        return new Binary<T>(bitArr);
    }
    public static Binary<T> operator >>(Binary<T> binary, int bitCount)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        Array.Copy(binary.Bits, 0, bitArr, bitCount, binary.Bits.Length - bitCount);
        return new Binary<T>(bitArr);
    }
    public static Binary<T> operator |(Binary<T> binary, Binary<T> dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 || destBin[i] == 1) ? 1 : 0);
        return new Binary<T>(newBin);
    }
    public static Binary<T> operator &(Binary<T> binary, Binary<T> dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 && destBin[i] == 1) ? 1 : 0);
        return new Binary<T>(newBin);
    }
    public static Binary<T> operator ^(Binary<T> binary, Binary<T> dest)
    {
        var bin = binary.Bits;
        var newBin = new byte[bin.Length];
        var destBin = dest.Bits;
        for (int i = 0; i < bin.Length; i++)
            newBin[i] = (byte)((bin[i] == 1 && destBin[i] == 0) || (bin[i] == 0 && destBin[i] == 1) ? 1 : 0);
        return new Binary<T>(newBin);
    }
    public static Binary<T> operator ~(Binary<T> binary)
    {
        var bitArr = (byte[])binary.Bits.Clone();
        for (int i = 0; i < bitArr.Length; i++)
            bitArr[i] = (byte)(bitArr[i] == 1 ? 0 : 1);
        return new Binary<T>(bitArr);
    }
    public Binary(byte[] bits) : base(bits) { }
    public Binary(T obj) : this(ToBinary(obj).Bits) { }
    public T GetValue() => To<T>();
    public void SetValue(T obj) => Bits = ToBinary(obj).Bits;
    public Binary Base => this;
}
