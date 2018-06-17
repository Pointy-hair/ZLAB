using System;
using System.Threading.Tasks;

namespace Zlab.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(Test().GetAwaiter().GetResult());
            Console.ReadLine();
        }
        static async Task<int> Test()
        {
            GetTest2();
            GetTest1();
            Random R = new Random();
            if (R.Next(0, 2) == 0)
                return await GetTest1();
            
            return await GetTest2();
        }
        static async Task<int> GetTest1()
        {
            await Task.Delay(1000);
            Console.WriteLine("test1");
            return await Task.FromResult(1);
        }
        static async Task<int> GetTest2()
        {
            await Task.Delay(1000);
            Console.WriteLine("test2");
            return await Task.FromResult(2);
        }
    }
}
