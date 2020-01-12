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
    public class ArmCodeWidget : Box
    {
        public const int BlockLength = 100;
        
        private bool _enableViewer;
        private long _currentAddress;
        private long _previousScrollOffset;

        // ARM ASM controls
        [GUI] Frame _armAsmFrame;
        [GUI] TreeView _armAsmView;
        [GUI] CheckButton _enableAllBreakpointCheck;
        [GUI] ListStore _codeModel;
        [GUI] Scrollbar _scrollbar;
        [GUI] Adjustment _scrollAdjustment;
        [GUI] ScrolledWindow _scrolledWindow;

        // Buffers
        [GUI] TextBuffer _jitEquivCodeBuffer;

        public ArmCodeWidget() : this(new Builder("Ryujinx.Debugger.UI.ArmAsmWidget.glade")) { }

        public ArmCodeWidget(Builder builder) : base(builder.GetObject("_mainBox").Handle)
        {
            builder.Autoconnect(this);

            _enableAllBreakpointCheck.Toggled += _enableAllBreakpointCheck_Toggled;
            _codeModel.SetSortColumnId(-1, SortType.Descending);

            CodeViewer.CodeViewer.Update += CodeViewer_Update;
            _scrollbar.ValueChanged += _scrollBar_ValueChanged;
            _previousScrollOffset = 0;
            _scrolledWindow.Vadjustment.ValueChanged += _scrolledWindow_Changed;
        }

        public void _scrolledWindow_Changed(object sender, EventArgs e)
        {
            long currentScrollOffset = (long)_scrolledWindow.Vadjustment.Value;
            _scrolledWindow.Vadjustment.Value = _previousScrollOffset;
            long scrollDiff = (((long)currentScrollOffset - _previousScrollOffset) / 8) * 8;
            _previousScrollOffset = 0;
            _scrollbar.Value += scrollDiff;
        }

        private void CodeViewer_Update(object sender, EventArgs e)
        {
            _currentAddress = CodeViewer.CodeViewer.ArmStartAddress;
            _scrollAdjustment.Lower = _currentAddress;
            _scrollAdjustment.Upper = CodeViewer.CodeViewer.ArmEndAddress;
           // _scrollAdjustment.StepIncrement = 8;

            Update();
        }

        private void _scrollBar_ValueChanged(object sender, EventArgs e)
        {
            _currentAddress = (long)_scrollbar.Value;

            Update();
        }

        public void Update()
        {
            _currentAddress = (_currentAddress / 4) * 4;
            if (_enableViewer && Enabled)
            {
                try
                {
                    var currentBlock = GetData(CodeType.Aarch64, _currentAddress, BlockLength);

                    ParseData(currentBlock);
                }
                catch (Exception ex)
                {

                }
            }
        }


        public void ParseData(List<CodeInstruction> instructions)
        {
            _codeModel.Clear();

            long endAddress = _currentAddress + (instructions.Count * 4);

            for (long address = _currentAddress; address <= endAddress; address += 4)
            {
                var code = instructions.Find(x=>x.Address == address);

                if (code.Instruction == null)
                {
                    code = new CodeInstruction()
                    {
                        Address = address,
                        Data = new byte[4],
                        Instruction = "und"
                    };
                }

                _codeModel.AppendValues($"0x{(code.Address + ArmStartAddress).ToString("x8")}",
                    ToHex(code.Data),
                    code.Instruction);
            }
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
