using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luhn_check
{
    class Program
    {
        static void Main(string[] args)
        {
            var tagService = new TagService();

            for(int i = 0;i<50000;i++)
            {
                var tag = tagService.GetUnusedPrimary();
                Console.WriteLine(tag);
            }
            
        }
    }
}
