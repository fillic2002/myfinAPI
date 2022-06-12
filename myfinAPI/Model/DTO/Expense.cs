using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class ExpenseDTO
	{
		public int folioId { get; set; }
		public DateTime dtOfTran{ get; set; }
		public double amt { get; set; }
		public string desc{ get; set; }
		public ExpType expenseType { get; set; }		

	}
	public class ExpType
	{
		public int expId { get; set; }
		public string expTypeDesc { get; set; }
	}
	public class MonthlyExpenseDTO
	{
		public double totalExpAmt { get; set; }
		public string monthYear { get; set; }
		public int folioId { get; set; }
	}
}
