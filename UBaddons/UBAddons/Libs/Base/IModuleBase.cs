using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBAddons.Libs.Base
{
    public interface IModuleBase
    {
        bool ShouldExecuted();

        void OnLoad();

        void Execute();
    }
}
