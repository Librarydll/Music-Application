using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.MyHelpers
{
	public static class MathHelper
	{
		public static int GetMinutes(int number)
		{
			int min;
			if(number<60)
			{
				min = 0;
			}
			else
			{
				min = number / 60;
			}
			return min;
		}
		public static int GetSeconds(int number)
		{
			if (number % 60 == 0)
				return 0;
			else
			{
				return number % 60;
			}
		}
	}
}
