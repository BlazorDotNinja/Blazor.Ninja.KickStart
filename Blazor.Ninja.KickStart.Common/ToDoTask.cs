using System;

using Blazor.Ninja.Common.Data;

namespace Blazor.Ninja.KickStart.Common
{
	public class ToDoTask : Ticket
	{
		public DateTime? DueDate { get; set; }
	}
}
