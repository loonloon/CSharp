using System;
using Net2Interface;

namespace Net2Assembly
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CLR version from EXE: {0}", Environment.Version);

            //Tested with console / windows (WPF) application, non exe needs to register to registry by using Regasm.exe?
            var myClassAdapterType = Type.GetTypeFromProgID("Net4ToNet2Adapter.MyClassAdapter");
            var myClassAdapterInstance = Activator.CreateInstance(myClassAdapterType);
            var myClassAdapter = (IMyClassAdapter)myClassAdapterInstance;
            myClassAdapter.DoNet4Action();
        }
    }
}
