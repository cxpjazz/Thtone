using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreHelper
{
	[AttributeUsage(AttributeTargets.Field)]

	public class ItemDiscAttribute : Attribute
	{

		/// <summary>

		///

		/// </summary>

		private string _Description;



		/// <summary>

		///

		/// </summary>

		public string Description
		{

			get { return _Description; }

			set { _Description = Description; }

		}



		/// <summary>

		///

		/// </summary>

		/// <param name="Description"></param>

		public ItemDiscAttribute(string Description)
		{

			_Description = Description;

		}

	}
}
