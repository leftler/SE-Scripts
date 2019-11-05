using System;
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        private readonly List<IMyTerminalBlock> _blocksWithInventory = new List<IMyTerminalBlock>();
        private readonly List<IMyInventory> _inventories = new List<IMyInventory>();
        private readonly MyIni _myIni = new MyIni();
        
        private const string SECTION_NAME = "InvCompactSettings";
        private const string OPT_IN_WITH_TAG_KEY = "OPT_IN_WITH_TAG";
        private const string TAG_NAME_KEY = "TAG_NAME";
        private const string AUTO_DETECT_CHANGES_KEY = "AUTO_DETECT_CHANGES";

        private const string TAG_NAME_DEFAULT = "InvCompact";


        private readonly bool _optInWithTag;
        private readonly string _tagName;
        private readonly bool _autoDetectChanges;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100 |  UpdateFrequency.Update10 | UpdateFrequency.Once;

            MyIniParseResult result;
            if (!_myIni.TryParse(Me.CustomData, SECTION_NAME, out result))
                throw new Exception(result.ToString());

            _optInWithTag = _myIni.Get(SECTION_NAME, OPT_IN_WITH_TAG_KEY).ToBoolean();
            _tagName = _myIni.Get(SECTION_NAME, TAG_NAME_KEY).ToString(TAG_NAME_DEFAULT);
            _autoDetectChanges = _myIni.Get(SECTION_NAME, AUTO_DETECT_CHANGES_KEY).ToBoolean();
            
            RefreshBlockList();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var refreshMask = UpdateType.Terminal;
            if (_autoDetectChanges)
            {
                refreshMask |= UpdateType.Update100;
            }

            if ((updateSource & refreshMask) != 0)
            {
                RefreshBlockList();
            }

            CompactInventory();
        }

        private void CompactInventory()
        {
            foreach (var inventory in _inventories)
            {
                for (var i = inventory.ItemCount - 1; i > 0; i--) 
                { 
                    inventory.TransferItemTo(inventory, i, stackIfPossible: true); 
                } 
            }
        }

        private void RefreshBlockList()
        {
           GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocksWithInventory, block => 
               block.HasInventory && block.IsSameConstructAs(Me) &&
               (!_optInWithTag || MyIni.HasSection(block.CustomData, _tagName)));

           _inventories.Clear();
           _inventories.AddRange(_blocksWithInventory.SelectMany(block => Enumerable.Range(0, block.InventoryCount).Select(block.GetInventory)));
        }
    }
}
