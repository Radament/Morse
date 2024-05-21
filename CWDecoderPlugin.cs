using SDRSharp.Common;
using SDRSharp.Radio;
using System.Windows.Forms;

public class CWDecoderPlugin : ISharpPlugin, ICanLazyLoadGui, ISupportStatus, IExtendedNameProvider
{
    private ISharpControl _control;
    private SettingsPanel _settingsPanel;
    private CWDecoder _decoder;
    private CWAudioProcessor _audioProcessor;

    public UserControl GuiControl => _settingsPanel;

    public string DisplayName => "CW Decoder";
    public string Category => "Misc";

    public string MenuItemName => DisplayName;

    public bool IsActive => _settingsPanel != null && _settingsPanel.Visible;

    public UserControl Gui
    {
        get
        {
            LoadGui();
            return _settingsPanel;
        }
    }

    public void LoadGui()
    {
        if (_settingsPanel == null)
        {
            _settingsPanel = new SettingsPanel();
        }
    }

    public void Initialize(ISharpControl control)
    {
        _control = control;
        _settingsPanel = new SettingsPanel();
        _decoder = new CWDecoder();
        _settingsPanel.SetDecoder(_decoder);

        _audioProcessor = new CWAudioProcessor(_decoder);
    }

    public void Close()
    {
        _control.UnregisterStreamHook(_decoder);
    }
}

public class CWAudioProcessor : IRealProcessor
{
    private readonly CWDecoder _decoder;

    public CWAudioProcessor(CWDecoder decoder)
    {
        _decoder = decoder;
    }

    public bool Enabled { get; set; } = false;
    public double SampleRate { set => throw new System.NotImplementedException(); }

    // TODO check this part in debug
    public unsafe void Process(float* buffer, int length)
    {
        _decoder.Process(buffer, length);
    }
}