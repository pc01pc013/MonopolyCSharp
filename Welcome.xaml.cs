using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MonopolyCSharp
{
    /// <summary>
    /// Welcome表單傳送初始遊戲資料結構
    /// </summary>
    public struct GameInitData
    {
        public int playerNum;  //統計玩家人數
        public string[] playerName;  //統計玩家名稱
        public string[] characterName;  //統計玩家選擇角色
    }
    /// <summary>
    /// Welcome.xaml 的互動邏輯
    /// </summary>
    public partial class Welcome : Window
    {
        private struct comboboxNameData
        {
            public string[] strName;
            public bool[] CheckPoint;
        }
        //=========================================================
        List<CheckBox> checkboxPerson = new List<CheckBox>();
        List<TextBox> textboxPerson = new List<TextBox>();
        List<ComboBox> comboboxPerson = new List<ComboBox>();
        //=========================================================
        GameInitData _initData = new GameInitData();
        comboboxNameData _comboboxCheckData = new comboboxNameData();
        int _isClickStartButton = 0;  //是否有按下開始按鈕標記
        public Welcome()
        {
            InitializeComponent();
            //========================定義物件陣列(泛型)
            checkboxPerson.Add(checkBox_player1);
            checkboxPerson.Add(checkBox_player2);
            checkboxPerson.Add(checkBox_player3);
            checkboxPerson.Add(checkBox_player4);
            textboxPerson.Add(textBox_player1);
            textboxPerson.Add(textBox_player2);
            textboxPerson.Add(textBox_player3);
            textboxPerson.Add(textBox_player4);
            comboboxPerson.Add(comboBox_player1);
            comboboxPerson.Add(comboBox_player2);
            comboboxPerson.Add(comboBox_player3);
            comboboxPerson.Add(comboBox_player4);
            //==========================
            SetComboBoxValue();
            SetCheckBoxEvent();
        }
        private void SetCheckBoxEvent()
        {
            for(int i = 0; i < 4; i++)
            {
                checkboxPerson[i].Click += new RoutedEventHandler(EventCheckBox_Click);
            }
        }
        private void SetComboBoxValue()
        {
            _comboboxCheckData.strName=new string[] { "大雄", "哆啦A夢", "靜香", "胖虎" };
            _comboboxCheckData.CheckPoint = new bool[] { false, false, false, false };
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    comboboxPerson[i].Items.Add(_comboboxCheckData.strName[j]);
                }
            }
        }
        private void EventCheckBox_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 4; i++)
            {
                if (e.Source.Equals(checkboxPerson[i]) == true)
                {
                    if (checkboxPerson[i].IsChecked == true)
                    {
                        textboxPerson[i].IsEnabled = true;
                        comboboxPerson[i].IsEnabled = true;
                    }
                    else
                    {
                        textboxPerson[i].IsEnabled = false;
                        comboboxPerson[i].IsEnabled = false;
                    }
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_isClickStartButton == 0)
            {
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }
            else
            {
                _isClickStartButton = 0;
            }
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            //開始遊戲初始化
            //比對資料
            int _playerNum = 0;  //統計玩家人數
            int i, j; //暫時變數
            for (i = 0; i < 4; i++)
            {
                if (checkboxPerson[i].IsChecked == true)
                {
                    ++_playerNum;
                    textboxPerson[i].Text.Trim();
                    if (textboxPerson[i].Text.Length == 0 || comboboxPerson[i].SelectedIndex == -1)
                    {
                        MessageBox.Show("玩家資訊輸入不完整!",
                            "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            if (_playerNum < 2)
            {
                MessageBox.Show("玩家數量不足2位!",
                      "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _initData.playerNum = _playerNum;
            _initData.playerName = new string[_playerNum];
            _initData.characterName = new string[_playerNum];
            for (i = 0, j=0; i < 4; i++)
            {
                if (checkboxPerson[i].IsChecked == true)
                {
                    _initData.playerName[j] = textboxPerson[i].Text;
                    _initData.characterName[j] = comboboxPerson[i].SelectedItem.ToString();
                    ++j;
                }
            }
            _isClickStartButton = 1;
            this.Close();
        }
        public GameInitData GetGameInitData()
        {
            return this._initData;
        }
    }
}
