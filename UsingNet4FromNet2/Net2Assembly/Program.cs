using System;
using Net4ToNet2Adapter;

namespace Net2Assembly
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CLR version from EXE: {0}", Environment.Version);

            var myClassAdapterType = Type.GetTypeFromProgID("Net4ToNet2Adapter.MyClassAdapter");
            var myClassAdapterInstance = Activator.CreateInstance(myClassAdapterType);
            var myClassAdapter = (IMyClassAdapter)myClassAdapterInstance;
            myClassAdapter.DoNet4Action();
        }
    }
}
