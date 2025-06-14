using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TermUI.Core.ObjectTable;
using TermUI.Core.View;
using TermUI.Model;

namespace TermUI.Commands.Threads;

public class ThreadView : ViewBase
{
    private DumpModel DumpModel { get; }
    private readonly ObjectTableView<ClrThreadInfo> tvThreads;
    private readonly ObjectTableSource<ClrThreadInfo> otsThreads;
    private readonly ObjectTableView<ClrStackFrameInfo> tvCallStackFrames;
    private readonly ObjectTableSource<ClrStackFrameInfo> otsCallStackFrames;
    private readonly ObjectTreeView<IClrObjectInfoExt> tvStackObjects;
    
    public ThreadView(DumpModel dumpModel)
    {
        DumpModel = dumpModel;
        
        var clrThreadInfos = DumpModel
            .GetThreadInfos()
            .OrderBy(info => info.Id)
            .ToArray();
        otsThreads = new ObjectTableSource<ClrThreadInfo>(clrThreadInfos);
        tvThreads = new ObjectTableView<ClrThreadInfo>(otsThreads)
        {
            Y = 1  
        };
        
        tvThreads.SelectedCellChanged += OnSelectedRowChanged;
        var filter = new ObjectTableFilter<ClrThreadInfo>(tvThreads, nameof(ClrThreadInfo.Name))
        {
            Width = Dim.Fill()
        };

        otsCallStackFrames = new ObjectTableSource<ClrStackFrameInfo>([]);
        tvCallStackFrames = new ObjectTableView<ClrStackFrameInfo>(otsCallStackFrames)
        {
            Y = 0
        };

        var callStackView = new FrameView
        {
            Title = "Call Stack",
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        callStackView.Add(tvCallStackFrames);
        
        var leftView = new TileView
        {
            Orientation = Orientation.Horizontal,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        var stackView = new FrameView
        {
            Title = "Stack",
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        tvStackObjects = new ObjectTreeView<IClrObjectInfoExt>([], render => render.Name, info => info.GetFields());
        stackView.Add(tvStackObjects);

        leftView.Tiles.First().ContentView?.Add(filter, tvThreads);
        leftView.Tiles.Last().ContentView?.Add(callStackView);
        
        var mainView = new TileView(2)
        {
            Orientation = Orientation.Vertical,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        mainView.Tiles.First().ContentView?.Add(leftView);
        mainView.Tiles.Last().ContentView?.Add(stackView);
        Add([mainView]);
        tvThreads.SetFocus();
        OnSelectedRowChanged(tvThreads, new SelectedCellChangedEventArgs(otsThreads, 0, 0, 0, 0));
    }

    private void OnSelectedRowChanged(object? sender, SelectedCellChangedEventArgs e)
    {
        if (e.NewRow < 0 || e.NewRow >= otsThreads.DisplayValues.Length)
        {
            return;
        }
        
        ClrThreadInfo clrThreadInfo = otsThreads.DisplayValues[e.NewRow];
        var frames = clrThreadInfo
            .ClrStackFrames()
            .Select(frame => new ClrStackFrameInfo(frame))
            .ToArray();
        otsCallStackFrames.Init(frames);
        tvCallStackFrames.NeedsDraw = true;

        var clrObjectInfos = DumpModel.GetStackObjects(clrThreadInfo)
            .Select((clrValue, i) => new ClrObjectInfoExt($"#{i}", clrValue))
            .ToArray();
        
        tvStackObjects.Init(clrObjectInfos);
        tvStackObjects.NeedsDraw = true;
    }
}