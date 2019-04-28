using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.MyHelpers
{
	public static class RandomHelper
	{
		[ThreadStatic] private static Random Local;

		public static Random ThisThreadsRandom
		{
			get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		//Возвращает определеное колличество неповторяющих случайный чисел от 1 до count
		public static List<int> GetRandom(int count)
		{
			List<int> listNumbers = new List<int>();
			Random random = new Random();
			int number;
			for (int i = 0; i < count+1; i++)
			{
				do
				{
					number = random.Next(1, count+2);
				} while (listNumbers.Contains(number));
				listNumbers.Add(number);
			}
			return listNumbers;
		}
	}
}
