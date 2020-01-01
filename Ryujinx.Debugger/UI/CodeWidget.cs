using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Ryujinx.Debugger.CodeViewer;

using Gtk;
using GUI = Gtk.Builder.ObjectAttribute;

using static Ryujinx.Debugger.CodeViewer.CodeViewer;
using static Ryujinx.Common.HexUtils;

namespace Ryujinx.Debugger.UI
{
    public class CodeWidget : Box
    {
        public const int BlockLength = 100;

        private Dictionary<long, CodeInstruction> _currentView;

        private bool _enableViewer;
        private long _armOffset;

        // ARM ASM controls
        [GUI] Frame _armAsmFrame;
        [GUI] TreeView _breakpointTreeView;
        [GUI] TreeView _registersTreeView;
        [GUI] TreeView _armAsmView;
        [GUI] CheckButton _enableAllBreakpointCheck;
        [GUI] ScrolledWindow _armAsmScrollWindow;
        [GUI] ListStore _codeModel;

        // Buffers
        [GUI] TextBuffer _jitEquivCodeBuffer;

        public CodeWidget() : this(new Builder("Ryujinx.Debugger.UI.CodeWidget.glade")) { }

        public CodeWidget(Builder builder) : base(builder.GetObject("_mainBox").Handle)
        {
            builder.Autoconnect(this);

            _enableAllBreakpointCheck.Toggled += _enableAllBreakpointCheck_Toggled;
            _codeModel.SetSortColumnId(-1, SortType.Descending);

            _armAsmScrollWindow.Vadjustment.ValueChanged += _armAsmScrollWindow_ScrollEvent;

            CodeViewer.CodeViewer.Update += CodeViewer_Update;

            _currentView = new Dictionary<long, CodeInstruction>();
        }

        private void _armAsmScrollWindow_ScrollEvent(object o, EventArgs args)
        {
            /*_armOffset = (long)(_armAsmScrollWindow.Vadjustment.Value);

            _armOffset &= ~3;

            Update();*/

            _armAsmView.GetVisibleRange(out TreePath start, out TreePath end);

            //_codeModel.rem()
        }

        private void CodeViewer_Update(object sender, EventArgs e)
        {
            Update();
        }

        private void _armAsmScrollBar_ValueChanged(object sender, EventArgs e)
        {
            
        }

        public string codeString = string.Empty;
        public long pos = 0;

        public void Update()
        {
            if(_enableViewer && Enabled)
            {
                try
                {
                    if ((_armOffset - 200) > 0 || _currentView.Count == 0)
                    {
                        if (_armOffset + 200 > _currentView.LastOrDefault().Key)
                        {
                            var currentBlock = GetData(CodeType.Aarch64, _armOffset, BlockLength);

                            foreach (var instruction in currentBlock)
                            {
                                if (!_currentView.TryAdd(instruction.Key, instruction.Value))
                                {
                                    _currentView[instruction.Key] = instruction.Value;
                                }
                            }
                        }

                        ParseData();
                    }
                }catch(Exception ex)
                {

                }
            }
        }

        public void AddValuesToList(long startAddress, int count)
        {
            long endAddress = startAddress + (count * 4);

            for (long address = startAddress; address <= endAddress; address += 4)
            {
                _currentView.TryGetValue(address, out CodeInstruction code);

                if (code.Instruction == null)
                {
                    code = new CodeInstruction()
                    {
                        Address = address,
                        Data = new byte[4],
                        Instruction = "und"
                    };
                }

                _codeModel.AppendValues($"0x{(code.Address + ArmCodeOffset).ToString("x8")}",
                    ToHex(code.Data),
                    code.Instruction);
            }
        }

        public void ParseData()
        {
            _codeModel.Clear();

            AddValuesToList(0, BlockLength);
        }

        private void _enableAllBreakpointCheck_Toggled(object sender, EventArgs e)
        {

        }

        public void RegisterParentDebugger(DebuggerWidget debugger)
        {
            debugger.DebuggerEnabled += Debugger_DebuggerEnabled; ;
            debugger.DebuggerDisabled += Debugger_DebuggerDisabled; ;
        }

        private void Debugger_DebuggerDisabled(object sender, EventArgs e)
        {
            _enableViewer = false;
        }

        private void Debugger_DebuggerEnabled(object sender, EventArgs e)
        {
            _enableViewer = true;
            Update();
        }
    }
}
