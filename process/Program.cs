using PipTemplatesServiceData.Containers;
using System;

namespace PipTemplatesServiceData.Bin
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var task = (new EntitiesProcess()).RunAsync(args);
                task.Wait();
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
