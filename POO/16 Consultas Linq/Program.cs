List<int> nums = [3, 5, 6, 12, 7, 3, 8, 9, 23, 54];
List<int> pares = (from n in nums where n % 2 == 0 select n).ToList();
List<int> pares2 = nums.Where(n => n % 2 == 0).ToList();

int[] arrayNums = [3, 5, 6, 12, 7, 3, 8, 9, 23, 54];

// Los métodos Linq son comunes a todas las colecciones
Console.WriteLine(nums.All(n => n < 100));
Console.WriteLine(arrayNums.All(n => n < 100));

var masNums = nums.Concat([1, 2]);
List<int> masNums2 = [..nums, 1, 2];
Console.WriteLine(string.Join(",", masNums));
Console.WriteLine(string.Join(",", masNums2));


