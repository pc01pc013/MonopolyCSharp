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
    /// MonopolyCSharp-GUI圖形介面
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[,] MapXY;  //地圖每一地點之座標位置
        private int MapPointNum = 0;  //地圖總共地點數量
        private int[] PersonMapPointNow;  //現在人物之站點編號
        public int PersonTotNum { get; set; }  //人物數量
        private int PersonNumNow=0; //現在輪到之人物編號
        //private string[] PersonName; //人物名稱
        //private int[] PersonMoney;  //人物現金額
        //private int[] PersonDeposit;  //人物存款額
        //=========================================================
        public MainWindow()
        {
            InitializeComponent();
        }
        public void InitMapXY(int New_MapPointNum,int[,] numMapXY)
        {
            MapPointNum = New_MapPointNum;
            MapXY = new int[MapPointNum, 2];
            for(int i = 0; i <= MapPointNum; i++)
            {
                for(int j = 0; j <= 1; j++)
                {
                    MapXY[i, j] = numMapXY[i, j];
                }
            }
        }
        private void SysMessageErrorShow(int num)
        {
            switch (num)
            {
                case 1:
                    MessageBox.Show("GUI:人物初始化錯誤", "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        public void InitPerson(int _personNum,string[] _personName,int _personMoneySet=100000,int _personDepositSet=100000)
        {
            /*try
            {
                PersonTotNum = _personNum;
                PersonName = new string[PersonTotNum];
                PersonMoney = new int[PersonTotNum];
                PersonDeposit = new int[PersonTotNum];
                for(int i = 0; i < PersonTotNum; i++)
                {
                    PersonName[i] = _personName[i];
                    PersonMoney[i] = _personMoneySet;
                    PersonDeposit[i] = _personDepositSet;
                }
            }
            catch
            {
                this.SysMessageErrorShow(1);
            }*/
        }
        public void ScreenSetPersonMoney(int PersonNum,int MoneyNum)
        {

        }
        public void ScreenSetPersonPlay(int PersonNum)
        {

        }
    }
}
