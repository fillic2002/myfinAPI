using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class User
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
	}
}
