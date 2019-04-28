using Caliburn.Micro;
using Keeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Keeper
{
    class Bootstrapper :BootstrapperBase
    {
		public Bootstrapper()
		{
			Initialize();
			
		}
		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			DisplayRootViewFor<ShellViewModel>();
		}

		protected override void Configure()
		{
			MessageBinder.SpecialValues.Add("$mousepoint", ctx =>
			 {
				 var sl = ctx.Source as Slider;
				 var e = ctx.EventArgs as MouseEventArgs;
				 if (e == null)
					 return null;
				 return e.GetPosition(sl);
			 });

		}
	}
}
