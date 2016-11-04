using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Shapes;

namespace MonopolyCSharp
{
    /// <summary>地圖分類列舉</summary>
    public enum SquareClassification : int
    {
        /// <summary>起點</summary>
        Start = 0,
        /// <summary>一般景點</summary>
        Normal = 1,
        /// <summary>特殊景點</summary>
        Special = 2
    }
    /// <summary>地圖事件列舉</summary>
    public enum SquareEventData : int
    {
        /// <summary>無事件</summary>
        Nothing = 0,
        /// <summary>起點事件</summary>
        StartEvent = 1,
        /// <summary>特殊景點事件</summary>
        SpecialEvent = 2
    }
    /// <summary>
    /// MonopolyCSharp-地圖
    /// </summary>
    class Square
    {
        /// <summary>地圖圖片表示</summary>
        Image picme = new Image();
        /// <summary>地圖名稱</summary>
        public string pName { get; set; }
        /// <summary>地圖每一地點之座標位置</summary>
        private int[,] MapXY;
        /// <summary>地圖總共地點數量</summary>
        public int MapPointNum { get; set; }
        /// <summary>地圖格子物件</summary>
        private SquareGrid[] sGird;
        //===========================
        public Square(AddGirdConnect APGCMain)
        {
            picme.Source = new BitmapImage(new Uri("images/monopoly.jpg", UriKind.Relative));
            picme.Stretch = Stretch.Uniform;
            picme.Width = 591;
            picme.Height = 593;
            picme.Visibility = Visibility.Visible;
            APGCMain(picme);
            picme.HorizontalAlignment = HorizontalAlignment.Left;
            picme.VerticalAlignment = VerticalAlignment.Top;
            picme.Margin = new Thickness(50, 62, 0, 0);
            //==============
            MapPointNum = 40; //monopoly地圖
            squareInitXY();
            squareInitGird();
        }
        /// <summary>
        ///地圖格子初始化作業
        /// </summary>
        private void squareInitGird()
        {
            sGird = new SquareGrid[40];
            for(int i = 1; i <= 40; i++)
            {
                switch (i)
                {
                    case 1:
                        sGird[i - 1] = new SquareGrid(SquareClassification.Start,SquareEventData.StartEvent,
                            0,0,0,0,"起點");
                        break;
                    case 11:
                    case 21:
                    case 31:
                        sGird[i - 1] = new SquareGrid(SquareClassification.Special, SquareEventData.SpecialEvent,
                            2000,1500,0,0,"Name(特別)-"+i);
                        break;
                    default:
                        sGird[i - 1] = new SquareGrid(SquareClassification.Normal, SquareEventData.Nothing,
                            1000,800,1000,1200,"Name(一般)-"+i);
                        break;
                }
            }
        }
        /// <summary>
        /// 地圖定位點初始化作業
        /// </summary>
        private void squareInitXY()
        {
            MapXY = new int[40, 2];  //monopoly地圖共40格，大格80*80，小格48*80
            for(int i = 0; i < 4; i++)
            {
                for(int j = 1; j <= 10; j++)
                {
                    if (j <=2)
                    {
                        switch (i)
                        {
                            //(1)40(大格中間值)//(2)(80(大格)+24(小格中間值))
                            case 0:
                                MapXY[(i * 10 + j) - 1, 0] = 552;
                                MapXY[(i * 10 + j) - 1, 1] = 40 + 64 * (j - 1);
                                break;
                            case 1:
                                MapXY[(i * 10 + j) - 1, 0] = 552 - 64 * (j - 1);
                                MapXY[(i * 10 + j) - 1, 1] = 552;
                                break;
                            case 2:
                                MapXY[(i * 10 + j) - 1, 0] = 40;
                                MapXY[(i * 10 + j) - 1, 1] = 552 - 64 * (j - 1);
                                break;
                            case 3:
                                MapXY[(i * 10 + j) - 1, 0] = 40 + 64 * (j - 1);
                                MapXY[(i * 10 + j) - 1, 1] = 40;
                                break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            //40(補大格中間值)+24(小格中間值)+48*n
                            case 0:
                                MapXY[(i * 10 + j) - 1, 0] = 552;
                                MapXY[(i * 10 + j) - 1, 1] = 104+ 48 * (j - 2); 
                                break;
                            case 1:
                                MapXY[(i * 10 + j) - 1, 0] = 488 - 48 * (j - 2);
                                MapXY[(i * 10 + j) - 1, 1] = 552;
                                break;
                            case 2:
                                MapXY[(i * 10 + j) - 1, 0] = 40;
                                MapXY[(i * 10 + j) - 1, 1] = 488 - 48 * (j - 2);
                                break;
                            case 3:
                                MapXY[(i * 10 + j) - 1, 0] = 104 + 48 * (j - 2);
                                MapXY[(i * 10 + j) - 1, 1] = 40;
                                break;
                        }
                    }
                    MapXY[(i * 10 + j) - 1, 0] += (int) picme.Margin.Left;
                    MapXY[(i * 10 + j) - 1, 1] += (int) picme.Margin.Top;
                }
            }
        }
        /// <summary>
        /// 取得地圖定位點(X)
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns></returns>
        public int GetMapX(int _mapNum)
        {
            return MapXY[_mapNum - 1, 0];
        }
        /// <summary>
        /// 取得地圖定位點(Y)
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns></returns>
        public int GetMapY(int _mapNum)
        {
            return MapXY[_mapNum - 1, 1];
        }
        /// <summary>
        /// 人物到達地圖格子應對函式-第一序位
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <param name="_personNum">人物編號</param>
        public SquareGridRunFucData GridRunFunctionFirst(int _mapNum,int _personNum)
        {
            SquareGridRunFucData _data;
            switch (sGird[_mapNum-1].Classification)
            {
                //踩到一般景點時
                case SquareClassification.Normal:
                    //若該格子產權所有人為自身時
                    if (sGird[_mapNum - 1].RegisterPersonNum == _personNum) _data.Value = SquareGridRunFucValue.BulidHouse;
                    //若該格子產權所有人為別人時
                    else if (sGird[_mapNum - 1].RegisterPersonNum != 0) _data.Value = SquareGridRunFucValue.PayMoney;
                    //否則為空地
                    else _data.Value = SquareGridRunFucValue.Buy;
                    break;
                //踩到特殊景點時
                case SquareClassification.Special:
                    //若該格子產權所有人為自身時
                    if (sGird[_mapNum - 1].RegisterPersonNum == _personNum) _data.Value = SquareGridRunFucValue.Nothing;
                    //若該格子產權所有人為別人時
                    else if (sGird[_mapNum - 1].RegisterPersonNum != 0) _data.Value = SquareGridRunFucValue.PayMoney;
                    //否則為空地
                    else _data.Value = SquareGridRunFucValue.Buy;
                    break;
                default:
                    _data.Value = SquareGridRunFucValue.Nothing;
                    break;
            }
            _data.Eventdata = sGird[_mapNum - 1].EventData;
            return _data;
        }
        /// <summary>
        /// 取得地圖格子之房子建造高度
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>建造高度</returns>
        public int GetGridBulidHeight(int _mapNum)
        {
            return sGird[_mapNum - 1].BulidHeight;
        }
        /// <summary>
        /// 取得地圖格子之總價值
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>格子總價值</returns>
        public int GetGridValue(int _mapNum)
        {
            //格子價值+房子價值*房子建造高度
            return sGird[_mapNum - 1].SquareValue+ sGird[_mapNum - 1].BulidHouseValue* sGird[_mapNum - 1].BulidHeight;
        }
        /// <summary>
        /// 取得地圖格子之購買本值
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>格子購買本值</returns>
        public int GetGridBuyValue(int _mapNum)
        {
            return sGird[_mapNum - 1].SquareBuyValue;
        }
        /// <summary>
        /// 取得地圖格子之房子購買本值
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>房子購買本值</returns>
        public int GetGridHouseBuyValue(int _mapNum)
        {
            return sGird[_mapNum - 1].BulidHouseBuyValue;
        }
        /// <summary>
        /// 取得地圖格子之名稱
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>地圖格子名稱</returns>
        public string GetGridName(int _mapNum)
        {
            return sGird[_mapNum - 1].SquareGridName;
        }
        /// <summary>
        /// 取得地圖格子之產權登記歸屬
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <returns>登記歸屬之人物編號</returns>
        public int GetGridRegisterPerson(int _mapNum)
        {
            return sGird[_mapNum - 1].RegisterPersonNum;
        }
        /// <summary>
        /// 設定地圖格子產權歸屬
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        /// <param name="_personNum">人物編號</param>
        public void SetGridRegisterPerson(int _mapNum, int _personNum)
        {
            sGird[_mapNum - 1].RegisterPersonNum = _personNum;
        }
        /// <summary>
        /// 設定地圖格子標示點
        /// </summary>
        /// <param name="AGC">委派方法-物件加入主畫面</param>
        /// <param name="_mapNum">地圖編號</param>
        /// <param name="_color">標示點顏色</param>
        public void SetGridRegisterPoint(AddGirdConnect AGC, int _mapNum, Color _color)
        {
            sGird[_mapNum - 1].SetRegisterPoint(AGC, GetMapX(_mapNum), GetMapY(_mapNum), _color);
        }
        /// <summary>
        /// 設定地圖格子增加房子高度
        /// </summary>
        /// <param name="_mapNum">地圖編號</param>
        public void SetGridBulidHouse(int _mapNum)
        {
            ++sGird[_mapNum - 1].BulidHeight;
        }
        /// <summary>
        /// MonopolyCSharp-地圖-地圖格子
        /// </summary>
        class SquareGrid
        {
            /// <summary>地圖分類</summary>
            public SquareClassification Classification { get; set; }
            /// <summary>地圖事件</summary>
            public SquareEventData EventData { get; set; }
            /// <summary>地圖格子房子建造高度</summary>
            public int BulidHeight { get; set; }
            /// <summary>地圖格子產權登記歸屬</summary>
            public int RegisterPersonNum
            {
                get
                {
                    return _registerpersonnum;
                }
                set
                {
                    if (this.Classification == SquareClassification.Normal || this.Classification==SquareClassification.Special)
                    {
                        _registerpersonnum = value;
                    }
                }
            }
            private int _registerpersonnum = 0;
            /// <summary>地圖格子價值</summary>
            public int SquareValue { get; set; }
            /// <summary>地圖格子房子價值</summary>
            public int BulidHouseValue { get; set; }
            /// <summary>地圖格子購買本值</summary>
            public int SquareBuyValue { get; set; }
            /// <summary>地圖格子房子購買本值</summary>
            public int BulidHouseBuyValue { get; set; }
            /// <summary>地圖格子名稱</summary>
            public string SquareGridName { get; set; }
            private Ellipse RegisterPoint =new Ellipse();
            /// <summary>
            /// 地圖格子-建構式
            /// </summary>
            /// <param name="sc1">地圖分類</param>
            /// <param name="_event">地圖事件</param>
            /// <param name="_buyValue">地圖格子購買本值</param>
            /// <param name="_totValue">地圖格子價值</param>
            /// <param name="_bulidValue">地圖格子房子購買本值</param>
            /// <param name="_houseValue">地圖格子房子總價值</param>
            /// <param name="_name">地圖格子名稱</param>
            public SquareGrid(SquareClassification sc1, SquareEventData _event, int _buyValue, int _totValue,
               int _bulidValue, int _houseValue, string _name)
            {
                Classification = sc1;
                EventData = _event;
                SquareBuyValue = _buyValue;
                SquareValue = _totValue;
                BulidHouseValue = _houseValue;
                BulidHouseBuyValue = _bulidValue;
                SquareGridName = _name;
                BulidHeight = 0;
            }
            /// <summary>
            /// 設定地圖格子標示點
            /// </summary>
            /// <param name="AGC">委派方法-物件加入主畫面</param>
            /// <param name="_X">座標X</param>
            /// <param name="_Y">座標Y</param>
            /// <param name="_color">標示點顏色</param>
            public void SetRegisterPoint(AddGirdConnect AGC, int _X, int _Y, Color _color)
            {
                RegisterPoint.Width = 40;
                RegisterPoint.Height = 40;
                RegisterPoint.Fill = new SolidColorBrush(_color);
                RegisterPoint.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                RegisterPoint.Opacity = 0.70;
                RegisterPoint.HorizontalAlignment = HorizontalAlignment.Left;
                RegisterPoint.VerticalAlignment = VerticalAlignment.Top;
                RegisterPoint.Visibility = Visibility.Visible;
                AGC(RegisterPoint);
                RegisterPoint.Margin = new Thickness(_X-(RegisterPoint.Width/2), _Y- (RegisterPoint.Height / 2), 0, 0);
            }
        }
    }
}
