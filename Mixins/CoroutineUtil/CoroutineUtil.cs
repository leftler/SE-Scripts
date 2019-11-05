using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class CoroutineUtil
        {
            private readonly Program _program;
            private IEnumerator _stateMachine;

            public CoroutineUtil(Program program, IEnumerable stateMachineSource)
            {
                _program = program;
                _stateMachine = stateMachineSource.GetEnumerator();
            }

            public void Run()
            {
                if(_stateMachine == null)
                    return;

                if (!_stateMachine.MoveNext())
                {
                    var disposable = _stateMachine as IDisposable;
                    disposable?.Dispose();

                    _stateMachine = null;
                }
                else
                {
                    _program.Runtime.UpdateFrequency |= UpdateFrequency.Once;
                }
            }
        }
    }
}
