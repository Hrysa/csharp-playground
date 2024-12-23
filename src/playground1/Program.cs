﻿using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using playground1;
using shared;


var buffer = GC.AllocateUninitializedArray<A>(1024, pinned: true); // Create to pinned object heap

Helper.MeasureGC(() => { Console.WriteLine(buffer); });
Helper.MeasureGC(() =>
{
    var i = 1;
});
Helper.MeasureGC(() => { Console.WriteLine(typeof(A)); });

Helper.MeasureTime(() => { });
Helper.MeasureTime(() =>
{
    var z = 0;
    while (z < 100)
    {
        z++;
        Thread.Sleep(1);
        // SpinWait.SpinUntil(() => false, 1);
    }
}, true);


unsafe
{
    MyStruct myStruct = new MyStruct(); // Create the managed struct
    IntPtr myStructPtr = Marshal.AllocHGlobal(32); // Allocate unmanaged memory for the struct
    Console.WriteLine($"{myStruct.A} {myStruct.B}");


    memset((byte*)myStructPtr, 0, 32);
    byte* arr = (byte*)myStructPtr;

    *(uint*)myStructPtr = 0xffffaa;
    *((uint*)myStructPtr + 1) = 0xff;

    for (int i = 0; i < 32; i++)
    {
        Console.Write($"0x{arr[i]:x2} ");
    }

    // Marshal.StructureToPtr<MyStruct>(myStruct, myStructPtr, false);
    for (int i = 0; i < 32; i++)
    {
        Console.Write($"0x{arr[i]:x2} ");
    }

    Console.WriteLine();
    // arr[0] = 2;

    Console.WriteLine(sizeof(MyStruct));
    MyStruct* s = (MyStruct*)myStructPtr;

    Console.WriteLine(s->A);
}

unsafe void memset(byte* buffer, byte value, int size)
{
    for (int i = 0; i < size; i++)
    {
        *(buffer + i) = value;
    }
}

unsafe MyStruct* Get()
{
    IntPtr myStructPtr = Marshal.AllocHGlobal(32); // Allocate unmanaged memory for the struct

    return (MyStruct*)myStructPtr;
}

struct MyStruct()
{
    public uint A = 0;
    public uint B = 0;
}

class A
{
}