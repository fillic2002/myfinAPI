using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class ExpenseDTO
	{
		public int expId { get; set; }
		public int folioId { get; set; }
		public DateTime dtOfTran{ get; set; }
		public double amt { get; set; }
		public string desc{ get; set; }
		public ExpenseType expenseType { get; set; }		

	}
	public class ExpType
	{
		public int expTypeId { get; set; }
		public string expTypeDesc { get; set; }
	}
	public enum ExpenseType
	{
		Internet = 1,
		HouseRent,
		LoanEMI,
		UPI,
		Mobile,
		School,
		Electricity,
		TataSky,
		GasCylinder,
		Sodexo,
		CashExp
	}
	public class MonthlyExpenseDTO
	{
		public double totalExpAmt { get; set; }
		public string monthYear { get; set; }
		public int folioId { get; set; }
	}
}
