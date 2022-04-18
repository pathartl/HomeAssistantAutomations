using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Extensions
{
    public static class ButtonExtensions
    {
        public static void OnPress(this ButtonEntity button, Action action)
        {
            button.StateAllChanges().Subscribe(s =>
            {
                if (s.New?.State != s.Old?.State)
                {
                    action();
                }
            });
        }
    }
}
