using System.Linq;

namespace FindTheSameElementFromTwoList
{
    class Program
    {
        static void Main(string[] args)
        {
            var arrayA = new[] { 1, 2, 3, 4, 5 };
            var arrayB = new[] { 3, 2, 1, 6 };

            //method 1
            var method1 = arrayA.Intersect(arrayB);

            //method2
            var method2 = from item in arrayA
                          where arrayB.Contains(item)
                          select item;

            //method 3
            var method3 = from itemA in arrayA
                          join itemB in arrayB on itemA equals itemB
                          select itemA;
        }
    }
}
