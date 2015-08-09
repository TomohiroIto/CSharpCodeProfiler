using CSharpCodeProfiler;
using System;
using System.Data;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataTable table;
            using (CodeProfiler prof = new CodeProfiler())
            {
                TestMethod();

                prof.Stop();
                prof.OutputResult(@"C:\Temp\test.xml");
            }
        }


        static void TestMethod()
        {
            for (int i = 0; i < 10000; i++)
            {
                MethodA();
            }

            for (int j = 0; j < 100; j++)
            {
                MethodB();
            }
        }

        static void MethodA()
        {
            Console.WriteLine("MethodA");
        }

        static void MethodB()
        {
            for (int k = 0; k < 100; k++)
            {
                MethodC();
            }
        }

        static void MethodC()
        {
            Console.WriteLine("MethodC");
        }
    }
}
