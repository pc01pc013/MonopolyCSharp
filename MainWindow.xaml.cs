using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Globalization;

namespace MonopolyCSharp
{
    /// <summary>
    /// 加入物件至主視窗委派
    /// </summary>
    /// <param name="obj1">加入物件</param>
    delegate void AddGirdConnect(UIElement obj1);
    //=========================================
    /// <summary>
    /// 人物到達地圖格子應對函式-回應列舉
    /// </summary>
    public enum SquareGridRunFucValue : int
    {
        /// <summary>不執行動作</summary>
        Nothing = 0,
        /// <summary>建造房子操作</summary>
        BulidHouse = 1,
        /// <summary>購買產權操作</summary>
        Buy = 2,
        /// <summary>支付金錢操作</summary>
        PayMoney = 3
    }
    /// <summary>
    /// 人物到達地圖格子應對函式資料結構
    /// </summary>
    public struct SquareGridRunFucData
    {
        /// <summary>人物到達地圖格子應對函式-回應列舉資料</summary>
        public SquareGridRunFucValue Value;
        /// <summary>地圖事件列舉資料</summary>
        public SquareEventData Eventdata;
    }
    /// <summary>
    /// MonopolyCSharp-GUI圖形介面
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>人物數量</summary>
        public int PersonTotNum { get; set; }
        /// <summary>現在輪到之人物編號</summary>
        private int PersonNumNow
        {
            get
            {
                return _personNumNow;
            }
            set
            {
                if (value > PersonTotNum)
                {
                    value -= PersonTotNum;
                    DateAdd(1);  //每輪一輪即過完一天
                }
                _personNumNow = value;
            }
        }
        private int _personNumNow = 0;
        /// <summary>人物當下擲骰後數值</summary>
        private int DiceNowNumber = 0;
        /// <summary>骰子待機動畫暫時變數</summary>
        private int DiceTempNumber
        {
            get
            {
                return _dicetempnumber;
            }
            set
            {
                if (value > 6) value -= 6;
                _dicetempnumber = value;
            }
        }
        private int _dicetempnumber=0;
        /// <summary>主程式-土地物件</summary>
        private Square MapMain;
        /// <summary>主程式-人物物件</summary>
        private Person[] Player;
        /// <summary>骰子待機動畫Timer</summary>
        private DispatcherTimer TrDice = new DispatcherTimer();
        /// <summary>主程式-日期時間物件</summary>
        private DateTime dt { get; set; }
        //=========================================================
        List<Image> imagePersonHead = new List<Image>();
        List<Image> imageDice = new List<Image>();
        List<Label> labelPersonMoney = new List<Label>();
        //=========================================================
        public MainWindow()
        {
            InitializeComponent();
            //=========================定義物件陣列(泛型)
            imagePersonHead.Add(this.imagehead1);
            imagePersonHead.Add(this.imagehead2);
            imagePersonHead.Add(this.imagehead3);
            imagePersonHead.Add(this.imagehead4);
            labelPersonMoney.Add(this.label_money1);
            labelPersonMoney.Add(this.label_money2);
            labelPersonMoney.Add(this.label_money3);
            labelPersonMoney.Add(this.label_money4);
            imageDice.Add(this.MW_DiceImage);
            imageDice.Add(this.MW_DiceImage2);
            //=========================實際編譯階段圖片隱藏
            for (int i = 0; i < 4; i++)
            {
                imagePersonHead[i].Visibility = Visibility.Collapsed;
                labelPersonMoney[i].Visibility = Visibility.Collapsed;
            }
            image_person1.Visibility = Visibility.Collapsed;
            Image_Map.Visibility = Visibility.Collapsed;
            MW_Ellipse1.Visibility = Visibility.Collapsed;
            MW_button1.Visibility = Visibility.Collapsed;
            //===========================
            Welcome welcomeWindow = new Welcome();
            welcomeWindow.ShowDialog();
            //===========================
            MapMain = new Square(MW_AddGirdConnectMain);
            InitPlayerData(welcomeWindow.GetGameInitData());
            //===========================
            dt = new DateTime(2016, 1, 1);
            DateAdd(0);  //顯示日期於視窗上
            DiceControlerInit();
            PersonNextTurn();
            //===========================
        }
        /// <summary>
        /// GUI-人物物件初始化
        /// </summary>
        /// <param name="personNum">第n位</param>
        /// <param name="picNum">圖片資源之編號</param>
        private void PersonNewInit(int personNum,int picNum,string _name)
        {
            PersonInitData _personInitData = new PersonInitData()
            {
                APGCMain = MW_AddGirdConnectMain,
                APGCInformation = MW_AddGirdConnectInformation,
                picNum = picNum,
                InformationXYNum = personNum
            };
            Player[personNum - 1] = new Person(_personInitData);
            Player[personNum - 1].WalkCompleted += new EventHandler(PersonWalkEvent);
            Player[personNum - 1].SetPersonXYN(MapMain.GetMapX(1), MapMain.GetMapY(1), 1);
            Player[personNum - 1].pName = _name;
            Player[personNum - 1].Money_Save(30000);
        }
        /// <summary>
        /// GUI-執行人物行動動畫
        /// </summary>
        /// <param name="personnum">第n位</param>
        /// <param name="walknum">行動長度</param>
        private void PersonWalk(int personnum, int walknum)
        {
            int[][] _pMapXY = new int[walknum+1][];  //記載自目前位置到目的地之座標(移動長度+1)
            int _pmapnum=0;
            for(int i = 0; i < walknum+1; i++)
            {
                //如果欲移動之地圖位置已逾其一輪時
                if (Player[personnum - 1].pMapNowNum + i > MapMain.MapPointNum)
                {
                    _pmapnum = (Player[personnum - 1].pMapNowNum + i) - MapMain.MapPointNum;
                }
                else
                {
                    _pmapnum = Player[personnum - 1].pMapNowNum + i;
                }
                _pMapXY[i] = new int[] { MapMain.GetMapX(_pmapnum), MapMain.GetMapY(_pmapnum) };
            }
            Player[personnum - 1].pMapNowNum = _pmapnum;
            Player[personnum-1].Walk(walknum, _pMapXY);
        }
        /// <summary>
        /// GUI-執行人物行動動畫完畢後觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonWalkEvent(object sender, EventArgs e)
        {
            SquareGridRunFirst();
            PersonNextTurn();
        }
        /// <summary>
        /// GUI-輪至下一位人物執行
        /// </summary>
        private void PersonNextTurn()
        {
            GC.Collect();
            ++PersonNumNow;
            MW_GirdMainZorder(Player[PersonNumNow - 1].GetImageMain());
            DiceStartAnimation();
        }
        /// <summary>
        /// 加入物件至Gird(主畫面)
        /// </summary>
        /// <param name="obj1"></param>
        private void MW_AddGirdConnectMain(UIElement obj1)
        {
            MWGird.Children.Add(obj1);
        }
        /// <summary>
        /// 將物件顯示至最前面(主畫面)
        /// </summary>
        /// <param name="obj1"></param>
        private void MW_GirdMainZorder(UIElement obj1)
        {
            MWGird.Children.Remove(obj1);
            MWGird.Children.Add(obj1);
        }
        /// <summary>
        /// 加入物件至Gird(資訊版)
        /// </summary>
        /// <param name="obj1"></param>
        private void MW_AddGirdConnectInformation(UIElement obj1)
        {
            MWGirdInformation.Children.Add(obj1);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            PersonWalk(1, 30);
            PersonWalk(2, 15);
        }
        /// <summary>
        /// 初始化骰子控制介面
        /// </summary>
        private void DiceControlerInit()
        {
            TrDice.Interval = TimeSpan.FromMilliseconds(50);
            TrDice.Tick += new EventHandler(MW_TrDiceEvent);
            for (int i = 0; i < imageDice.Count; ++i)
            {
                imageDice[i].Source = new BitmapImage(new Uri("images/Dice1.png", UriKind.Relative));
            }
        }
        /// <summary>
        /// 骰子控制介面-開始待機動畫
        /// </summary>
        private void DiceStartAnimation()
        {
            TrDice.Stop();
            DiceTempNumber = 1;
            TrDice.Start();
        }
        /// <summary>
        /// 骰子控制介面-結束待機動畫
        /// </summary>
        private void DiceStopAnimation()
        {
            TrDice.Stop();
        }
        /// <summary>
        /// 骰子控制介面-進行隨機擲骰
        /// </summary>
        /// <returns>擲骰數值</returns>
        private int MW_DiceRandom()
        {
            Random ran1 = new Random(Guid.NewGuid().GetHashCode());
            int[] rnum = new int[] { ran1.Next(1, 6), ran1.Next(1, 6) };
            for (int i = 0; i < imageDice.Count; ++i)
            {
                imageDice[i].Source = new BitmapImage(new Uri("images/Dice" + rnum[i] + ".png", UriKind.Relative));
            }
            return rnum.Sum();
        }
        /// <summary>
        /// 骰子控制介面-待機動畫進行中Timer觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MW_TrDiceEvent(object sender, EventArgs e)
        {
            ++DiceTempNumber;
            for (int i = 0; i < imageDice.Count; ++i)
            {
                imageDice[i].Source = new BitmapImage(new Uri("images/Dice" + DiceTempNumber + ".png", UriKind.Relative));
            }
        }
        private void MW_Button_GO_Click(object sender, RoutedEventArgs e)
        {
            DiceStopAnimation();
            DiceNowNumber = MW_DiceRandom();
            //暫時暫停動作
            Storyboard TrTemp = new Storyboard();  
            TrTemp.Duration = TimeSpan.FromMilliseconds(200);
            TrTemp.Completed += new EventHandler(MW_Button_GO_TrCompletedEvent);
            TrTemp.Begin();
        }
        private void MW_Button_GO_TrCompletedEvent(object sender, EventArgs e)
        {
            PersonWalk(PersonNumNow, DiceNowNumber);
        }
        /// <summary>
        /// 人物到達地圖格子應對函式-第一序位
        /// </summary>
        private void SquareGridRunFirst()
        {
            SquareGridRunFucData _returnMessage = MapMain.GridRunFunctionFirst(Player[PersonNumNow - 1].pMapNowNum, PersonNumNow);
            switch (_returnMessage.Value)
            {
                case SquareGridRunFucValue.BulidHouse:
                    SquareGridRun_BulidHouse();
                    break;
                case SquareGridRunFucValue.Buy:
                    SquareGridRun_Buy();
                    break;
                case SquareGridRunFucValue.PayMoney:
                    SquareGridRun_PayMoney();
                    break;
            }
            if (_returnMessage.Eventdata==SquareEventData.SpecialEvent||
                _returnMessage.Eventdata == SquareEventData.StartEvent)
            {
                //執行事件
            }
        }
        /// <summary>
        /// 人物到達地圖格子應對函式-回應建造房子方法
        /// </summary>
        private void SquareGridRun_BulidHouse()
        {
            MessageBoxResult _result;
            int _mapNum = Player[PersonNumNow - 1].pMapNowNum;
            _result = MessageBox.Show("您是否要在自己的格子「" +
                MapMain.GetGridName(_mapNum) + "」上建造房子？" + Environment.NewLine +
                "房子建造價格：" + MapMain.GetGridHouseBuyValue(_mapNum) + Environment.NewLine +
                "房子已建造：" + MapMain.GetGridBulidHeight(_mapNum) +" 棟",
                "MonopolyCSharp", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (_result == MessageBoxResult.Yes)
            {
                if (Player[PersonNumNow - 1].Money_Pay(MapMain.GetGridHouseBuyValue(_mapNum)) == true)
                {
                    MapMain.SetGridBulidHouse(_mapNum);
                    MessageBox.Show("已建造房子。",
                   "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("您現在並無足夠之金額。",
                    "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        /// <summary>
        /// 人物到達地圖格子應對函式-回應購買產權方法
        /// </summary>
        private void SquareGridRun_Buy()
        {
            MessageBoxResult _result;
            int _mapNum = Player[PersonNumNow - 1].pMapNowNum;
            _result = MessageBox.Show("您是否要購買景點「" +
                MapMain.GetGridName(_mapNum) + "」？" + Environment.NewLine +
                "本值：" + MapMain.GetGridBuyValue(_mapNum),
                "MonopolyCSharp", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (_result == MessageBoxResult.Yes)
            {
                if (Player[PersonNumNow - 1].Money_Pay(MapMain.GetGridBuyValue(_mapNum)) == true)
                {
                    MapMain.SetGridRegisterPerson(_mapNum, PersonNumNow);
                    MapMain.SetGridRegisterPoint(MW_AddGirdConnectMain, _mapNum,
                        GetPointColor(Player[PersonNumNow - 1].picNum));
                    MW_GirdMainZorder(Player[PersonNumNow - 1].GetImageMain());
                    MessageBox.Show("已購買景點。",
                   "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("您現在並無足夠之金額。",
                    "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        /// <summary>
        /// 人物到達地圖格子應對函式-回應支付現金方法
        /// </summary>
        private void SquareGridRun_PayMoney()
        {
            int _mapNum = Player[PersonNumNow - 1].pMapNowNum;
            MessageBox.Show("本景點為 " + Player[MapMain.GetGridRegisterPerson(_mapNum)-1].pName + " 所有" +
                Environment.NewLine + "需支付：" + MapMain.GetGridValue(_mapNum) + "元",
                "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
            if (Player[PersonNumNow - 1].Money_Pay(MapMain.GetGridValue(_mapNum)) == true)
            {
                Player[MapMain.GetGridRegisterPerson(_mapNum) - 1].Money_Save(MapMain.GetGridValue(_mapNum));
            }
            else
            {
                MessageBox.Show("玩家 " + Player[PersonNumNow - 1].pName + " 已無足夠金額支付" +
                    Environment.NewLine + "宣告破產。",
                    "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Warning);
                GameSet_End();
            }
        }
        /// <summary>
        /// 取得標示點顏色
        /// </summary>
        /// <param name="_personNum">人物角色編號</param>
        /// <returns>標示點顏色</returns>
        private Color GetPointColor(int _personNum)
        {
            switch (_personNum)
            {
                case 1:
                    return Color.FromRgb(255, 255, 0);  //黃色
                case 2:
                    return Color.FromRgb(0, 0, 255);  //藍色
                case 3:
                    return Color.FromRgb(255, 128, 255);  //粉色
                case 4:
                    return Color.FromRgb(255, 128, 64);  //橘色
                default:
                    return Color.FromRgb(255, 255, 255);  //白色
            }
        }
        class NormalException : Exception
        {
            public void MsgShow(String str1)
            {
                MessageBox.Show(str1, "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }
        /// <summary>
        /// 初始化GUI人物物件(面向Welcome視窗)
        /// </summary>
        /// <param name="_initData">Welcome表單傳送初始遊戲資料</param>
        private void InitPlayerData(GameInitData _initData)
        {
            PersonTotNum = _initData.playerNum;
            Player = new Person[PersonTotNum];
            for(int i=1;i<= PersonTotNum; i++)
            {
                switch (_initData.characterName[i - 1])
                {
                    case "大雄":
                        PersonNewInit(i, 1, _initData.playerName[i-1]);
                        break;
                    case "哆啦A夢":
                        PersonNewInit(i, 2, _initData.playerName[i - 1]);
                        break;
                    case "靜香":
                        PersonNewInit(i, 3, _initData.playerName[i - 1]);
                        break;
                    case "胖虎":
                        PersonNewInit(i, 4, _initData.playerName[i - 1]);
                        break;
                    default:
                        PersonNewInit(i, 1, _initData.playerName[i - 1]);
                        break;
                }
            }
        }
        /// <summary>
        /// GUI-增加日期並顯示
        /// </summary>
        /// <param name="_num">增加之日期天數</param>
        private void DateAdd(int _num)
        {
            dt = dt.AddDays(_num);
            label_time.Content= dt.ToString("MMMM  dd",CultureInfo.CreateSpecificCulture("en-US"));
        }
        /// <summary>
        /// GameSet_End方法所需之資料儲存物件
        /// </summary>
        class PersonMoneySetItem
        {
            public string Name { get; set; }
            public int Money { get; set; }
        }
        /// <summary>
        /// 遊戲結束執行
        /// </summary>
        private void GameSet_End()
        {
            List<PersonMoneySetItem> PersonMoneySet = new List<PersonMoneySetItem>();
            string strShow;
            int i;  //暫時變數
            for(i = 0; i < PersonTotNum; i++)
            {
                PersonMoneySet.Add(new PersonMoneySetItem() { Name=Player[i].pName, Money=Player[i].GetMoneyValue() });
            }
            //以Linq語法執行依 Money 做遞減排序
            PersonMoneySet = PersonMoneySet.OrderByDescending(x => x.Money).ToList();
            strShow = "遊戲結束" + Environment.NewLine + Environment.NewLine +
                "最終遊戲排名：" + Environment.NewLine;
            for (i = 1; i <= PersonTotNum; i++)
            {
                strShow += "第" + i + "名：" + PersonMoneySet[i - 1].Name + "    金額：" + PersonMoneySet[i - 1].Money + Environment.NewLine;
            }
            MessageBox.Show(strShow, "MonopolyCSharp", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        
    }
}
