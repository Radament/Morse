using System;
using System.Windows.Forms;

public class SettingsPanel : UserControl

{
    private CWDecoder _decoder;
    private ComboBox _timeoutSelector;
    private ComboBox _alphabetSelector;
    private string _pathToAlphabet = "Plugins/CWDecoder_Alphabet.xml";

    public SettingsPanel()
    {
        _timeoutSelector = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Left = 10,
            Top = 10,
            Width = 100
        };
        _timeoutSelector.Items.AddRange(new object[] { "0.5s", "1s", "2s" });
        _timeoutSelector.SelectedIndexChanged += TimeoutSelector_SelectedIndexChanged;
        Controls.Add(_timeoutSelector);

        _alphabetSelector = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Left = 10,
            Top = 40,
            Width = 100
        };
        _alphabetSelector.Items.AddRange(new object[] { "English", "Russian" });
        _alphabetSelector.SelectedIndexChanged += AlphabetSelector_SelectedIndexChanged;
        Controls.Add(_alphabetSelector);
        this.ResumeLayout(false);
    }


    public void SetDecoder(CWDecoder decoder)
    {
        _decoder = decoder;
    }

    private void TimeoutSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (_timeoutSelector.SelectedItem.ToString())
        {
            case "0.5s":
                _decoder.SetMessageTimeout(TimeSpan.FromSeconds(0.5));
                break;
            case "1s":
                _decoder.SetMessageTimeout(TimeSpan.FromSeconds(1));
                break;
            case "2s":
                _decoder.SetMessageTimeout(TimeSpan.FromSeconds(2));
                break;
        }
    }

    private void AlphabetSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_alphabetSelector.SelectedItem != null)
        {
            string selectedAlphabet = _alphabetSelector.SelectedItem.ToString();
            _decoder.LoadAlphabetFromXml(_pathToAlphabet, selectedAlphabet);
        }
    }
}