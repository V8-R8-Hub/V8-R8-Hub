using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shared.Util {
	public static class DatabaseUtils {
		public static string CreateConnectionString(ConnectionStringModel model) {
			string format;
			if (model.DatabaseName != null) {
				format = "Server={0}; User Id={1}; Database='{2}'; Port={3}; Password={4};SSLMode=Prefer";
			} else {
				format = "Server={0}; User Id={1}; Port={3}; Password={4};SSLMode=Prefer";
			}
			return string.Format(format, model.Host, model.Username, model.DatabaseName, model.Port ?? 5432, model.Password);
		}
	}
}
