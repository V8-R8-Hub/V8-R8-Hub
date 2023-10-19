using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models {
	public record ConnectionStringModel {
		public required string Host { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
		public ushort? Port { get; set; }
		public string? DatabaseName { get; set; }
	}
}
