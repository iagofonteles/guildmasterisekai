using System.Collections.Generic;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class CellCommand : IBuildCommand
    {
        private readonly GridBuilder _builder;
        private bool _done;
        public OperationCost cost = new();
        public List<IGridCell> before = new();
        public List<IGridCell> after = new();

        public CellCommand(GridBuilder builder) => _builder = builder;

        public bool Do()
        {
            if (_done) return false;
            if (!_builder.Bag.CanPay(cost)) return false;
            _builder.Bag.Pay(cost);
            foreach (var cell in after)
                _builder.Grid.ApplyState(cell);
            _done = true;
            return true;
        }

        public bool Undo()
        {
            if (!_done) return false;
            if (!_builder.Bag.CanRefund(cost)) return false;
            _builder.Bag.Refund(cost);
            foreach (var cell in before)
                _builder.Grid.ApplyState(cell);
            _done = false;
            return true;
        }
    }
}